using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figure : MonoBehaviour
{
    Board parent;
    Node node;

    int speed;
    string owner;
    Color32 color;

    float moveSpeed = 4f;
    Queue<Vector3> targetPositions = new Queue<Vector3>();
    Vector3 targetPosition;

    float floatAbove = 1.5f;

    float resizeSpeed = 4f;
    Vector3 startingScale;
    float targetScaleMultiply = 1f;

    public void Set(string _owner, int _speed, Node _node, string colorString)
    {
        parent = transform.parent.GetComponent<Board>();
        SetOwner(_owner);
        SetSpeed(_speed);
        SetColor(colorString);
        SetNode(_node); Place();

        startingScale = transform.localScale;

    }

    void SetOwner(string _owner) { owner = _owner; }
    void SetSpeed(int _speed) { speed = _speed; }

    void SetColor(string colorString)
    {
        color = parent.TranslateColor(colorString);

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Color32[] colors = new Color32[mesh.vertices.Length];
        for (int i = 0; i < colors.Length; i++) { colors[i] = color; }
        mesh.colors32 = colors;
    }

    public Node GetNode() { return node; }
    public int GetSpeed() { return speed; }
    public string GetOwner() { return owner; }

    public void SetNode(Node _node)
    {
        node = _node;
        Replace();
    }

    public void Place()
    {
        Pair<float, float> coordinates = node.GetCoordinates();
        transform.localPosition = new Vector3(
            coordinates.First,
            transform.localScale.y + 2,
            coordinates.Second);
        targetPosition = transform.localPosition;
    }

    public void Replace()
    {
        Pair<float, float> coordinates = node.GetCoordinates();
        targetPositions.Enqueue(new Vector3(
            coordinates.First,
            startingScale.y + floatAbove, // is rewritten at Update
            coordinates.Second));
    }

    void Update()
    {

        if (targetPosition != transform.localPosition)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition, targetPosition,
                moveSpeed * Time.deltaTime);
        }
        else if (targetPositions.Count > 0)
        {
            targetPosition = targetPositions.Dequeue();
        }

        if (transform.localScale.x != targetScaleMultiply)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale, startingScale * targetScaleMultiply,
                resizeSpeed * Time.deltaTime);
        }

        targetPosition.y = targetScaleMultiply * startingScale.y + floatAbove;

    }

    public void MoveToNode(Node nodeTo)
    {
        node.RemoveFigure();
        nodeTo.PlaceFigure(this);
    }

    public void ColorAsSelected(bool selected)
    {
        if (selected)
        {
            targetScaleMultiply = 1.5f;
        }
        else
        {
            targetScaleMultiply = 1f;
        }
    }

    void OnMouseDown()
    {
        node.OnMouseDown();
    }
}
