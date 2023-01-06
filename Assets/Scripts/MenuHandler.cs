using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class MenuHandler : MonoBehaviour
{
    public bool onWeb = false;

    int leaderboardLevel = 1;
    int difficulty = 0;
    GameObject winScreen = null;
    DictionaryController dictController = null;
    UIHandler uiHandler = null;

    // Start is called before the first frame update
    void Start()
    {
        loadDontDestroys();

        //Modes
        transform.GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            dictController.modeIndex++;
            if (dictController.modeIndex == 3) {
                dictController.modeIndex = 0;
            }

            switch (dictController.modeIndex) {
                case 0:
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Easy Mode";
                    break;
                case 1:
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Hard Mode";
                    break;
                case 2:
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Glitch Mode";
                    break;
            }
        });

        //Main Menu
        transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate{
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        });
        transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            SceneManager.LoadScene("Tutorial Map");
            winScreen.GetComponent<WinController>().levelStopwatch = Stopwatch.StartNew();
            winScreen.GetComponent<WinController>().jumpCount = 0;
            winScreen.GetComponent<WinController>().letterCount = 0;
            uiHandler.gameObject.SetActive(true);
        });
        transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            transform.GetChild(0).gameObject.SetActive(false);
            if (onWeb) {
                transform.GetChild(3).gameObject.SetActive(true);
            } else {
                transform.GetChild(2).gameObject.SetActive(true);
                updateLeaderboard();
            }
        });
        transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{Application.Quit();});

        //Levels
        transform.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            SceneManager.LoadScene("Map 1");
            winScreen.GetComponent<WinController>().levelStopwatch = Stopwatch.StartNew();
            winScreen.GetComponent<WinController>().jumpCount = 0;
            winScreen.GetComponent<WinController>().letterCount = 0;
            uiHandler.gameObject.SetActive(true);
        });
        transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            SceneManager.LoadScene("Map 2");
            winScreen.GetComponent<WinController>().levelStopwatch = Stopwatch.StartNew();
            winScreen.GetComponent<WinController>().jumpCount = 0;
            winScreen.GetComponent<WinController>().letterCount = 0;
            uiHandler.gameObject.SetActive(true);
        });
        transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            SceneManager.LoadScene("Map 3");
            winScreen.GetComponent<WinController>().levelStopwatch = Stopwatch.StartNew();
            winScreen.GetComponent<WinController>().jumpCount = 0;
            winScreen.GetComponent<WinController>().letterCount = 0;
            uiHandler.gameObject.SetActive(true);
        });
        transform.GetChild(1).GetChild(4).GetComponent<Button>().onClick.AddListener(delegate{
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
        });

        //Leaderboard
        transform.GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{
            leaderboardLevel = 1;
            updateLeaderboard();
        });
        transform.GetChild(2).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            leaderboardLevel = 2;
            updateLeaderboard();
        });
        transform.GetChild(2).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{
            leaderboardLevel = 3;
            updateLeaderboard();
        });
        transform.GetChild(2).GetChild(8).GetComponent<Button>().onClick.AddListener(delegate{
            difficulty = 0;
            updateLeaderboard();
        });
        transform.GetChild(2).GetChild(9).GetComponent<Button>().onClick.AddListener(delegate{
            difficulty = 1;
            updateLeaderboard();
        });
        transform.GetChild(2).GetChild(10).GetComponent<Button>().onClick.AddListener(delegate{
            difficulty = 2;
            updateLeaderboard();
        });
        transform.GetChild(2).GetChild(11).GetComponent<Button>().onClick.AddListener(delegate{
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
        });

        transform.GetChild(3).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            transform.GetChild(3).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
        });
    }

    void Update() {
        if (dictController == null) {
            loadDontDestroys();
        }

        switch (dictController.modeIndex) {
            case 0:
                transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Easy Mode";
                break;
            case 1:
                transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Hard Mode";
                break;
            case 2:
                transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Glitch Mode";
                break;
        }
    }

    public void loadDontDestroys() {
        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (t.name == "Win Screen") {
                winScreen = t.gameObject;
            }
        }
        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (t.name == "Dictionary") {
                dictController = t.gameObject.GetComponent<DictionaryController>();
            }
        }

        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (t.name == "UI") {
                uiHandler = t.gameObject.GetComponent<UIHandler>();
            }
        }

        switch (dictController.modeIndex) {
            case 0:
                transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Easy Mode";
                break;
            case 1:
                transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Hard Mode";
                break;
            case 2:
                transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Glitch Mode";
                break;
        }
    }

    void updateLeaderboard() {
        GameObject leaderboard = transform.GetChild(2).gameObject;

        LeaderboardController leaderboardController = null;

        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (t.name == "Leaderboard") {
                leaderboardController = t.gameObject.GetComponent<LeaderboardController>();
            }
        }

        List<LeaderboardEntry> entries = null;

        switch (leaderboardLevel) {
            case 1:
                entries = leaderboardController.leaderboardData.levelOne;
                break;
            case 2:
                entries = leaderboardController.leaderboardData.levelTwo;
                break;
            case 3:
                entries = leaderboardController.leaderboardData.levelThree;
                break;
        }

        entries.RemoveAll(x => x.difficulty!=difficulty);
        entries.Sort((x,y) => x.time.CompareTo(y.time));

        LeaderboardEntry firstPlace = null;
        LeaderboardEntry secondPlace = null;
        LeaderboardEntry thirdPlace = null;

        if (entries.Count < 1) {
            firstPlace = new LeaderboardEntry(){playerName = "Nobody", time=0, jumpCount=0, letters=0, difficulty=difficulty};
        } else {
            firstPlace = entries[0];
        }
        if (entries.Count < 2) {
            secondPlace = new LeaderboardEntry(){playerName = "Nobody", time=0, jumpCount=0, letters=0, difficulty=difficulty};
        } else {
            secondPlace = entries[1];
        }
        if (entries.Count < 3) {
            thirdPlace = new LeaderboardEntry(){playerName = "Nobody", time=0, jumpCount=0, letters=0, difficulty=difficulty};
        } else {
            thirdPlace = entries[2];
        }

        Transform firstPlaceObj = leaderboard.transform.GetChild(4);
        Transform secondPlaceObj = leaderboard.transform.GetChild(5);
        Transform thirdPlaceObj = leaderboard.transform.GetChild(6);

        firstPlaceObj.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = firstPlace.playerName;
        firstPlaceObj.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = firstPlace.time + "s";
        firstPlaceObj.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = firstPlace.jumpCount + "";
        firstPlaceObj.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = firstPlace.letters + "";

        secondPlaceObj.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = secondPlace.playerName;
        secondPlaceObj.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = secondPlace.time + "s";
        secondPlaceObj.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = secondPlace.jumpCount + "";
        secondPlaceObj.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = secondPlace.letters + "";

        thirdPlaceObj.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = thirdPlace.playerName;
        thirdPlaceObj.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = thirdPlace.time + "s";
        thirdPlaceObj.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = thirdPlace.jumpCount + "";
        thirdPlaceObj.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = thirdPlace.letters + "";
    }
}
