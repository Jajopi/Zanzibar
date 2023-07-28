using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Quest : MonoBehaviour
{
    string playerName;
    string task;
    List<int> points = new List<int>();

    Color32 transparentColor = new Color(1, 1, 1, 0);
    Color32 visibleColor = new Color(1, 1, 1, 1);

    public void SetPlayer(string _playerName)
    {
        playerName = _playerName;
    }

    public void SetTask(string _task)
    {
        task = _task;
    }

    public void SetPoints(List<int> _points)
    {
        points = _points;
    }

    public string GetTask() { return task; }
    public string GetPlayer() { return playerName; }
    public List<int> GetPoints() { return points; }

    void OnEnable()
    {
        SetTransparent();
    }

    public void Setup()
    {
        // Needs to be called after other setters
        SetTextRepresentation();
    }

    public void SetTransparent()
    {
        gameObject.GetComponent<TextMeshProUGUI>().color = transparentColor;
    }

    public void SetVisible()
    {
        gameObject.GetComponent<TextMeshProUGUI>().color = visibleColor;
    }

    void SetTextRepresentation()
    {
        string text;
        if (points.Count == 0)
        {
            text = task;
        }
        else
        {
            text = task + ": ";
            text += string.Join(", ", points.Select(x => x.ToString()).ToArray());
            //Debug.Log(text);
        }
        gameObject.GetComponent<TextMeshProUGUI>().text = text;
    }
}
