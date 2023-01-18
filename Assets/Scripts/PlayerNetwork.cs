using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerNetwork : NetworkBehaviour
{
    public GameObject keyDoor;
    public GameObject keyDoor1;
    public NetworkVariable<float> timeForLevel = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> jumpCount = new(writePerm: NetworkVariableWritePermission.Owner);
    private GameObject spawnedKeyDoor;
    WinController winController;
    NetworkManager networkManager = null;
    private NetworkVariable<Vector3> _netPos = new(writePerm: NetworkVariableWritePermission.Owner);
    public Vector3 overwritePos;
    UIHandler uiHandler = null;
    bool hidPlayer = false;

    // Update is called once per frame
    void Update()
    {
        if (uiHandler == null) {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "UI") {
                    uiHandler = t.gameObject.GetComponent<UIHandler>();
                }
            }
        }

        if (networkManager == null) {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "NetworkManager") {
                    networkManager = t.gameObject.GetComponent<NetworkManager>();
                }
            }
        }

        if (IsOwner) {
            if (winController == null) {
                foreach (WinController win in Resources.FindObjectsOfTypeAll<WinController>()) {
                    winController = win;
                }
            }
        }

        if (IsHost && spawnedKeyDoor == null && (SceneManager.GetActiveScene().name.Contains("Map 1") || SceneManager.GetActiveScene().name.Contains("Map 3"))) {
            if (SceneManager.GetActiveScene().name.Contains("Map 1")) {
                spawnedKeyDoor = Instantiate(keyDoor, new Vector3(4.000426f, 7f, 0f), Quaternion.identity);
            } else {
                spawnedKeyDoor = Instantiate(keyDoor1, new Vector3(5, 25.5f, 0f), Quaternion.identity);
            }
            spawnedKeyDoor.GetComponent<NetworkObject>().Spawn(true);
            spawnedKeyDoor.GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);
        }

        if (!IsOwner && !IsHost) {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if ((t.name == "Key Door(Clone)" || t.name == "Key Door 1(Clone)") && !t.GetComponent<NetworkObject>().IsOwner && t.gameObject.activeSelf) {
                    t.gameObject.SetActive(false);
                }
            }
        }

        if (!IsOwner && spawnedKeyDoor != null && spawnedKeyDoor.activeSelf) {
            spawnedKeyDoor.SetActive(false);
        }

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

        if (IsOwner) {
            Debug.Log(winController.levelStopwatch);
            if (winController.levelStopwatch != null && winController.levelStopwatch.ElapsedMilliseconds != 0) {
                timeForLevel.Value = Mathf.Round(winController.levelStopwatch.ElapsedMilliseconds/100)/10;
            }
            jumpCount.Value = winController.jumpCount;
        }

        GetComponent<SpriteRenderer>().enabled = SceneManager.GetActiveScene().name.Contains("Map ");
        uiHandler.gameObject.SetActive(SceneManager.GetActiveScene().name.Contains("Map "));

        GetComponent<SpriteRenderer>().color = new Color(1,1,1, (IsOwner ? 1 : 0.5f));

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
