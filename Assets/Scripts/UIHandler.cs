using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{

    public string currentWord = "";
    public GameObject errorText;
    List<KeyCode> allowedInputs = new List<KeyCode>(){KeyCode.Minus};

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
