using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    int state;
    int stepsLeft;
    List<string> playerNames;
    int actualPlayer;
    Figure figure, targetFigure, selectedFigure;
    List<Figure> movedFigures;
    Node targetNode;
    // select: selectedFigure --then-- targetFigure --then-- targetNode

    int movesPerPlayer = 2;

    void Start()
    {
        state = 0;
        /*  Meaning of states:
         *  0: "waitForFigureToChoose" -> 1
         *  1: "waitForPlaceToMove" -> 1 / 2 / 3
         *  2: "waitForPlaceToPush" -> 1 / 0
         *  3: "waitForPlaceToYeet" -> 1 / 0
         *  4: "waitForTimer"
         */

        movedFigures = new List<Figure>();
    }

    void Update()
    {
        if (state == 4)
        {
            GoToState(0);
        }
    }

    public void SetPlayers(List<string> names)
    {
        playerNames = names;
        actualPlayer = 0;
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
            UnSelect(selectedFigure);
            UnSelect(targetFigure);

            //Debug.Log(movesPerPlayer.ToString() + " " + movedFigures.Count.ToString());
            if (movesPerPlayer == movedFigures.Count) {
                GoToState(4);
            }
        }
        else if (state == 1)
        {
            UnSelect(targetFigure);
            Select(selectedFigure);

            if (stepsLeft == 0) { GoToState(0); }
            if (stepsLeft < 0) { throw new Exception("Somehow managed to have < 0 steps left."); }
        }
        else if (state == 2)
        {
            Select(targetFigure);
        }
        else if (state == 3)
        {
            Select(targetFigure);
        }
        else if (state == 4)
        {
            movedFigures.Clear();

            actualPlayer++;
            if (actualPlayer >= playerNames.Count)
            {
                actualPlayer = 0;
            }
        }
    }

    public void OnNodeClick(Node node)
    {
        //Debug.Log(node.name + " " + state.ToString());
        if (state == 0)
        {
            figure = node.GetFigure();
            if (figure is null) { return; }
            //Debug.Log(figure.GetOwner() + " " + playerNames[actualPlayer]);
            if (figure.GetOwner() != playerNames[actualPlayer]) { return; }
            if (movedFigures.Contains(figure)) { return; }
            stepsLeft = figure.GetSpeed();
            selectedFigure = figure;
            GoToState(1);
        }

        else if (state == 1)
        {
            if (node.GetFigure() == selectedFigure) { GoToState(0); }

            List<Node> neighbours = node.GetNeighbours();
            bool found = false;
            foreach (Node neighbour in neighbours)
            {
                if (neighbour.GetFigure() == selectedFigure)
                {
                    found = true; break;
                }
            }
            if (!found && !selectedFigure.GetNode().IsUnconnected()) { return; }

            if (node.GetFigure() is null)
            {
                if (selectedFigure.GetNode().IsUnconnected())
                {
                    stepsLeft = 1;
                }
                MoveFigure(selectedFigure, node);
                GoToState(1);
            }
            else
            {
                if (selectedFigure.GetNode().IsUnconnected()) { return; }
                figure = node.GetFigure();

                if (figure.GetOwner() == selectedFigure.GetOwner()) { return; }
                if (figure.GetSpeed() < selectedFigure.GetSpeed()) { return; }
                else if (figure.GetSpeed() == selectedFigure.GetSpeed())
                {
                    List<Node> freeNeighbours = GetFreeNeighbours(node);
                    if (freeNeighbours.Count > 0) { return; }
                    else if (freeNeighbours.Count == 0) {
                        targetFigure = figure;
                        GoToState(3);
                    }
                }
                else if (figure.GetSpeed() > selectedFigure.GetSpeed())
                {
                    List<Node> freeNeighbours = GetFreeNeighbours(node);
                    if (freeNeighbours.Count == 0) {
                        targetFigure = figure;
                        GoToState(3);
                    }
                    else if (freeNeighbours.Count == 1) {
                        targetFigure = figure;
                        GoToState(2);
                        OnNodeClick(freeNeighbours[0]);
                    }
                    else if (freeNeighbours.Count > 1) {
                        targetFigure = figure;
                        GoToState(2);
                    }
                }
            }
        }

        else if (state == 2)
        {
            if (node.GetFigure() == targetFigure) { GoToState(1); }

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
            MoveFigure(selectedFigure, lastNode);
            GoToState(1);
        }

        else if (state == 3)
        {
            if (node.GetFigure() == targetFigure) { GoToState(1); }

            targetNode = node;
            if (targetNode.IsOccupied()) { return; }

            Node lastNode = targetFigure.GetNode();
            targetFigure.MoveToNode(targetNode);
            MoveFigure(selectedFigure, lastNode);
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

    void MoveFigure(Figure figure, Node node)
    {
        figure.MoveToNode(node);
        if (!movedFigures.Contains(figure))
        {
            movedFigures.Add(figure);
        }
        stepsLeft--;
    }
}
