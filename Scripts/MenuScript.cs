using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuScript : MonoBehaviour
{
    int MAX_PLAYERS = 6;
    int playerFields = 0;

    float fieldHeight = 70f;

    public GameObject inputFieldPrefab;

    void Start()
    {
        AddPlayer();
    }

    public void NewPlayerButton()
    {
        if (playerFields >= MAX_PLAYERS) { return; }
        AddPlayer();
    }

    void AddPlayer()
    {
        GameObject newField = Instantiate(inputFieldPrefab, transform);
        newField.GetComponent<RectTransform>().anchoredPosition = new Vector3(
            0, -1 * playerFields++ * fieldHeight, 0);
        newField.SetActive(true);
    }

    List<string> GetPlayerNames()
    {
        List<string> playerNames = new List<string>();
        foreach (GameObject field in GameObject.FindGameObjectsWithTag("InputField"))
        {
            string playerName = field.GetComponent<TextMeshProUGUI>().text.Trim();
            //Debug.Log(playerName);
            //Debug.Log(playerName.Length);
            if (playerName.Length > 1 ) { playerNames.Add(playerName); }
        }
        return playerNames;
    }

    public void NewGameButton()
    {
        List<string> playerNames = GetPlayerNames();
        if (playerNames.Count == 0) { return; }

        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetString("newGameOrContinue", "newGame");
        PlayerPrefs.SetInt("playerCount", playerNames.Count);
        PlayerPrefs.SetInt("figuresPerPlayer", 10 - playerNames.Count);
        for (int i = 0; i < playerNames.Count; i++)
        {
            string number = i.ToString();
            PlayerPrefs.SetString("playerName" + number, playerNames[i]);
        }

        SceneManager.LoadScene("GameScene");
    }

    public void ContinueButton()
    {
        if (!PlayerPrefs.HasKey("timestamp")) { return; }
        PlayerPrefs.SetString("newGameOrContinue", "continue");

        SceneManager.LoadScene("GameScene");
    }
}
