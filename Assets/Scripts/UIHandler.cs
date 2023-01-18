using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{

    public string currentWord = "";
    public GameObject errorText;
    List<KeyCode> allowedInputs = new List<KeyCode>(){KeyCode.Minus};
    GameObject pauseScreen;
    GameObject winScreen;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 97; i <= 122; i++) {
            allowedInputs.Add((KeyCode)i);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (pauseScreen == null) {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "Pause Screen") {
                    pauseScreen = t.gameObject;
                }
            }
        }

        if (winScreen == null) {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "Win Screen") {
                    winScreen = t.gameObject;
                }
            }
        }

        if(!pauseScreen.activeSelf && !winScreen.activeSelf) {
            foreach(KeyCode code in allowedInputs) {
                if (Input.GetKeyDown(code) && currentWord.Length < getMaxLength()) {
                    currentWord += code.ToString();
                    errorText.SetActive(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.Backspace) && currentWord.Length > 0) {
                currentWord = currentWord.Substring(0, currentWord.Length-1);
                errorText.SetActive(false);
            }

            for (int i = 0; i <= 9; i++) {
                GameObject currentLetter = transform.GetChild(0).GetChild(i).gameObject;
                if (currentWord.Length < i+1) {
                    currentLetter.GetComponent<Image>().color = new Color(1,1,1,0.1f);
                    currentLetter.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                } else {
                    currentLetter.GetComponent<Image>().color = new Color(1,1,1,1f);
                    currentLetter.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = currentWord[i].ToString();
                }
            }
        }

        if (winScreen.GetComponent<WinController>().levelStopwatch != null) {
            if (winScreen.GetComponent<WinController>().levelStopwatch.IsRunning && (pauseScreen.activeSelf || winScreen.activeSelf)) {
                winScreen.GetComponent<WinController>().levelStopwatch.Stop();
            }
            if (!winScreen.GetComponent<WinController>().levelStopwatch.IsRunning && !(pauseScreen.activeSelf || winScreen.activeSelf)) {
                winScreen.GetComponent<WinController>().levelStopwatch.Start();
            }
        }

        if (winScreen.GetComponent<WinController>().levelStopwatch != null) {
            transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = Mathf.Round(winScreen.GetComponent<WinController>().levelStopwatch.ElapsedMilliseconds/100)/10 + "s";
        }
    }

    int getMaxLength() {
        int returnVal = 0;
        for (int i = 0; i <= 9; i++) {
            if (transform.GetChild(0).GetChild(i).gameObject.activeSelf) {
                returnVal++;
            } else {
                break;
            }
        }

        return returnVal;
    }
}
