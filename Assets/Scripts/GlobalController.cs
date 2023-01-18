using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // for accessing the Scene Manager

public class GlobalController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("DontDestroy").Length <= 1) {
            DontDestroyOnLoad(this); // make this gameObject persist across scenes
        } else {
            foreach (GameObject o in GameObject.FindGameObjectsWithTag("DontDestroy")) {
                if (o.scene.name != "DontDestroyOnLoad") {
                    GameObject.Destroy(o); // destroy the duplicate gameObjects
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { // check if the Escape key is pressed
            if (SceneManager.GetActiveScene().name.Contains("Map")) { // check if the active scene is a map
                transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf); // toggle the active state of the first child gameObject
            } else {
                Application.Quit(); // otherwise, quit the application
            }
        }
    }
}