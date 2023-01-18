using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Netcode;

public class CameraController : NetworkBehaviour // inherit from NetworkBehaviour
{

    GameObject player; // reference to the player object

    // Start is called before the first frame update
    void Start()
    {
        player = NetworkManager.LocalClient.PlayerObject.gameObject; // assign the local player object to the player variable
    }

    // Update is called once per frame
    void Update()
    {
        float yPos = player.transform.localPosition.y; // get the y position of the player object
        if (SceneManager.GetActiveScene().name == "Map 1") { // check if the active scene is Map 1
            if (yPos < 2.5f) { // if the player's y position is less than 2.5
                transform.localPosition = new Vector3(transform.localPosition.x, 2.5f, -10); // set the camera's y position to 2.5
            } else {
                transform.localPosition = new Vector3(transform.localPosition.x, yPos, -10); // otherwise, set the camera's y position to the player's y position
            }
        } else {
            if (yPos < 0) { // if the player's y position is less than 0
                transform.localPosition = new Vector3(transform.localPosition.x, 0, -10); // set the camera's y position to 0
            } else {
                if (yPos < 24.5f) { // if the player's y position is less than 24.5
                    transform.localPosition = new Vector3(transform.localPosition.x, yPos, -10); // set the camera's y position to the player's y position
                } else {
                    transform.localPosition = new Vector3(transform.localPosition.x, 24.5f, -10); // otherwise, set the camera's y position to 24.5
                }
            }
        }
    }
}