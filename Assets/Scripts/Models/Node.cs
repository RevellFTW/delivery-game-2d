using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2 position;
    public Node parent;
    public float g;
    public float h;
    public float f;

    public Node(Vector2 position, Node parent, float g, float h)
    {
        this.position = position;
        this.parent = parent;
        this.g = g;
        this.h = h;
    }

    public float getF()
    {
        return g + h;
    }
    
}
