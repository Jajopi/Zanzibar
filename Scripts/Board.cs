using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    Logic logic;
    int createdFigures = 0;
    public int figureCount = 5;
    public int playerCount = 3;
    float figureMargin = 1f;
    float figureBenchX = 4f, figureBenchY = 2f;

    public string mapFileName;

    public List<Node> nodes = new List<Node>();
    public List<Figure> figures = new List<Figure>();
    public string[] playerNames;
    public string[] playerColors;

    GameObject emptyLine;
    public GameObject emptyNode;
    public GameObject emptyFigure;

    int lineIndex = 0;

    Color32 edgeColor = Color.red;
    string defaultColorName = "ffffff";

    public float minutesPerDay = 2;

    void Start()
    {
        LoadPrefabs();

        logic = transform.GetComponent<Logic>();
        logic.SetPlayers(new List<string>(playerNames));

        if (mapFileName != "") { CreateFromFile(mapFileName); }

        //Camera camera = GetComponentInChildren<Camera>();
        //Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        //camera.orthographicSize *= transform.localScale.x;
        //camera.transform.LookAt(transform.position);

        PlaceAllFigures();
    }

    void LoadPrefabs()
    {
        emptyLine = new GameObject();
    }

    public void CreateFromFile(string fileName)
    {
        fileName = "Maps/" + fileName;
        TextAsset file = (TextAsset)Resources.Load(fileName);
        if (file is null) { throw new Exception("File not found: " + fileName); }
        CreateGraph(file.text);
    }


    Figure GetFigureByParams(string owner, int speed)
    {
        foreach (Figure figure in figures)
        {
            if (figure.GetOwner() == owner &&
                figure.GetSpeed() == speed) { return figure; }
        }
        throw new Exception("No figure with owner and speed: "
            + owner + " " + speed.ToString());
        //return null;
    }

    public List<Figure> GetAllFigures() { return figures; }
    public List<Node> GetAllNodes() { return nodes; }

    Node GetNodeByTitle(string title)
    {
        foreach (Node node in nodes)
        {
            if (node.name == title) { return node; }
        }
        throw new Exception("No node named: " + title);
        //return null;
    }

    public Node AddNode(string title, string resource, string location,
        string color, Pair<float, float> coordinates)
    {
        if (resource == "-") { resource = null; }
        if (location == "-") { location = null; }

        GameObject node = Instantiate(emptyNode, this.transform, false);
        node.name = title;
        node.AddComponent<Node>();
        node.GetComponent<Node>().Set(resource, location, color, coordinates);
        nodes.Add(node.GetComponent<Node>());

        return node.GetComponent<Node>();
    }

    public void PlaceFigure(string owner, int speed, Node node)
    {
        if (node is not null)
        {
            if (node.IsOccupied()) { return; }
        }

        string title = owner.ToString() + "-" + speed.ToString();
        GameObject figure = Instantiate(emptyFigure, this.transform, false);
        figure.name = title;
        figure.AddComponent<Figure>();
        figures.Add(figure.GetComponent<Figure>());

        if (node is null)
        {
            float yCoord = createdFigures % figureCount * figureMargin
                + figureBenchX;
            float xCoord = Mathf.Floor(createdFigures / figureCount)
                * figureMargin + figureBenchX;
            node = AddNode("underFigure" + xCoord.ToString() + yCoord.ToString(),
                "-", "-", "white", new Pair<float, float>(xCoord, yCoord));
        }

        figure.GetComponent<Figure>().Set(owner, speed, node);
        node.PlaceFigure(figure.GetComponent<Figure>());
        createdFigures++;
    }

    void PlaceAllFigures()
    {
        for (int i = 0; i < playerCount; i++)
        {
            for (int j = 0; j < figureCount; j++)
            {
                PlaceFigure(playerNames[i], j + 2, null);
            }
        }
    }

    public void CreateLine(Node nodeA, Node nodeB)
    {
        Vector3[] positions = new Vector3[2];
        Pair<float, float> p;
        p = nodeA.GetCoordinates();
        positions[0] = new Vector3(p.First, 1, p.Second);
        p = nodeB.GetCoordinates();
        positions[1] = new Vector3(p.First, 1, p.Second);

        string title = "Line" + lineIndex++.ToString();
        GameObject line = Instantiate(emptyLine, this.transform, false);
        line.name = title;
        line.AddComponent<LineRenderer>();
        LineRenderer renderer = line.GetComponent<LineRenderer>();

        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.startWidth = 0.1f * transform.localScale.x;
        renderer.endWidth = renderer.startWidth;
        renderer.startColor = edgeColor; renderer.endColor = edgeColor;

        renderer.useWorldSpace = false;
        renderer.positionCount = 2;
        renderer.SetPositions(positions);
    }

    public void AddEdge(string titleA, string titleB)
    {
        Node nodeA = GetNodeByTitle(titleA);
        if (nodeA is null) {
            throw new InvalidOperationException("No node titled: " + titleA);
        }
        Node nodeB = GetNodeByTitle(titleB);
        if (nodeB is null) {
            throw new InvalidOperationException("No node titled: " + titleB);
        }

        if (nodeA.Connect(nodeB)) { CreateLine(nodeA, nodeB); }
    }

    public void CreateGraph(string representation)
    {
        foreach (string line in representation.Split("\n")) {
            string[] args = line.Split(" ");
            for (int i = 0; i < args.Length; i++) {
                args[i] = args[i].Trim(); }

            if (args.Length == 0) { continue; }
            if (args[0].Length == 0) { continue; }
            if (args[0][0] == '#') { continue; }
            if (args[0] == "figureBench:")
            {
                figureBenchX = float.Parse(args[1]);
                figureBenchY = float.Parse(args[2]);
                continue;
            }
            if (args[0] == "figureCount:")
            {
                figureCount = Int32.Parse(args[1]);
                continue;
            }
            if (args[0] == "playerCount:")
            {
                playerCount = Int32.Parse(args[1]);
                continue;
            }

            if (args.Length == 2)
            {
                AddEdge(args[0], args[1]);
            }
            else if (args.Length == 5)
            {
                AddNode(args[0], args[1], args[2], defaultColorName,
                    new Pair<float, float>(
                        - float.Parse(args[4]),
                        float.Parse(args[3])));
            }
            else if (args.Length == 6)
            {
                AddNode(args[0], args[1], args[2], args[3],
                    new Pair<float, float>(
                        - float.Parse(args[5]),
                        float.Parse(args[4])));
            }
            else if (args.Length == 7)
            {
                AddNode(args[0], args[1], args[2], args[3],
                    new Pair<float, float>(
                        - float.Parse(args[5]),
                        float.Parse(args[4])));
                AddEdge(args[0], args[6]);
            }
        }
    }

    public Color32 TranslateColor(string colorString)
    {
        for (int i = 0; i < playerNames.Length; i++)
        {
            if (playerNames[i] == colorString)
            {
                colorString = playerColors[i]; break;
            }
        }

        if (colorString == "red") colorString = "ff0000";
        else if (colorString == "blue") colorString = "0000ff";
        else if (colorString == "green") colorString = "00ff00";
        else if (colorString == "white") colorString = "ffffff";
        else if (colorString == "black") colorString = "000000";
        else if (colorString == "purple") colorString = "ff00ff";
        else if (colorString == "yellow") colorString = "ffff00";
        else if (colorString == "orange") colorString = "ff8000";

        string r, g, b;
        r = colorString.Substring(0, 2);
        g = colorString.Substring(2, 2);
        b = colorString.Substring(4, 2);
        return new Color32((byte)Convert.ToInt32(r, 16),
                            (byte)Convert.ToInt32(g, 16),
                            (byte)Convert.ToInt32(b, 16), 0);
    }

    public void OnNodeClick(Node node)
    {
        logic.OnNodeClick(node);
    }
}
