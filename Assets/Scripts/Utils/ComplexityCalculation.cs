using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComplexityCalculation : MonoBehaviour
{
    public static int CalculateDeliveryRoundComplexity(PackageRound packageRound)
    {
        int complexity = 0;
        Vector2[] deliveryLocations = new Vector2[packageRound.Packages.Count];
        int x = 0;
        foreach (Package d in packageRound.Packages)
        {
            var location = d.DeliveryLocation;
            deliveryLocations[x] = (location);
            x++;
        }

        // Loop through all pairs of delivery locations and calculate the shortest path between them
        for (int i = 0; i < deliveryLocations.Length; i++)
        {
            for (int j = i + 1; j < deliveryLocations.Length; j++)
            {
                // Calculate the shortest path between the current pair of delivery locations using A*
                List<Vector2> shortestPath = CalculateShortestPath(deliveryLocations[i], deliveryLocations[j]);

                // Add the length of the shortest path to the total complexity
                complexity += shortestPath.Count;
            }
        }
        
        return complexity;
    }

    private static List<Vector2> CalculateShortestPath(Vector2 start, Vector2 end)
    {
        List<Vector2> shortestPath = new List<Vector2>();

        // Create a list of nodes to explore
        List<Node> openList = new List<Node>();
        openList.Add(new Node(start, null, 0, EstimateDistance(start, end)));

        // Create a list of nodes that have already been explored
        List<Node> closedList = new List<Node>();

        while (openList.Count > 0)
        {
            // Sort the open list by the f score of each node
            openList.Sort((a, b) => a.getF().CompareTo(b.getF()));

            // Get the node with the lowest f score
            Node currentNode = openList[0];

            // If we've reached the end node, we're done
            if (currentNode.position == end)
            {
                shortestPath = new List<Vector2>();
                while (currentNode != null)
                {
                    shortestPath.Add(currentNode.position);
                    currentNode = currentNode.parent;
                }
                shortestPath.Reverse();
                break;
            }

            // Remove the current node from the open list and add it to the closed list
            openList.RemoveAt(0);
            closedList.Add(currentNode);

            // Loop through each neighbor of the current node
            foreach (Vector2 neighborPosition in GetNeighborPositions(currentNode.position))
            {
                // If the neighbor is already in the closed list, skip it
                if (closedList.Exists(n => n.position == neighborPosition))
                {
                    continue;
                }

                // Calculate the cost to move to the neighbor from the current node
                float tentativeGScore = currentNode.g + Vector2.Distance(currentNode.position, neighborPosition);

                // If the neighbor is not in the open list, add it
                Node neighborNode = openList.Find(n => n.position == neighborPosition);
                if (neighborNode == null)
                {
                    neighborNode = new Node(neighborPosition, currentNode, tentativeGScore, EstimateDistance(neighborPosition, end));
                    openList.Add(neighborNode);
                }
                else if (tentativeGScore < neighborNode.g)
                {
                    // If the neighbor is already in the open list and the new g score is lower, update it
                    neighborNode.parent = currentNode;
                    neighborNode.g = tentativeGScore;
                    neighborNode.f = neighborNode.g + neighborNode.h;
                }
            }
        }

        return shortestPath;
    }

    // Estimate the distance between two positions using a heuristic function
    private static float EstimateDistance(Vector2 start, Vector2 end)
    {
        return Vector2.Distance(start, end);
    }
    
    // Get the positions of all neighboring nodes
    private static List<Vector2> GetNeighborPositions(Vector2 position)
    {
        List<Vector2> neighbors = new List<Vector2>();
        // TODO: Implement code to get positions of all neighboring nodes
        return neighbors;
    }
}
