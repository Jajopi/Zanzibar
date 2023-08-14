using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Logic : MonoBehaviour
{
    Board board;

    int state;
    int stepsLeft;
    List<string> playerNames;
    List<int> playerPoints;
    int actualPlayer;
    Figure figure, targetFigure, selectedFigure;
    List<Figure> movedFigures;
    Node targetNode;
    // select: selectedFigure --then-- targetFigure --then-- targetNode

    public int movesPerPlayer = 3;
    bool jumpingTurn = false;

    public GameObject scoreBoard;
    public int defaultPoints = 10;

    float remainingTime;
    public float turnDurationSeconds = 120f;

    /*
     * Delete + O + C - maze ulozené data (netreba robit)
     * Oba shifty + Delete - preskakuje kolo (dava cas 1s do konca)
     * L + O + C - nacitava posledne ulozene udaje (koniec miniuleho kola)
     */

    void Start()
    {
        state = 0;
        /*  Meaning of states:
         *  0: "waitForFigureToChoose" -> 1
         *  1: "waitForPlaceToMove" -> 0 / 1 / 2 / 3
         *  2: "waitForPlaceToPush" -> 1 / 0
         *  3: "waitForPlaceToYeet" -> 1 / 0
         *  4: "waitForTimer"
         */

        movedFigures = new List<Figure>();

        board = GameObject.Find("Board").GetComponent<Board>();

        UpdatePoints();

        remainingTime = turnDurationSeconds;
    }

    void Update()
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime < 0)
        {
            remainingTime += turnDurationSeconds;
            PassNextTurn();
        }

        CheckSavesClearLoad();

        if (Input.GetKey(KeyCode.LeftShift) &&
            Input.GetKey(KeyCode.RightShift) &&
            Input.GetKeyDown(KeyCode.Delete))
        {
            remainingTime = 1f;
        }
    }

    public float GetRemainingTime() { return remainingTime; }

    public void SetPlayers(List<string> names)
    {
        playerNames = names;
        playerPoints = new List<int>(new int[playerNames.Count]);
        for (int i = 0; i < playerNames.Count; i++)
        {
            playerPoints[i] = defaultPoints;
        }

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
        state = _state;
        if (state == 0)
        {
            UnSelect(selectedFigure);
            UnSelect(targetFigure);
            if (movesPerPlayer == movedFigures.Count)
            {
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
            // Do nothing and wait
        }
    }

    void PassNextTurn()
    {
        SaveState();

        movedFigures.Clear();
        jumpingTurn = false;

        actualPlayer++;
        if (actualPlayer >= playerNames.Count)
        {
            actualPlayer = 0;
        }

        GoToState(0);
    }

    public void OnNodeClick(Node node)
    {
        if (state == 0)
        {
            figure = node.GetFigure();
            if (figure is null) { return; }
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
            if (!found && !selectedFigure.GetNode().IsUnconnected() &&
                !node.IsUnconnected()) { return; }

            if (node.GetFigure() is null)
            {
                if (selectedFigure.GetNode().IsUnconnected()
                    || node.IsUnconnected())
                {
                    stepsLeft = 1;
                    jumpingTurn = true;
                }
                MoveFigure(selectedFigure, node);
                GoToState(1);
            }
            else
            {
                if (selectedFigure.GetNode().IsUnconnected() ||
                    node.IsUnconnected()) { return; }
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
            if (targetNode.IsOccupied() || targetNode.IsUnconnected()) { return; }

            Node lastNode = targetFigure.GetNode();
            targetFigure.MoveToNode(targetNode);
            MoveFigure(selectedFigure, lastNode);
            GoToState(1);
        }
    }

    public void ActivateQuest(List<int> points, string task)
    {
        List<Figure> figures = board.GetAllFigures();

        int count = 0;
        foreach (Figure figure in figures)
        {
            if (figure.GetOwner() == playerNames[actualPlayer])
            {
                if (task == figure.GetNode().GetResource())
                {
                    count++;
                }
                else if (task == figure.GetNode().GetLocation())
                {
                    count++;
                }
            }
        }

        playerPoints[actualPlayer] += points[Mathf.Min(count, points.Count - 1)];
        if (playerPoints[actualPlayer] < 0)
        {
            playerPoints[actualPlayer] = 0;
        }
        UpdatePoints();
    }

    void UpdatePoints()
    {
        string spaces = "        ";
        string text = "";
        for (int i = 0; i < playerNames.Count; i++)
        {
            text += playerNames[i].Replace("_", " ") +
                ": " + playerPoints[i].ToString() + spaces;
        }
        scoreBoard.GetComponent<TextMeshProUGUI>().text = text;
    }

    void Select(Figure fig)
    {
        if (fig is null) { return; }
        fig.MarkAsSelected(true);
    }

    void UnSelect(Figure fig)
    {
        if (fig is null) { return; }
        fig.MarkAsSelected(false);
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

    public string GetActualPlayer()
    {
        return playerNames[actualPlayer];
    }

    public bool GetJumpingTurn()
    {
        return jumpingTurn;
    }


    void CheckSavesClearLoad()
    {
        if (Input.GetKey(KeyCode.Delete) &&
            Input.GetKey(KeyCode.O) &&
            Input.GetKey(KeyCode.C))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
        if (Input.GetKey(KeyCode.L) &&
            Input.GetKey(KeyCode.O) &&
            Input.GetKeyDown(KeyCode.C))
        {
            if (PlayerPrefs.HasKey("timestamp"))
            {
                LoadState();
            }
        }
    }

    void SaveState()
    {
        PlayerPrefs.SetFloat("timestamp", Time.time);
        PlayerPrefs.SetInt("playerCount", playerNames.Count);
        PlayerPrefs.SetInt("actualPlayer", actualPlayer);
        for (int i = 0; i < playerNames.Count; i++)
        {
            string number = i.ToString();
            PlayerPrefs.SetString("playerName" + number, playerNames[i]);
            PlayerPrefs.SetInt("playerPoints" + number, playerPoints[i]);
        }

        string figures = "";
        foreach (Figure figure in board.GetAllFigures())
        {
            PlayerPrefs.SetString("figure" + figure.name,
                figure.GetNode().name);
            if (figures.Length > 0) { figures += ";"; }
            figures += figure.name;
        }

        figures = "";
        foreach (Figure figure in movedFigures)
        {
            if (figures.Length > 0) { figures += ";"; }
            figures += figure.name;
        }
        PlayerPrefs.SetString("movedFigureNames", figures);

        string quests = "";
        foreach (Quest quest in GameObject.Find("Canvas").GetComponent<Quests>(
            ).GetAllQuests())
        {
            if (quests.Length > 0) { quests += ";"; }
            quests += quest.GetPlayer() + "!" +
                quest.GetTask() + "!" +
                string.Join(",", quest.GetPoints().ToArray());
        }
        PlayerPrefs.SetString("activeQuests", quests);

        int jumpingTurnInt = 0;
        if (GetJumpingTurn()) { jumpingTurnInt = 1; }
        PlayerPrefs.SetInt("jumpingTurn", jumpingTurnInt);

        PlayerPrefs.Save();
    }

    void LoadState()
    {
        int playerCount = PlayerPrefs.GetInt("playerCount");
        actualPlayer = PlayerPrefs.GetInt("actualPlayer");
        remainingTime = turnDurationSeconds;

        playerNames = new List<string>();
        playerPoints = new List<int>();
        for (int i = 0; i < playerCount; i++)
        {
            string number = i.ToString();
            playerNames.Add(PlayerPrefs.GetString("playerName" + number));
            playerPoints.Add(PlayerPrefs.GetInt("playerName" + number));
        }

        string figures = PlayerPrefs.GetString("figureNames");
        foreach (string figureName in figures.Split(";"))
        {
            GameObject.Find(PlayerPrefs.GetString("figure" + figureName)
                    ).GetComponent<Node>().PlaceFigure(
                GameObject.Find(figureName).GetComponent<Figure>()
                );
        }

        movedFigures = new List<Figure>();
        figures = PlayerPrefs.GetString("movedFigureNames");
        //Debug.Log(figures.Length);
        if (figures.Length > 0)
        {
            foreach (string figureName in figures.Split(";"))
            {
                movedFigures.Add(
                    GameObject.Find(figureName).GetComponent<Figure>()
                    );
            }
        }
        if (movesPerPlayer == movedFigures.Count)
        {
            GoToState(4);
        }

        Quests quests = GameObject.Find("Canvas").GetComponent<Quests>();

        quests.DestroyAllQuests();
        string questsString = PlayerPrefs.GetString("activeQuests");
        if (questsString.Length > 0)
        {
            foreach (string questString in questsString.Split(";"))
            {
                string[] splitString = questString.Split("!");
                List<int> points = new List<int>();
                foreach (string p in splitString[2].Split(","))
                {
                    points.Add(int.Parse(p));
                }
                quests.AddQuestInsecure(quests.CreateQuest(
                    points, splitString[1], splitString[0]));
            }
        }

        if (PlayerPrefs.GetInt("jumpingTurn") > 1)
        {
            jumpingTurn = true;
        }
    }
}
