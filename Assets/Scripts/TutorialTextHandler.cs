using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TutorialTextHandler : MonoBehaviour
{

    public enum passConditions {
        AngleChange,
        AngleNudge,
        PowerChange,
        Jump,
        WallBounce,
        GrabKey,
        Finish,
    }

    public List<string> tutorialText;
    public List<passConditions> conditions;
    public List<string> congratulatoryMessages;
    
    int currentIndex = 0;
    int jumpCount = 0;
    Stopwatch congratulationTimer;
    bool finished = false;
    TMPro.TextMeshPro textBox;

    GameObject letters;

    // Start is called before the first frame update
    void Start()
    {
        textBox = GetComponent<TMPro.TextMeshPro>();

        for (int i = 0; i <= 9; i++) {
            GameObject.Find("Letters").transform.GetChild(i).gameObject.SetActive(false);
        }

        textBox.text = tutorialText[currentIndex];

        congratulationTimer = Stopwatch.StartNew();

        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (t.name == "Letters") {
                letters = t.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (congratulationTimer == null || congratulationTimer.ElapsedMilliseconds > 1000 && !finished) {
            textBox.text = tutorialText[currentIndex];
        }
    }

    public void TriggerCondition(passConditions condition) {
        if (conditions[currentIndex] == condition && congratulationTimer.ElapsedMilliseconds > 1000) {
            congratulationTimer = Stopwatch.StartNew();
            textBox.text = congratulatoryMessages[Random.Range(0, congratulatoryMessages.Count)];

            switch (condition) {
                case passConditions.AngleNudge:
                    for (int i = 0; i <= 9; i++) {
                        GameObject.Find("Letters").transform.GetChild(i).gameObject.SetActive(i < 2);
                    }
                    break;
                case passConditions.Jump:
                    if (jumpCount == 0) {
                        for (int i = 0; i <= 9; i++) {
                            GameObject.Find("Letters").transform.GetChild(i).gameObject.SetActive(i < 4);
                        }
                    } else {
                        for (int i = 0; i <= 9; i++) {
                            GameObject.Find("Letters").transform.GetChild(i).gameObject.SetActive(true);
                        }
                    }
                    jumpCount++;
                    break;
                case passConditions.WallBounce:
                    GameObject.Find("Surfaces").transform.Find("Key").gameObject.SetActive(true);
                    break;
                
            }

            if (currentIndex < tutorialText.Count-1) {
                currentIndex++;
            } else {
                textBox.text = "Congratulations! You have finished the tutorial!";
                finished = true;
            }
        }
    }
}
