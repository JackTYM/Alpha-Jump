using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class WinController : MonoBehaviour
{

    public Stopwatch levelStopwatch;
    public int jumpCount = 0;
    public int letterCount = 0;

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
            LeaderboardEntry entry = new LeaderboardEntry(){playerName = playerName, jumpCount = jumpCount, time = Mathf.Round(levelStopwatch.ElapsedMilliseconds/100)/10, letters=letterCount, difficulty=transform.parent.GetChild(4).GetComponent<DictionaryController>().modeIndex};

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

    // Update is called once per frame
    public void EndLevel()
    {
        transform.Find("Level Stats").GetComponent<TMPro.TextMeshProUGUI>().text = "Completed level in " + Mathf.Round(levelStopwatch.ElapsedMilliseconds/100)/10 + "s with " + jumpCount + " jumps";
        gameObject.SetActive(true);
    }
}
