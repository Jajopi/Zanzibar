using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Random = UnityEngine.Random;

public class Quests : MonoBehaviour
{
    Logic logic;
    Board board;

    List<string> tasks = new List<string>();
    List<List<int>> points = new List<List<int>>();
    List<Quest> quests = new List<Quest>();
    int maxQuestsPerPlayer = 3;
    int questIndex = 0;
    Quest visibleQuest;
    List<Node> selectedNodes = new List<Node>();

    Quest noQuest;
    string emptyQuestString = "Prázdna úloha";

    public GameObject questPrefab;

    public string questFileName;

    TextMeshProUGUI remainingTimeText;

    void Start()
    {
        logic = GameObject.Find("Board").GetComponent<Logic>();
        board = GameObject.Find("Board").GetComponent<Board>();
        noQuest = CreateQuest(new List<int>(), emptyQuestString, "");
        LoadQuestStrings(questFileName);
        remainingTimeText = transform.Find("RemainingTimeText"
            ).gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UpdateQuests();
        UpdateTime();
    }

    void UpdateQuests()
    {
        if (visibleQuest is not null)
        {
            visibleQuest.SetTransparent();
            UnselectNodes();
            visibleQuest = null;
        }

        bool activate = Input.GetKeyDown(KeyCode.Return);
        bool replace = Input.GetKeyDown(KeyCode.N) &&
            (Input.GetKey(KeyCode.RightShift) ||
            Input.GetKey(KeyCode.LeftShift)) &&
            !activate;

        int number = -1;
        if (Input.GetKey(KeyCode.Alpha1)) { number = 1; }
        else if (Input.GetKey(KeyCode.Alpha2)) { number = 2; }
        else if (Input.GetKey(KeyCode.Alpha3)) { number = 3; }
        if (number < 0)
        {
            if (replace) {
                TryAddNewQuest();
            }
            return;
        }

        visibleQuest = null;
        int index = 0;
        while (index < quests.Count)
        {
            if (quests[index].GetPlayer() == GetActualPlayer())
            {
                number--;
                if (number == 0)
                {
                    visibleQuest = quests[index];
                    break;
                }
            }
            index++;
        }
        if (visibleQuest is not null)
        {
            if (activate && !logic.GetJumpingTurn())
            {
                logic.ActivateQuest(visibleQuest.GetPoints(),
                        visibleQuest.GetTask());
                quests.Remove(visibleQuest);
                Destroy(visibleQuest.gameObject);
                visibleQuest = null;
            }
            else if (replace)
            {
                Destroy(quests[index].gameObject);
                quests[index] = CreateRandomQuest(GetActualPlayer());
                visibleQuest = quests[index];
            }

            if (visibleQuest is not null)
            {
                visibleQuest.SetVisible();
                SelectNodes();
            }

        }
        else
        {
            visibleQuest = noQuest;
            visibleQuest.SetVisible();
        }
    }

    void TryAddNewQuest()
    {
        string actualPlayer = GetActualPlayer();
        int count = 0;
        foreach (Quest quest in quests)
        {
            if (quest.GetPlayer() == actualPlayer)
            {
                count++;
                if (count >= maxQuestsPerPlayer)
                {
                    return;
                }
            }
        }

        quests.Add(CreateRandomQuest(actualPlayer));
    }

    public void AddQuestInsecure(Quest quest)
    {
        quests.Add(quest);
    }

    public void DestroyAllQuests()
    {
        while (quests.Count > 0)
        {
            Quest quest = quests[0];
            quests.RemoveAt(0);
            Destroy(quest.gameObject);
        }
    }

    void SelectNodes()
    {
        foreach (Node node in board.GetAllNodes())
        {
            if (node.GetResource() == visibleQuest.GetTask() ||
                node.GetLocation() == visibleQuest.GetTask())
            {
                node.SetSelected();
                selectedNodes.Add(node);
            }
        }
    }

    void UnselectNodes()
    {
        foreach (Node node in selectedNodes)
        {
            node.SetUnselected();
        }
        selectedNodes.Clear();
    }

    string GetActualPlayer()
    {
        return logic.GetActualPlayer();
    }

    public void LoadQuestStrings(string fileName)
    {
        fileName = "Quests/" + fileName;
        TextAsset file = (TextAsset)Resources.Load(fileName);
        if (file is null) { throw new Exception("File not found: " + fileName); }
        ParseQuestStrings(file.text);
    }

    void ParseQuestStrings(string text)
    {
        foreach (string _line in text.Split("\n"))
        {
            string line = _line.Trim();
            if (line.Length == 0) { continue; }
            string[] args = line.Split(":");

            tasks.Add(args[0]);
            points.Add(new List<int>());
            string[] pnts = args[1].Split(" ");
            for (int i = 0; i < pnts.Length; i++)
            {
                pnts[i] = pnts[i].Trim();
            }

            foreach (string pnt in pnts)
            {
                if (pnt.Length == 0) { continue; }
                points[points.Count - 1].Add(Int32.Parse(pnt));
            }
        }
    }

    public Quest CreateQuest(List<int> _points, string task, string playerName)
    {
        GameObject questObject = Instantiate(questPrefab, this.transform, false);
        questObject.name = "Quest-" + playerName + "-" + questIndex++.ToString();
        questObject.AddComponent<Quest>();

        Quest quest = questObject.GetComponent<Quest>();
        quest.SetPlayer(playerName);
        quest.SetTask(task);
        quest.SetPoints(_points);
        quest.Setup();

        return quest;
    }

    public Quest CreateRandomQuest(string _playerName)
    {
        int index = Random.Range(0, tasks.Count - 1);

        while (!TestQuestNotDuplicate(_playerName, tasks[index]))
        {
            index = (index + 1) % tasks.Count;
        }

        Quest newQuest = CreateQuest(points[index], tasks[index], _playerName);
        return newQuest;
    }

    bool TestQuestNotDuplicate(string player, string task)
    {
        foreach (Quest q in quests)
        {
            if (q.GetTask() == task &&
                q.GetPlayer() == player)
            {
                return false;
            }
        }
        return true;
    }

    void UpdateTime()
    {
        float remainingTime = logic.GetRemainingTime();
        remainingTimeText.text = logic.GetActualPlayer().Replace("_", " ") + ": \n"
            + Mathf.Round(remainingTime);
    }

    public List<Quest> GetAllQuests()
    {
        return quests;
    }
}
