using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class WinController : MonoBehaviour
{

    public Stopwatch levelStopwatch;
    public float timeForLevel = 0f;
    public int jumpCount = 0;
    public int letterCount = 0;
    GameObject pauseScreen;

    // Start is called before the first frame update
    void Start()
    {
        levelStopwatch = Stopwatch.StartNew();

        UIHandler uiHandler = null;

        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (t.name == "UI") {
                uiHandler = t.gameObject.GetComponent<UIHandler>();
            }
        }

        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{
            string playerName = transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMPro.TMP_InputField>().text;
            if (playerName == "") {
                playerName = "Anonymous";
            }
            LeaderboardEntry entry = new LeaderboardEntry(){playerName = playerName, jumpCount = jumpCount, time = timeForLevel, letters=letterCount, difficulty=transform.parent.GetChild(4).GetComponent<DictionaryController>().modeIndex};

            UnityEngine.Debug.Log(letterCount);

            transform.parent.GetComponent<LeaderboardController>().addScore(entry, int.Parse(SceneManager.GetActiveScene().name.Split(" ")[1]));
        });
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            SceneManager.LoadScene("Menu");
            gameObject.SetActive(false);
            uiHandler.gameObject.SetActive(false);
        });
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{Application.Quit();});
    }
    
    public void EndLevel()
    {
        timeForLevel = Mathf.Round(levelStopwatch.ElapsedMilliseconds/100)/10;
        transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().text = "You Win!";
        transform.Find("Level Stats").GetComponent<TMPro.TextMeshProUGUI>().text = "Completed level in " + timeForLevel + "s with " + jumpCount + " jumps";
        gameObject.SetActive(true);
    }

    public void LoseLevel(float timeForLevel2, float jumpCount2) {
        transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().text = "You Lose!";
        transform.Find("Level Stats").GetComponent<TMPro.TextMeshProUGUI>().text = "The other player completed the level in " + timeForLevel2 + "s with " + jumpCount2 + " jumps";
        gameObject.SetActive(true);
    }
}
