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
        if (NetworkManager.IsConnectedClient || NetworkManager.IsHost) {
            player = NetworkManager.LocalClient.PlayerObject.gameObject; // assign the local player object to the player variable
        } else {
            player = GameObject.Find("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float yPos = player.transform.localPosition.y; // get the y position of the player object
        if (yPos < 2.5f) { // if the player's y position is less than 2.5
            transform.localPosition = new Vector3(transform.localPosition.x, 2.5f, -10); // set the camera's y position to 2.5
        } else {
            if (yPos < 24.5f) { // if the player's y position is less than 24.5
                transform.localPosition = new Vector3(transform.localPosition.x, yPos, -10); // set the camera's y position to the player's y position
            } else {
                transform.localPosition = new Vector3(transform.localPosition.x, 24.5f, -10); // otherwise, set the camera's y position to 24.5
            }
        }
    }
}