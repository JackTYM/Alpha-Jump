using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/Leaderboard.json")) {
            System.IO.File.CreateText(Application.persistentDataPath + "/Leaderboard.json"); // create a file to store leaderboard data
            System.IO.File.WriteAllText(Application.persistentDataPath + "/Leaderboard.json", "{}"); // write an empty JSON object to the file
        }
        leaderboardData = JsonUtility.FromJson<LeaderboardData>(System.IO.File.ReadAllText(Application.persistentDataPath + "/Leaderboard.json")); // read the data from the file and store it in the leaderboardData object
    }

    [SerializeField] public LeaderboardData leaderboardData = new LeaderboardData(); // object to store the leaderboard data

    public void addScore(LeaderboardEntry entry, int level) { // function to add a score to the leaderboard
        switch (level) { // check the level and add the entry to the corresponding list
            case 1:
                leaderboardData.levelOne.Add(entry);
                break;
            case 2:
                leaderboardData.levelTwo.Add(entry);
                break;
            case 3:
                leaderboardData.levelThree.Add(entry);
                break;
        }

        string json = JsonUtility.ToJson(leaderboardData); // convert the leaderboardData object to a JSON string

        System.IO.File.WriteAllText(Application.persistentDataPath + "/Leaderboard.json", json); // write the JSON string to the file
    }
}

[System.Serializable]
public class LeaderboardData {
    public List<LeaderboardEntry> levelOne = new List<LeaderboardEntry>(); // list for level 1 scores
    public List<LeaderboardEntry> levelTwo = new List<LeaderboardEntry>(); // list for level 2 scores
    public List<LeaderboardEntry> levelThree = new List<LeaderboardEntry>(); // list for level 3 scores
    public List<LeaderboardEntry> levelTutorial = new List<LeaderboardEntry>(); // list for tutorial scores
}

[System.Serializable]
public class LeaderboardEntry {
    public string playerName; // player name
    public int jumpCount; // jump count
    public float time; // time
    public int letters; // letters
    public int difficulty; // difficulty
}