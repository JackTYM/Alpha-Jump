using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TutorialTextHandler : MonoBehaviour
{
    // Enum to hold the different conditions that trigger tutorial text
    public enum passConditions {
        AngleChange,
        AngleNudge,
        PowerChange,
        Jump,
        WallBounce,
        GrabKey,
        Finish,
    }

    // Lists to hold the tutorial text, conditions to trigger each text, and congratulatory messages
    public List<string> tutorialText;
    public List<passConditions> conditions;
    public List<string> congratulatoryMessages;
    
    // Variables to keep track of the current tutorial text index, jump count, and whether the tutorial is finished
    int currentIndex = 0;
    int jumpCount = 0;
    Stopwatch congratulationTimer;
    bool finished = false;
    TMPro.TextMeshPro textBox;

    GameObject letters;

    // Start is called before the first frame update
    void Start()
    {
        // Get the text box component
        textBox = GetComponent<TMPro.TextMeshPro>();

        // Deactivate all letter objects
        for (int i = 0; i <= 9; i++) {
            GameObject.Find("Letters").transform.GetChild(i).gameObject.SetActive(false);
        }

        // Set the initial tutorial text
        textBox.text = tutorialText[currentIndex];

        // Start the congratulation timer
        congratulationTimer = Stopwatch.StartNew();

        // Find the letters game object
        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (t.name == "Letters") {
                letters = t.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the congratulation timer has expired and the tutorial is not finished
        if (congratulationTimer == null || congratulationTimer.ElapsedMilliseconds > 1000 && !finished) {
            textBox.text = tutorialText[currentIndex];
        }
    }

    public void TriggerCondition(passConditions condition) {
        if (conditions[currentIndex] == condition && congratulationTimer.ElapsedMilliseconds > 1000) {
            // Reset the congratulation timer
            congratulationTimer = Stopwatch.StartNew();
            // Update the tutorial text box with a random congratulatory message
            textBox.text = congratulatoryMessages[Random.Range(0, congratulatoryMessages.Count)];

            if (letters == null) {
                // Find the letters game object
                foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                    if (t.name == "Letters") {
                        letters = t.gameObject;
                    }
                }
            }

            switch (condition) {
                case passConditions.AngleNudge:
                    // Activates the first 2 letters
                    for (int i = 0; i <= 9; i++) {
                        letters.transform.GetChild(i).gameObject.SetActive(i < 2);
                    }
                    break;
                case passConditions.Jump:
                    // Activates different letters depending on the jump count
                    if (jumpCount == 0) {
                        for (int i = 0; i <= 9; i++) {
                            letters.transform.GetChild(i).gameObject.SetActive(i < 4);
                        }
                    } else {
                        for (int i = 0; i <= 9; i++) {
                            letters.transform.GetChild(i).gameObject.SetActive(true);
                        }
                    }
                    jumpCount++;
                    break;
                case passConditions.WallBounce:
                    // Activates the "Key" object
                    GameObject.Find("Surfaces").transform.Find("Key Door").transform.GetChild(1).gameObject.SetActive(true);
                    break;
                
            }

            // If the tutorial has more content, add to index, if not, show finished text
            if (currentIndex < tutorialText.Count-1) {
                currentIndex++;
            } else {
                textBox.text = "Congratulations! You have finished the tutorial!";
                finished = true;
            }
        }
    }
}

