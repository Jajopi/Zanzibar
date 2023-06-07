using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    Board parent;
    List<Node> neighbours = new List<Node>();

    string resource;
    Color32 color;
    bool occupied = false;
    Figure figure = null;

    Material sprite;

    public void Set(string _resource, string colorString, Pair<float, float> coordinates)
    {
        parent = transform.parent.GetComponent<Board>();
        //transform.localScale = new Vector3(
        //    1f / parent.transform.localScale.x,
        //    1f / parent.transform.localScale.y,
        //    1f / parent.transform.localScale.z);
        SetResource(_resource);
        SetCoordinates(coordinates);
        SetColor(colorString);
    }


    public bool IsOccupied() { return occupied; }

    public Figure GetFigure()
    {
        if (!IsOccupied()) return null;
        return figure;
    }

    public List<Node> GetNeighbours() { return neighbours; }
    public Board GetParent() { return  parent; }
    public string GetResource() { return resource; }
    public string GetTitle() { return this.name; }

    public Pair<float, float> GetCoordinates() {
        return new Pair<float, float>(
            transform.localPosition.x, transform.localPosition.z); }


    public void PlaceFigure(Figure _figure)
    {
        if (occupied) throw new Exception("Already occupied: " + this.name);
        figure = _figure;
        occupied = true;

        if (figure.GetNode() != this)
        {
            figure.GetNode().RemoveFigure();
            figure.SetNode(this);
        }
    }

    public void RemoveFigure()
    {
        figure = null; occupied = false;
    }

    public bool Connect(Node node)
    {
        if (node is null)
            throw new InvalidOperationException("Connecting nothing to " + this.name);
        if (!neighbours.Contains(node))
        {
            neighbours.Add(node);
            node.Connect(this);
            return true;
        }
        return false;
    }

    public void SetResource(string _resource)
    {
        resource = _resource;
        //sprite = parent.GetSprite(resource);
        //gameObject.GetComponent<MeshRenderer>().material = sprite;
    }

    public void SetColor(string colorString)
    {
        color = parent.TranslateColor(colorString);

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Color32[] colors = new Color32[mesh.vertices.Length];
        for (int i = 0; i < colors.Length; i++){ colors[i] = color; }
        mesh.colors32 = colors;
    }

    public void SetCoordinates(Pair<float, float> coordinates)
    {
        transform.localPosition = new Vector3(
            coordinates.First, 2, coordinates.Second);
    }


    public void OnMouseDown()
    {
        parent.OnNodeClick(this);
    }
}
