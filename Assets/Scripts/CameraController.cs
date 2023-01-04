using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float yPos = player.transform.localPosition.y;
        if (yPos < 0) {
            transform.localPosition = new Vector3(transform.localPosition.x, 0, -10);
        } else {

            if (yPos < 12 || SceneManager.GetActiveScene().name != "Level 1") {
                transform.localPosition = new Vector3(transform.localPosition.x, yPos, -10);
            } else {
                transform.localPosition = new Vector3(transform.localPosition.x, 12, -10);
            }
        }
    }
}
