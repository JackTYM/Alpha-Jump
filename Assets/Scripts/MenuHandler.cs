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
            GetComponent<AudioSource>().Play();
            loadDontDestroys(); // function to load objects that should not be destroyed on scene change
            dictController.modeIndex++; // increment the mode index
            if (dictController.modeIndex == 3) {
                dictController.modeIndex = 0;
            }

            switch (dictController.modeIndex) {
                case 0:
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(-288, 0, 0);
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);
                    break;
                case 1:
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(-15, 0, 0);
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(215, 100);
                    break;
                case 2:
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(285, 0, 0);
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(250, 100);
                    break;
            }
        });

        // Main Menu
        // Play Game Button
        transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        });
        // Rules Button
        transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(4).gameObject.SetActive(true);
        });
        // Leaderboard Button
        transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            updateLeaderboard();
        });
        // Quit Button
        transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();Application.Quit();});

        // Level One Button
        transform.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            loadDontDestroys();
            SceneManager.LoadScene("Map 1");
            uiHandler.gameObject.SetActive(true);
            countdown.startCountdown();
        });
        // Level Two Button
        transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            loadDontDestroys();
            SceneManager.LoadScene("Map 2");
            uiHandler.gameObject.SetActive(true);
            countdown.startCountdown();
        });
        // Level Three Button
        transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            loadDontDestroys();
            SceneManager.LoadScene("Map 3");
            uiHandler.gameObject.SetActive(true);
            countdown.startCountdown();
        });
        // Multiplayer Button
        transform.GetChild(1).GetChild(4).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(true);
        });
        // Tutorial Button
        transform.GetChild(1).GetChild(5).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            loadDontDestroys();
            SceneManager.LoadScene("Tutorial Map");
            uiHandler.gameObject.SetActive(true);
        });
        // Back Button
        transform.GetChild(1).GetChild(6).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
        });

        // Leaderboard
        // Leaderboard Level 1
        transform.GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            leaderboardLevel = 1;
            updateLeaderboard();
        });
        // Leaderboard Level 2
        transform.GetChild(2).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            leaderboardLevel = 2;
            updateLeaderboard();
        });
        // Leaderboard Level 3
        transform.GetChild(2).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            leaderboardLevel = 3;
            updateLeaderboard();
        });
        // Leaderboard Easy Mode
        transform.GetChild(2).GetChild(8).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            difficulty = 0;
            updateLeaderboard();
        });
        // Leaderboard Hard Mode
        transform.GetChild(2).GetChild(9).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            difficulty = 1;
            updateLeaderboard();
        });
        // Leaderboard Glitch Mode
        transform.GetChild(2).GetChild(10).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            difficulty = 2;
            updateLeaderboard();
        });
        // Back Button
        transform.GetChild(2).GetChild(11).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
        });

        // Multiplayer
        // Start Server Button
        transform.GetChild(3).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            NetworkManager.StartHost();
        });
        // Join Server Button
        transform.GetChild(3).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            if (transform.GetChild(3).GetChild(4).GetComponent<TMPro.TMP_InputField>().text != "") {
                if (!transform.GetChild(3).GetChild(4).GetComponent<TMPro.TMP_InputField>().text.Contains(":")) {
                    NetworkManager.gameObject.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>().ConnectionData.Address = transform.GetChild(3).GetChild(4).GetComponent<TMPro.TMP_InputField>().text;
                } else {
                    NetworkManager.gameObject.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>().ConnectionData.Address = transform.GetChild(3).GetChild(4).GetComponent<TMPro.TMP_InputField>().text.Split(":")[0];
                    NetworkManager.gameObject.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>().ConnectionData.Port = ushort.Parse(transform.GetChild(3).GetChild(4).GetComponent<TMPro.TMP_InputField>().text.Split(":")[1]);
                }
            }
            NetworkManager.StartClient();
        });
        // Copy IP Button
        transform.GetChild(3).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            GUIUtility.systemCopyBuffer = GetIPAddress();
        });
        // Level 1
        transform.GetChild(3).GetChild(5).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            loadDontDestroys();
            NetworkManager.SceneManager.LoadScene("Map 1", LoadSceneMode.Single);
            foreach (NetworkClient network in NetworkManager.ConnectedClients.Values)
            {
                network.PlayerObject.transform.position = new Vector3(0f, 0f, -0.57f);
                network.PlayerObject.GetComponent<PlayerNetwork>().overwritePos = new Vector3(0f, 0f, -0.57f);
            }
        });
        // Level 2
        transform.GetChild(3).GetChild(6).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            loadDontDestroys();
            NetworkManager.SceneManager.LoadScene("Map 2", LoadSceneMode.Single);
            foreach (NetworkClient network in NetworkManager.ConnectedClients.Values)
            {
                network.PlayerObject.transform.position = new Vector3(0f, 0f, -0.57f);
                network.PlayerObject.GetComponent<PlayerNetwork>().overwritePos = new Vector3(0f, 0f, -0.57f);
            }
        });
        // Level 3
        transform.GetChild(3).GetChild(7).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            loadDontDestroys();
            NetworkManager.SceneManager.LoadScene("Map 3", LoadSceneMode.Single);
            foreach (NetworkClient network in NetworkManager.ConnectedClients.Values)
            {
                network.PlayerObject.transform.position = new Vector3(0f, 0f, -0.57f);
                network.PlayerObject.GetComponent<PlayerNetwork>().overwritePos = new Vector3(0f, 0f, -0.57f);
            }
        });
        // Back Button
        transform.GetChild(3).GetChild(8).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            transform.GetChild(3).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        });

        // Back Button
        transform.GetChild(4).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            GetComponent<AudioSource>().Play();
            transform.GetChild(4).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
        });
    }

    void Update() {
        // Load don't destroy objects if they are null
        if (dictController == null) {
            loadDontDestroys();
        }

        // Set the current mode
        switch (dictController.modeIndex) {
                case 0:
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(-288, 0, 0);
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);
                    break;
                case 1:
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(-15, 0, 0);
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(215, 100);
                    break;
                case 2:
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(285, 0, 0);
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(250, 100);
                    break;
            }
    }

    // Loads all objects not located in the scene
    public void loadDontDestroys() {
        dictController = Resources.FindObjectsOfTypeAll<DictionaryController>()[0];
        uiHandler = Resources.FindObjectsOfTypeAll<UIHandler>()[0];
        countdown = Resources.FindObjectsOfTypeAll<CountdownHandler>()[0];

        switch (dictController.modeIndex) {
                case 0:
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(-288, 0, 0);
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);
                    break;
                case 1:
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(-15, 0, 0);
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(215, 100);
                    break;
                case 2:
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(285, 0, 0);
                    transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(250, 100);
                    break;
            }
    }

    // Gets the current public IP address
    private string GetIPAddress()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://checkip.dyndns.org");

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.DataProcessingError)
        {
            return "Error";
        }
        else
        {
            string result = www.downloadHandler.text;

            string[] a = result.Split(':'); // Split into two substrings -> one before : and one after. 
            string a2 = a[1].Substring(1);  // Get the substring after the :
            string[] a3 = a2.Split('<');    // Now split to the first HTML tag after the IP address.
            string a4 = a3[0];              // Get the substring before the tag.

            return a4;
        }
    }

    // Updates the leaderboard with the current level and difficulty
    void updateLeaderboard() {
        // Get the leaderboard object
        GameObject leaderboard = transform.GetChild(2).gameObject;

        LeaderboardController leaderboardController = null;

        // Find the leaderboard controller in the scene
        leaderboardController = Resources.FindObjectsOfTypeAll<LeaderboardController>()[0];

        leaderboardController.leaderboardData = JsonUtility.FromJson<LeaderboardData>(System.IO.File.ReadAllText(Application.persistentDataPath + "/Leaderboard.json")); // read the data from the file and store it in the leaderboardData object

        // Get the leaderboard entries based on the level
        List<LeaderboardEntry> entries = leaderboardLevel == 1 ? leaderboardController.leaderboardData.levelOne : leaderboardLevel == 2 ? leaderboardController.leaderboardData.levelTwo : leaderboardController.leaderboardData.levelThree;

        // Remove all entries that don't match the current difficulty
        entries.RemoveAll(x => x.difficulty!=difficulty);
        // Sort the entries by time
        entries.Sort((x,y) => x.time.CompareTo(y.time));

        LeaderboardEntry firstPlace = null;
        LeaderboardEntry secondPlace = null;
        LeaderboardEntry thirdPlace = null;

        // Get the first, second, and third place entries
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

        // Get the UI objects for the first, second, and third place entries
        Transform firstPlaceObj = leaderboard.transform.GetChild(4);
        Transform secondPlaceObj = leaderboard.transform.GetChild(5);
        Transform thirdPlaceObj = leaderboard.transform.GetChild(6);

        // Update the UI text with the player name, time, jump count, and letters
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
