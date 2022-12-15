using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("DontDestroy").Length <= 1) {
            DontDestroyOnLoad(this);
        } else {
            foreach (GameObject o in GameObject.FindGameObjectsWithTag("DontDestroy")) {
                if (o.scene.name != "DontDestroyOnLoad") {
                    GameObject.Destroy(o);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (SceneManager.GetActiveScene().name.Contains("Map")) {
                transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
            } else {
                Application.Quit();
            }
        }
    }
}
