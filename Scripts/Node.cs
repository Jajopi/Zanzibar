using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    Board parent;
    List<Node> neighbours = new List<Node>();

    string resource, location;
    Color32 color;
    bool occupied = false;
    Figure figure = null;

    Material sprite;

    float resizeSpeed = 4f;
    Vector3 startingScale;
    float targetScaleMultiply = 1f;

    public void Set(string _resource, string _location, string colorString,
        Pair<float, float> coordinates)
    {
        parent = transform.parent.GetComponent<Board>();
        //transform.localScale = new Vector3(
        //    1f / parent.transform.localScale.x,
        //    1f / parent.transform.localScale.y,
        //    1f / parent.transform.localScale.z);
        SetResource(_resource);
        SetLocation(_location);
        SetCoordinates(coordinates);
        SetColor(colorString);
    }

    void Start()
    {
        startingScale = transform.localScale;
    }

    void Update()
    {
        ResizeSelected();
    }

    void ResizeSelected()
    {
        if (transform.localScale.x != targetScaleMultiply)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale, startingScale * targetScaleMultiply,
                resizeSpeed * Time.deltaTime);
        }
    }

    public bool IsOccupied() { return occupied; }

    public bool IsUnconnected() { return (neighbours.Count == 0); }

    public Figure GetFigure()
    {
        if (!IsOccupied()) return null;
        return figure;
    }

    public List<Node> GetNeighbours() { return neighbours; }
    public Board GetParent() { return  parent; }
    public string GetResource() { return resource; }
    public string GetLocation() { return location; }
    public string GetTitle() { return this.name; }

    public Pair<float, float> GetCoordinates() {
        return new Pair<float, float>(
            transform.localPosition.x, transform.localPosition.z); }


    public void PlaceFigure(Figure _figure)
    {
        if (occupied && figure != _figure) throw new Exception("Already occupied: " + this.name);
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
    }

    public void SetLocation(string _location)
    {
        location = _location;
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

    public void SetSelected()
    {
        targetScaleMultiply = 1.5f;
    }

    public void SetUnselected()
    {
        targetScaleMultiply = 1f;
    }

    public void OnMouseDown()
    {
        parent.OnNodeClick(this);
    }
}
