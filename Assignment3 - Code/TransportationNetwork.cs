//2024 CAB301 Assignment 3 
//TransportationNetwok.cs
//Assignment3B-TransportationNetwork

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public partial class TransportationNetwork
{

    private string[]? intersections; //array storing the names of those intersections in this transportation network design
    private int[,]? distances; //adjecency matrix storing distances between each pair of intersections, if there is a road linking the two intersections

    public string[]? Intersections
    {
        get {return intersections;}
    }

    public int[,]? Distances
    {
        get { return distances; }
    }


    //Read information about a transportation network plan into the system
    //Preconditions: The given file exists at the given path and the file is not empty
    //Postconditions: The information about the transportation network plan is read into the system. The intersections are stored in the class field, intersections, and the distances of the links between the intersections are stored in the class fields, distances. 
    public bool ReadFromFile(string filePath)
    {
        try
        {
            var lines = System.IO.File.ReadAllLines(filePath);
            var intersectionSet = new HashSet<string>();

            // First pass: collect all unique intersection names
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length < 2)
                {
                    // Skip incorrectly formatted lines
                    continue;
                }
                intersectionSet.Add(parts[0].Trim());
                intersectionSet.Add(parts[1].Trim());
            }

            // Initialize the arrays
            intersections = intersectionSet.ToArray();
            distances = new int[intersections.Length, intersections.Length];

            // Initialize distances to int.MaxValue
            for (int i = 0; i < intersections.Length; i++)
            {
                for (int j = 0; j < intersections.Length; j++)
                {
                    distances[i, j] = int.MaxValue;
                }
            }

            // Second pass: fill the distances array
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length != 3)
                {
                    // Skip incorrectly formatted lines
                    continue;
                }

                var intersection1 = parts[0].Trim();
                var intersection2 = parts[1].Trim();
                if (!int.TryParse(parts[2].Trim(), out var distance))
                {
                    // Skip lines where the distance is not a valid integer
                    continue;
                }

                var index1 = Array.IndexOf(intersections, intersection1);
                var index2 = Array.IndexOf(intersections, intersection2);

                distances[index1, index2] = distance;
            }

            // Set distance from intersection to itself to 0
            for (int i = 0; i < intersections.Length; i++)
            {
                distances[i, i] = 0;
            }

            return true;
        }
        catch
        {
            // If there's an error reading the file, return false
            return false;
        }
    }



    //Display the transportation network plan with intersections and distances between intersections
    //Preconditions: The given file exists at the given path and the file is not empty
    //Postconditions: The transportation netork is displayed in a matrix format
    public void DisplayTransportNetwork()
    {
        if (intersections == null || distances == null)
        {
            Console.WriteLine("The transportation network is not initialized.");
            return;
        }

        // Print the rows
        for (int i = 0; i < intersections.Length; i++)
        {
            for (int j = 0; j < intersections.Length; j++)
            {
                if (distances[i, j] == int.MaxValue)
                {
                    Console.Write("∞\t");
                }
                else
                {
                    Console.Write($"{distances[i, j]}\t");
                }
            }
            Console.WriteLine();
        }
    }


    //Check if this transportation network is strongly connected. A transportation network is strongly connected, if there is a path from any intersection to any other intersections in thihs transportation network. 
    //Precondition: Transportation network plan data have been read into the system.
    //Postconditions: return true, if this transpotation netork is strongly connected; otherwise, return false. This transportation network remains unchanged.
    public bool IsConnected()
    {
        if (intersections == null || distances == null)
        {
            return false;
        }

        for (int i = 0; i < intersections.Length; i++)
        {
            bool[] visited = new bool[intersections.Length];
            DFS(i, visited);

            for (int j = 0; j < visited.Length; j++)
            {
                if (!visited[j])
                {
                    return false;
                }
            }
        }
    return true;
    }

    private void DFS(int node, bool[] visited)
    {
        if (intersections == null || distances == null)
        {
            return;
        }

        visited[node] = true;

        for (int i = 0; i < intersections.Length; i++)
        {
            if (distances[node, i] != 0 && !visited[i])
            {
                DFS(i, visited);
            }
        }
    }


    //Find the shortest path between a pair of intersections
    //Precondition: transportation network plan data have been read into the system
    //Postcondition: return the shorest distance between two different intersections; return 0 if there is no path from startVerte to endVertex; returns -1 if startVertex or endVertex does not exists. This transportation network remains unchanged.
    public int FindShortestDistance(string startVertex, string endVertex)
    {
        if (intersections == null || distances == null)
        {
            return -1;
        }

        int start = Array.IndexOf(intersections, startVertex);
        int end = Array.IndexOf(intersections, endVertex);

        if (start == -1 || end == -1)
        {
            return -1;
        }

        int[] dist = new int[intersections.Length];
        bool[] visited = new bool[intersections.Length];

        for (int i = 0; i < intersections.Length; i++)
        {
            dist[i] = int.MaxValue;
            visited[i] = false;
        }

        dist[start] = 0;

        for (int count = 0; count < intersections.Length - 1; count++)
        {
            int u = MinDistance(dist, visited);
            visited[u] = true;

            for (int v = 0; v < intersections.Length; v++)
            {
                if (!visited[v] && distances[u, v] != 0 && dist[u] != int.MaxValue && dist[u] + distances[u, v] < dist[v])
                {
                    dist[v] = dist[u] + distances[u, v];
                }
            }
        }

        return dist[end] == int.MaxValue ? 0 : dist[end];
    }

    private int MinDistance(int[] dist, bool[] visited)
    {
        int min = int.MaxValue, min_index = -1;
    
        for (int v = 0; v < intersections.Length; v++)
        {
            if (visited[v] == false && dist[v] <= min)
            {
                min = dist[v];
                min_index = v;
            }
        }
    
        return min_index;
    }


    //Find the shortest path between all pairs of intersections
    //Precondition: transportation network plan data have been read into the system
    //Postcondition: return the shorest distances between between all pairs of intersections through a two-dimensional int array and this transportation network remains unchanged
    public int[,] FindAllShortestDistances()
    {
        if (intersections == null || distances == null)
        {
            return null;
        }
    
        int[,] dist = new int[intersections.Length, intersections.Length];
    
        // Initialize the distance matrix with direct distances
        for (int i = 0; i < intersections.Length; i++)
        {
            for (int j = 0; j < intersections.Length; j++)
            {
                if (i == j)
                {
                    dist[i, j] = 0;
                }
                else if (distances[i, j] != 0)
                {
                    dist[i, j] = distances[i, j];
                }
                else
                {
                    dist[i, j] = int.MaxValue;
                }
            }
        }
    
        // Update the distance matrix to account for shorter paths through intermediate vertices
        for (int k = 0; k < intersections.Length; k++)
        {
            for (int i = 0; i < intersections.Length; i++)
            {
                for (int j = 0; j < intersections.Length; j++)
                {
                    if (dist[i, k] != int.MaxValue && dist[k, j] != int.MaxValue && dist[i, k] + dist[k, j] < dist[i, j])
                    {
                        dist[i, j] = dist[i, k] + dist[k, j];
                    }
                }
            }
        }
    
        return dist;
    }
}