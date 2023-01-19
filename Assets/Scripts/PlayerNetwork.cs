using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerNetwork : NetworkBehaviour
{
    // Public variables
    public GameObject keyDoor; // reference to the key door prefab
    public GameObject keyDoor1; // reference to the key door 1 prefab
    public NetworkVariable<float> timeForLevel = new(writePerm: NetworkVariableWritePermission.Owner); // Network variable for the time taken to complete the level
    public NetworkVariable<int> jumpCount = new(writePerm: NetworkVariableWritePermission.Owner); // Network variable for the number of jumps taken
    private GameObject spawnedKeyDoor; // reference to the instantiated key door
    WinController winController; // reference to the win controller
    NetworkManager networkManager = null; // reference to the network manager
    private NetworkVariable<Vector3> _netPos = new(writePerm: NetworkVariableWritePermission.Owner); // Network variable for the player's position
    public Vector3 overwritePos; // Overwrite position for the player's position
    UIHandler uiHandler = null; // reference to the UI handler
    bool hidPlayer = false; // flag to check if the player has been hidden or not

    // Update is called once per frame
    void Update()
    {
        // Get the UIHandler component
        if (uiHandler == null) {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "UI") {
                    uiHandler = t.gameObject.GetComponent<UIHandler>();
                }
            }
        }

        // Get the NetworkManager component
        if (networkManager == null) {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "NetworkManager") {
                    networkManager = t.gameObject.GetComponent<NetworkManager>();
                }
            }
        }

        // Get the WinController component
        if (IsOwner) {
            if (winController == null) {
                foreach (WinController win in Resources.FindObjectsOfTypeAll<WinController>()) {
                    winController = win;
                }
            }
        }

        // Spawn the key door
        if (IsHost && spawnedKeyDoor == null && (SceneManager.GetActiveScene().name.Contains("Map 1") || SceneManager.GetActiveScene().name.Contains("Map 3"))) {
            if (SceneManager.GetActiveScene().name.Contains("Map 1")) {
                spawnedKeyDoor = Instantiate(keyDoor, new Vector3(4.000426f, 7f, 0f), Quaternion.identity);
            } else {
                spawnedKeyDoor = Instantiate(keyDoor1, new Vector3(5, 25.5f, 0f), Quaternion.identity);
            }
            spawnedKeyDoor.GetComponent<NetworkObject>().Spawn(true);
            spawnedKeyDoor.GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);
        }

        // Deactivate the key door for non-owners
        if (!IsOwner && !IsHost) {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if ((t.name == "Key Door(Clone)" || t.name == "Key Door 1(Clone)") && !t.GetComponent<NetworkObject>().IsOwner && t.gameObject.activeSelf) {
                    t.gameObject.SetActive(false);
                }
            }
        }

        // Deactivate the other player's keydoor if it is active
        if (!IsOwner && spawnedKeyDoor != null && spawnedKeyDoor.activeSelf) {
            spawnedKeyDoor.SetActive(false);
        }

        // Deactivate the solo map's player and key door
        if (IsOwner && SceneManager.GetActiveScene().name.Contains("Map ") && !hidPlayer) {
            GameObject.Find("Player").SetActive(false);
            if (!SceneManager.GetActiveScene().name.Contains("Map 2")) {
                GameObject.Find("Key Door").SetActive(false);
            }
            hidPlayer = true;

            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "Countdown Screen") {
                    t.gameObject.GetComponent<CountdownHandler>().startCountdown();
                }
            }
        }

        // Set the current time elapsed and jump count for the win/lose screen
        if (IsOwner) {
            Debug.Log(winController.levelStopwatch);
            if (winController.levelStopwatch != null && winController.levelStopwatch.ElapsedMilliseconds != 0) {
                timeForLevel.Value = Mathf.Round(winController.levelStopwatch.ElapsedMilliseconds/100)/10;
            }
            jumpCount.Value = winController.jumpCount;
        }

        // Show the player if you are in a game
        GetComponent<SpriteRenderer>().enabled = SceneManager.GetActiveScene().name.Contains("Map ");
        uiHandler.gameObject.SetActive(SceneManager.GetActiveScene().name.Contains("Map "));

        // Set other player's alpha level to 0.5
        GetComponent<SpriteRenderer>().color = new Color(1,1,1, (IsOwner ? 1 : 0.5f));


        // Handle overwritten positions to remotely set player position
        if (IsOwner) {
            if (overwritePos != Vector3.zero) {
                transform.position = overwritePos;

                overwritePos = Vector3.zero;
            }
            _netPos.Value = transform.position;
        } else {
            transform.position = _netPos.Value;
        }
    }
}
