using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/Leaderboard.json")) {
            System.IO.File.CreateText(Application.persistentDataPath + "/Leaderboard.json");
            System.IO.File.WriteAllText(Application.persistentDataPath + "/Leaderboard.json", "{}");
        }
        leaderboardData = JsonUtility.FromJson<LeaderboardData>(System.IO.File.ReadAllText(Application.persistentDataPath + "/Leaderboard.json"));
    }

    [SerializeField] public LeaderboardData leaderboardData = new LeaderboardData();

    public void addScore(LeaderboardEntry entry, int level) {
        switch (level) {
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

        string json = JsonUtility.ToJson(leaderboardData);

        System.IO.File.WriteAllText(Application.persistentDataPath + "/Leaderboard.json", json);
    }
}

[System.Serializable]
public class LeaderboardData {
    public List<LeaderboardEntry> levelOne = new List<LeaderboardEntry>();
    public List<LeaderboardEntry> levelTwo = new List<LeaderboardEntry>();
    public List<LeaderboardEntry> levelThree = new List<LeaderboardEntry>();
    public List<LeaderboardEntry> levelTutorial = new List<LeaderboardEntry>();
}

[System.Serializable]
public class LeaderboardEntry {
    public string playerName;
    public int jumpCount;
    public float time;
    public int letters;
    public int difficulty;
}