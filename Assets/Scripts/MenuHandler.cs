using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Diagnostics;
using Unity.Netcode;

public class MenuHandler : NetworkBehaviour
{
    int leaderboardLevel = 1; // variable to store the current level shown in the leaderboard
    int difficulty = 0; // variable to store the current difficulty level
    CountdownHandler countdown = null; // variable to store the countdown handler
    DictionaryController dictController = null; // variable to store the dictionary controller
    UIHandler uiHandler = null; // variable to store the UI handler

    // Start is called before the first frame update
    void Start()
    {
        loadDontDestroys(); // function to load objects that should not be destroyed on scene change

        //Modes
        transform.GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys(); // function to load objects that should not be destroyed on scene change
            dictController.modeIndex++; // increment the mode index
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
        //When the first button in the first child of the object this script is attached to is clicked, it will deactivate the first child gameObject and activate the second child gameObject
        transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate{
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        });
        //when the second button in the first child of the object this script is attached to is clicked, it will load the "Tutorial Map" scene and activate the "uiHandler" gameObject
        transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            SceneManager.LoadScene("Tutorial Map");
            uiHandler.gameObject.SetActive(true);
        });
        //when the third button in the first child of the object this script is attached to is clicked, it will deactivate the first child gameObject and activate the third child gameObject and call the "updateLeaderboard" function
        transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            updateLeaderboard();
        });
        //when the fourth button in the first child of the object this script is attached to is clicked, it will quit the application
        transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{Application.Quit();});

        //Levels
        //when the first button in the second child of the object this script is attached to is clicked, it will load the "Map 1" scene and activate the "uiHandler" gameObject and call the "startCountdown" function
        transform.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            SceneManager.LoadScene("Map 1");
            uiHandler.gameObject.SetActive(true);
            countdown.startCountdown();
        });
        //when the second button in the second child of the object this script is attached to is clicked, it will load the "Map 2" scene and activate the "uiHandler" gameObject and call the "startCountdown" function
        transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            SceneManager.LoadScene("Map 2");
            uiHandler.gameObject.SetActive(true);
            countdown.startCountdown();
        });
        //when the third button in the second child of the object this script is attached to is clicked, it will load the "Map 3" scene and activate the "uiHandler" gameObject and call the "startCountdown" function
        transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            uiHandler.gameObject.SetActive(true);
            countdown.startCountdown();
        });
        //when the fourth button in the second child of the object this script is attached to is clicked, it will hide the Levels screen and show the Multiplayer screen
        transform.GetChild(1).GetChild(4).GetComponent<Button>().onClick.AddListener(delegate{
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(4).gameObject.SetActive(true);
        });
        //when the fifth button in the second child of the object this script is attached to is clicked, it will hide the Levels screen and show the Menu screen
        transform.GetChild(1).GetChild(5).GetComponent<Button>().onClick.AddListener(delegate{
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

        transform.GetChild(4).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{
            NetworkManager.StartHost();
        });
        transform.GetChild(4).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            NetworkManager.gameObject.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>().ConnectionData.Address = transform.GetChild(4).GetChild(4).GetComponent<TMPro.TMP_InputField>().text;
            NetworkManager.StartClient();
        });
        transform.GetChild(4).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{
            GUIUtility.systemCopyBuffer = GetIPAddress();
        });
        transform.GetChild(4).GetChild(5).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            NetworkManager.SceneManager.LoadScene("Map 1", LoadSceneMode.Single);
            foreach (NetworkClient network in NetworkManager.ConnectedClients.Values)
            {
                network.PlayerObject.transform.position = new Vector3(0f, 0f, -0.57f);
                network.PlayerObject.GetComponent<PlayerNetwork>().overwritePos = new Vector3(0f, 0f, -0.57f);
            }
        });
        transform.GetChild(4).GetChild(6).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            NetworkManager.SceneManager.LoadScene("Map 2", LoadSceneMode.Single);
            foreach (NetworkClient network in NetworkManager.ConnectedClients.Values)
            {
                network.PlayerObject.transform.position = new Vector3(0f, 0f, -0.57f);
                network.PlayerObject.GetComponent<PlayerNetwork>().overwritePos = new Vector3(0f, 0f, -0.57f);
            }
        });
        transform.GetChild(4).GetChild(7).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            NetworkManager.SceneManager.LoadScene("Map 3", LoadSceneMode.Single);
            foreach (NetworkClient network in NetworkManager.ConnectedClients.Values)
            {
                network.PlayerObject.transform.position = new Vector3(0f, 0f, -0.57f);
                network.PlayerObject.GetComponent<PlayerNetwork>().overwritePos = new Vector3(0f, 0f, -0.57f);
            }
        });
        transform.GetChild(4).GetChild(8).GetComponent<Button>().onClick.AddListener(delegate{
            transform.GetChild(4).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
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

    private string GetIPAddress()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://checkip.dyndns.org");

        if (www.isNetworkError || www.isHttpError)
        {
            return "Error";
        }
        else
        {
            string result = www.downloadHandler.text;

            // This results in a string similar to this: <html><head><title>Current IP Check</title></head><body>Current IP Address: 123.123.123.123</body></html>
            // where 123.123.123.123 is your external IP Address.
            //  Debug.Log("" + result);

            string[] a = result.Split(':'); // Split into two substrings -> one before : and one after. 
            string a2 = a[1].Substring(1);  // Get the substring after the :
            string[] a3 = a2.Split('<');    // Now split to the first HTML tag after the IP address.
            string a4 = a3[0];              // Get the substring before the tag.

            return a4;
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
