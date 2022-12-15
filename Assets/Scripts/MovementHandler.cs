using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class MovementHandler : MonoBehaviour
{

    public float currentJumpPower = 3f;
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
    UIHandler uiHandler;
    LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        uiHandler = GameObject.Find("Canvas").GetComponent<UIHandler>();
        lr = GetComponent<LineRenderer>();

        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (t.name == "Pause Screen") {
                pauseScreen = t.gameObject;
            } else if (t.name == "Win Screen") {
                winScreen = t.gameObject;
            }
        }
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

        if(!pauseScreen.activeSelf && !winScreen.activeSelf) {

            if (pausedVel != Vector2.zero) {
                transform.GetComponent<Rigidbody2D>().velocity = pausedVel;
                pausedVel = Vector2.zero;
            }

            if (Input.GetMouseButton(0)) {
                Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 playerPos = player.transform.position;

                currentJumpAngle = Mathf.Round((Mathf.Atan2(mousePoint.y - playerPos.y, mousePoint.x - playerPos.x)* 180 / Mathf.PI - 90) *-1);
                if (currentJumpAngle > 180) {
                    currentJumpAngle = currentJumpAngle-360;
                }

                Vector3 difference = mousePoint - playerPos;
                difference.z = 0;

                angleDistance = Mathf.Min(3.0f, difference.magnitude);

                SetLineAngle();

                if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                    GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.AngleChange);
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                currentJumpAngle -= 1;
                SetLineAngle();
                if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                    GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.AngleNudge);
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                currentJumpAngle += 1;
                SetLineAngle();
                if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                    GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.AngleNudge);
                }
            }

            if (Input.GetKeyUp(KeyCode.Return)) {
                if (!clickedLastTick) {
                    currentJumpPower = uiHandler.currentWord.Length * 15f;
                    uiHandler.currentWord = "";
                    lr.positionCount = 0;

                    Jump();
                    winScreen.GetComponent<WinController>().jumpCount += 1;
                    clickedLastTick = true;
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

            if (!onFloor) {
                yVel -= stoppingForce * Time.deltaTime;
            } else {
                yVel = 0;
            }

            transform.localRotation = new Quaternion();

            GetComponent<Rigidbody2D>().velocity = new Vector2(xVel, yVel);

        } else {
            pausedVel = transform.GetComponent<Rigidbody2D>().velocity;
            transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    void SetLineAngle() {
        Vector3 playerPos = player.transform.position;

        // normalize to only get a direction with magnitude = 1
        var direction = Quaternion.AngleAxis(currentJumpAngle, transform.up) * transform.forward;

        direction.y = direction.z;
        direction.z = 0;

        // and finally apply the end position
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
        xVel = currentJumpPower * (currentJumpAngle / 90);
        yVel = currentJumpPower * (1-(Mathf.Abs(currentJumpAngle) / 90));
        transform.GetChild(0).gameObject.SetActive(false);

        if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
            GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.Jump);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Vector2 direction = collision.GetContact(0).normal;

        onFloor = false;

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
        } else if(Mathf.Round(direction.y) == -1) {
            //Above Player
            yVel *= -1;
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
