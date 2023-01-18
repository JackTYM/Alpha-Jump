using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Unity.Netcode;

public class MovementHandler : NetworkBehaviour
{

    public float currentJumpPower = 0f;
    public float currentJumpAngle = 0f;
    public float stoppingForce = 0.5f;

    float xVel = 0;
    float yVel = 0;
    Vector2 pausedVel = Vector2.zero;
    float angleDistance = 3.0f;
    bool clickedLastTick = false;
    bool onFloor = false;
    bool stuck = false;
    GameObject player;
    GameObject countdown;
    GameObject pauseScreen;
    GameObject winScreen;
    DictionaryController dictController;
    UIHandler uiHandler;
    LineRenderer lr;
    Stopwatch arrowHoldTimer = Stopwatch.StartNew();
    Stopwatch onFloorFailsafe = Stopwatch.StartNew();
    float onFloorY = 0f;
    List<GameObject> floorCollisions = new List<GameObject>();

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) {
            transform.GetChild(0).gameObject.SetActive(false);
            enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        lr = GetComponent<LineRenderer>();

        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (t.name == "Pause Screen") {
                pauseScreen = t.gameObject;
            } else if (t.name == "Win Screen") {
                winScreen = t.gameObject;
            }
        }

        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (t.name == "UI") {
                uiHandler = t.gameObject.GetComponent<UIHandler>();
            }
        }

        onFloorY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        if (pauseScreen == null) {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "Pause Screen") {
                    pauseScreen = t.gameObject;
                }
            }
        }

        if (winScreen == null) {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "Win Screen") {
                    winScreen = t.gameObject;
                }
            }
        }

        if (dictController == null) {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "Dictionary") {
                    dictController = t.gameObject.GetComponent<DictionaryController>();
                }
            }
        }

        if (countdown == null) {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "Countdown Screen") {
                    countdown = t.gameObject;
                }
            }
        }

        transform.localRotation = new Quaternion();

        if(!pauseScreen.activeSelf && !winScreen.activeSelf && !countdown.activeSelf && GetComponent<SpriteRenderer>().enabled) {

            if (pausedVel != Vector2.zero) {
                transform.GetComponent<Rigidbody2D>().velocity = pausedVel;
                pausedVel = Vector2.zero;
            }

            if (Input.GetMouseButton(0)) {
                Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 playerPos = player.transform.position;

                currentJumpAngle = Mathf.Round(Mathf.Atan2(mousePoint.y - playerPos.y, mousePoint.x - playerPos.x)*180/Mathf.PI);

                Vector3 difference = mousePoint - playerPos;
                difference.z = 0;

                angleDistance = Mathf.Min(3.0f, difference.magnitude);

                SetLineAngle();

                if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                    GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.AngleChange);
                }
            }

            if (arrowHoldTimer.ElapsedMilliseconds > 100) {
                if (Input.GetKey(KeyCode.LeftArrow)) {
                    arrowHoldTimer = Stopwatch.StartNew();
                    currentJumpAngle += 1;
                    SetLineAngle();
                    if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                        GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.AngleNudge);
                    }
                }

                if (Input.GetKey(KeyCode.RightArrow)) {
                    arrowHoldTimer = Stopwatch.StartNew();
                    currentJumpAngle -= 1;
                    SetLineAngle();
                    if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                        GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.AngleNudge);
                    }
                }
            }

            if ((Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space)) && (onFloor || dictController.modeIndex == 2)) {
                if (dictController.validWord(uiHandler.currentWord.ToLower()) || dictController.modeIndex != 1) {
                    if (!clickedLastTick) {
                        winScreen.GetComponent<WinController>().jumpCount += 1;
                        winScreen.GetComponent<WinController>().letterCount += uiHandler.currentWord.Length;
                        currentJumpPower = uiHandler.currentWord.Length * 15f;
                        uiHandler.currentWord = "";
                        lr.positionCount = 0;

                        Jump();
                        clickedLastTick = true;
                    }
                } else {
                    uiHandler.errorText.SetActive(true);
                    uiHandler.errorText.GetComponent<TMPro.TextMeshProUGUI>().text = uiHandler.currentWord.ToLower() + " is not a valid word!";
                    uiHandler.currentWord = "";
                }
            } else {
                clickedLastTick = false;
            }

            if (xVel > 0) {
                xVel -= stoppingForce * Time.deltaTime;
                xVel = xVel < 0 ? 0 : xVel;
            } else {
                xVel += stoppingForce * Time.deltaTime;
                xVel = xVel > 0 ? 0 : xVel;
            }

            if (!stuck) {
                yVel -= stoppingForce * Time.deltaTime;

                if (yVel == 0) {
                    yVel = -0.01f;
                }
            }

            if (onFloorFailsafe.ElapsedMilliseconds > 200) {
                if (transform.position.y == onFloorY && !onFloor) {
                    onFloor = true;
                    yVel = 0;
                }
                onFloorFailsafe = Stopwatch.StartNew();
                onFloorY = transform.position.y;
            }

            GetComponent<Rigidbody2D>().velocity = new Vector2(xVel, yVel);

        } else {
            pausedVel = transform.GetComponent<Rigidbody2D>().velocity;
            transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    void SetLineAngle() {
        Vector3 playerPos = player.transform.position;

        var direction = new Vector3(Mathf.Cos(currentJumpAngle*(Mathf.PI/180)), Mathf.Sin(currentJumpAngle*(Mathf.PI/180)), 0f);

        var endPosition = playerPos + direction * angleDistance;

        lr.startColor = Color.black;
        lr.endColor = Color.black;
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[]{playerPos, endPosition});

        var textPosition = direction * 0.9f;

        textPosition = new Vector3(textPosition.x, textPosition.y, 0f);

        transform.GetChild(0).localPosition = textPosition;
        transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = currentJumpAngle.ToString();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    //Triggers when player presses Enter or Space
    //Calculates degrees and uses that as a ratio for the velocity
    public void Jump() {
        //Calculate the degree angle of the jump by subtracting 90 from the current jump angle and then multiplying that value by -1
        var degreeAngle = (currentJumpAngle-90)*-1;
        // If the degree angle is greater than 180, decrement it by 360
        if (degreeAngle > 180) {
            degreeAngle -= 360;
        }

        // Set the x and y velocity of the object based on the current jump power and the calculated degree angle
        xVel = currentJumpPower*(degreeAngle / 90);
        yVel = currentJumpPower*(1-(Mathf.Abs(degreeAngle) / 90));
        // Set the angle text to inactive
        transform.GetChild(0).gameObject.SetActive(false);

        if (currentJumpPower != 0f) {
            onFloor = false;
            stuck = false;
        }

        // Start stopwatch for failsafe
        onFloorFailsafe = Stopwatch.StartNew();
        // Check if the current scene is a tutorial scene
        if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
            // Trigger tutorial text
            GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.Jump);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Vector2 direction = collision.GetContact(0).normal;

        if (collision.gameObject.tag == "Kill") {
            transform.position = new Vector3(0,0,-0.49f);
            xVel = 0;
            yVel = 0;
            if (SceneManager.GetActiveScene().name.Contains("Map 3")) {
                foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                    if ((t.name == "Key Door(Clone)" || t.name == "Key Door 1(Clone)") && t.GetComponent<NetworkObject>().IsOwner && !t.gameObject.activeSelf) {
                        t.gameObject.SetActive(true);
                    }
                }
            }
        }
        if (stuck) {
            return;
        }
        if (collision.gameObject.tag == "Sticky") {
            xVel = 0;
            yVel = 0;
            transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            stuck = true;
        }

        if(Mathf.Round(direction.x) != 0) {
            //Side Of Player
            if (collision.gameObject.tag == "Bounce") {
                xVel *= -1;
                if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                    GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.WallBounce);
                }
            } else {
                xVel = 0;
            }
        } else if(Mathf.Round(direction.y) == 1) {
            //Under Player
            if (collision.gameObject.tag == "Finish") {
                if (IsOwner) {
                    if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                        GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.Finish);
                    }
                    winScreen.GetComponent<WinController>().EndLevel();
                } else {
                    foreach (WinController win in Resources.FindObjectsOfTypeAll<WinController>()) {
                        win.LoseLevel(transform.gameObject.GetComponent<PlayerNetwork>().timeForLevel.Value, 
                            transform.gameObject.GetComponent<PlayerNetwork>().jumpCount.Value);
                    }
                }
            }
            yVel = 0;
            onFloor = true;
            floorCollisions.Add(collision.gameObject);
        } else if(Mathf.Round(direction.y) == -1) {
            //Above Player
            yVel *= -1;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (floorCollisions.Contains(collision.gameObject)) {
            onFloor = false;

            floorCollisions.Remove(collision.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (IsOwner && col.transform.parent.GetComponent<NetworkObject>().IsOwner && col.IsTouching(transform.GetComponent<CircleCollider2D>())) {
            col.transform.parent.gameObject.SetActive(false);

            if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.GrabKey);
            }
        }
    }
}
