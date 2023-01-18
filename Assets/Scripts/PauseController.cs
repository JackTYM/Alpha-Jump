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
        uiHandler = Resources.FindObjectsOfTypeAll<UIHandler>()[0];
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{gameObject.SetActive(false);});
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            uiHandler = Resources.FindObjectsOfTypeAll<UIHandler>()[0];
            SceneManager.LoadScene("Menu");
            gameObject.SetActive(false);
            uiHandler.gameObject.SetActive(false);
        });
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{Application.Quit();});
    }
}
