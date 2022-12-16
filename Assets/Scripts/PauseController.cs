using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    UIHandler uiHandler = null;

    // Start is called before the first frame update
    void Start()
    {
        loadDontDestroys();
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{gameObject.SetActive(false);});
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            loadDontDestroys();
            SceneManager.LoadScene("Menu");
            gameObject.SetActive(false);
            uiHandler.gameObject.SetActive(false);
        });
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{Application.Quit();});
    }

    void loadDontDestroys() {
        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (t.name == "UI") {
                uiHandler = t.gameObject.GetComponent<UIHandler>();
            }
        }
    }
}
