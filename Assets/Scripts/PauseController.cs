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
        //Get the UIHandler component
        uiHandler = Resources.FindObjectsOfTypeAll<UIHandler>()[0];
        
        //Add a listener to the first child button that sets this object inactive
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate{gameObject.SetActive(false);});
        
        //Add a listener to the second child button that loads the menu scene and deactivates this object and the UIHandler
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate{
            uiHandler = Resources.FindObjectsOfTypeAll<UIHandler>()[0];
            SceneManager.LoadScene("Menu");
            gameObject.SetActive(false);
            uiHandler.gameObject.SetActive(false);
        });
        
        //Add a listener to the third child button that closes the application
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate{Application.Quit();});
    }
}