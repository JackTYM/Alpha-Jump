using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class MovementHandler : MonoBehaviour
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
    GameObject player;
    GameObject pauseScreen;
    GameObject winScreen;
    DictionaryController dictController;
    UIHandler uiHandler;
    LineRenderer lr;
    Stopwatch arrowHoldTimer = Stopwatch.StartNew();
    Stopwatch onFloorFailsafe = Stopwatch.StartNew();
    float onFloorY = 0f;
    List<GameObject> floorCollisions = new List<GameObject>();

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

        transform.localRotation = new Quaternion();

        if(!pauseScreen.activeSelf && !winScreen.activeSelf) {

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

            if ((Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space)) && onFloor) {
                if (dictController.validWord(uiHandler.currentWord.ToLower()) || dictController.easyMode) {
                    if (!clickedLastTick) {
                        currentJumpPower = uiHandler.currentWord.Length * 15f;
                        uiHandler.currentWord = "";
                        lr.positionCount = 0;

                        Jump();
                        winScreen.GetComponent<WinController>().jumpCount += 1;
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

            yVel -= stoppingForce * Time.deltaTime;

            if (yVel == 0) {
                yVel = -0.01f;
            }

            if (onFloorFailsafe.ElapsedMilliseconds > 500) {
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

    public void Jump() {
        var tempShittyCode = (currentJumpAngle-90)*-1;
        if (tempShittyCode > 180) {
            tempShittyCode -= 360;
        }

        xVel = currentJumpPower*(tempShittyCode / 90);
        yVel = currentJumpPower*(1-(Mathf.Abs(tempShittyCode) / 90));
        transform.GetChild(0).gameObject.SetActive(false);

        if (currentJumpPower != 0f) {
            onFloor = false;
        }

        onFloorFailsafe = Stopwatch.StartNew();
        if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
            GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.Jump);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Vector2 direction = collision.GetContact(0).normal;

        if (collision.gameObject.tag == "Kill") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            winScreen.GetComponent<WinController>().levelStopwatch = Stopwatch.StartNew();
            winScreen.GetComponent<WinController>().jumpCount = 0;
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
                if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                    GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.Finish);
                }
                winScreen.GetComponent<WinController>().EndLevel();
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
        col.gameObject.GetComponent<KeyHandler>().doorToUnlock.SetActive(false);
        col.gameObject.SetActive(false);

        if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
            GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.GrabKey);
        }
    }
}
