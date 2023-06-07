using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    int state;
    int movesLeft;
    int stepsLeft;
    Figure figure, targetFigure;
    Node targetNode;
    // select: figure --then-- targetFigure --then-- targetNode

    int movesPerPlayer = 3;

    void Start()
    {
        state = 0;
        /*  Meaning of states:
         *  0: "waitForFigureToChoose"
         *  1: "waitForPlaceToMove"
         *  2: "waitForPlaceToPush"
         *  3: "waitForPlaceToYeet"
         *  4: "waitForTimer"
         */
    }

    void Update()
    {
        if (state == 4)
        {
            movesLeft = 3;
            GoToState(0);
        }
    }

    List<Node> GetFreeNeighbours(Node node)
    {
        List<Node> neighbours = node.GetNeighbours().FindAll(
            a => !a.IsOccupied());
        return neighbours;
    }

    void GoToState(int _state)
    {
        //Debug.Log(state.ToString() + " " + _state.ToString());
        state = _state;
        if (state == 0)
        {
            UnSelect(figure);
            UnSelect(targetFigure);

            if (movesLeft <= 0) {
                movesLeft = movesPerPlayer;
                GoToState(4);
            }
            movesLeft--;
        }
        else if (state == 1)
        {
            UnSelect(targetFigure);
            Select(figure);

            if (stepsLeft <= 0) { GoToState(0); }
            stepsLeft--;
        }
        else if (state == 2)
        {
            Select(targetFigure);
        }
        else if (state == 3)
        {
            Select(targetFigure);
        }
    }

    public void OnNodeClick(Node node)
    {
        //Debug.Log(node.name + " " + state.ToString());
        if (state == 0)
        {
            figure = node.GetFigure();
            if (figure is null) { return; }
            stepsLeft = figure.GetSpeed();
            GoToState(1);
        }

        else if (state == 1)
        {
            if (node.GetFigure() == figure) { GoToState(0); }

            List<Node> neighbours = node.GetNeighbours();
            bool found = false;
            foreach (Node neighbour in neighbours)
            {
                if (neighbour.GetFigure() == figure)
                {
                    found = true; break;
                }
            }
            if (!found) { return; }

            if (node.GetFigure() is null)
            {
                figure.MoveToNode(node); GoToState(1);
            }
            else
            {
                targetFigure = node.GetFigure();

                if (targetFigure.GetOwner() == figure.GetOwner()) { return; }
                if (targetFigure.GetSpeed() < figure.GetSpeed()) { return; }
                else if (targetFigure.GetSpeed() == figure.GetSpeed())
                {
                    List<Node> freeNeighbours = GetFreeNeighbours(node);
                    if (freeNeighbours.Count > 0) { return; }
                    else if (freeNeighbours.Count == 0) { GoToState(3); }
                }
                else if (targetFigure.GetSpeed() > figure.GetSpeed())
                {
                    List<Node> freeNeighbours = GetFreeNeighbours(node);
                    if (freeNeighbours.Count == 0) { GoToState(3); }
                    else if (freeNeighbours.Count == 1) {
                        GoToState(2);
                        OnNodeClick(freeNeighbours[0]);
                    }
                    else if (freeNeighbours.Count > 1) { GoToState(2); }
                }
            }
        }

        else if (state == 2)
        {
            if (node.GetFigure() == targetFigure) { GoToState(0); }

            targetNode = node;
            if (targetNode.IsOccupied()) { return; }

            List<Node> neighbours = node.GetNeighbours();
            bool found = false;
            foreach (Node neighbour in neighbours)
            {
                if (neighbour.GetFigure() == targetFigure)
                {
                    found = true; break;
                }
            }
            if (!found) { return; }

            Node lastNode = targetFigure.GetNode();
            targetFigure.MoveToNode(targetNode);
            figure.MoveToNode(lastNode);
            GoToState(1);
        }

        else if (state == 3)
        {
            if (node.GetFigure() == targetFigure) { GoToState(0); }

            targetNode = node;
            if (targetNode.IsOccupied()) { return; }

            Node lastNode = targetFigure.GetNode();
            targetFigure.MoveToNode(targetNode);
            figure.MoveToNode(lastNode);
            GoToState(1);
        }
    }

    void Select(Figure fig)
    {
        if (fig is null) { return; }
        fig.ColorAsSelected(true);
    }

    void UnSelect(Figure fig)
    {
        if (fig is null) { return; }
        fig.ColorAsSelected(false);
    }
}
