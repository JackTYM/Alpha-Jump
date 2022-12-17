using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class MenuHandler : MonoBehaviour
{

    int leaderboardLevel = 1;
    GameObject winScreen = null;
    DictionaryController dictController = null;
    UIHandler uiHandler = null;

    // Start is called before the first frame update
    void Start()
    {
        loadDontDestroys();

        if (dictController.easyMode) {
            transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Easy Mode";
        } else {
            transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Hard Mode";
        }

        //Modes
        transform.GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(delegate{
            dictController.easyMode = !dictController.easyMode;

            if (dictController.easyMode) {
                transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Easy Mode";
            } else {
                transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Hard Mode";
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
            uiHandler.gameObject.SetActive(true);
        });
        transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            updateLeaderboard();
        });
        transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{Application.Quit();});

        //Levels
        transform.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            SceneManager.LoadScene("Map 1");
            winScreen.GetComponent<WinController>().levelStopwatch = Stopwatch.StartNew();
            winScreen.GetComponent<WinController>().jumpCount = 0;
            uiHandler.gameObject.SetActive(true);
        });
        transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            SceneManager.LoadScene("Map 2");
            winScreen.GetComponent<WinController>().levelStopwatch = Stopwatch.StartNew();
            winScreen.GetComponent<WinController>().jumpCount = 0;
            uiHandler.gameObject.SetActive(true);
        });
        transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            SceneManager.LoadScene("Map 3");
            winScreen.GetComponent<WinController>().levelStopwatch = Stopwatch.StartNew();
            winScreen.GetComponent<WinController>().jumpCount = 0;
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
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
        });
    }

    void loadDontDestroys() {
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
    }

    void updateLeaderboard() {
        GameObject leaderboard = transform.GetChild(2).gameObject;
    }
}
