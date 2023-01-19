using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Unity.Netcode;

public class MovementHandler : NetworkBehaviour
{

    // Public variables
    public float currentJumpPower = 0f; // current jump power of the player
    public float currentJumpAngle = 0f; // current jump angle of the player
    public float stoppingForce = 0.5f; // the force used to stop the player's movement
    public AudioClip hitBounce;
    public AudioClip slime;
    public AudioClip getKey;
    public AudioClip death;
    public AudioClip hitFloor;
    public AudioClip jump;
    public AudioClip win;

    // Private variables
    float xVel = 0; // x velocity of the player
    float yVel = 0; // y velocity of the player
    Vector2 pausedVel = Vector2.zero; // velocity of the player when the game is paused
    float angleDistance = 3.0f; // distance between the player and the angle of the line renderer
    bool clickedLastTick = false; // flag to check if the player clicked on the last frame
    bool onFloor = false; // flag to check if the player is on the floor
    bool stuck = false; // flag to check if the player is stuck
    GameObject player; // reference to the player game object
    GameObject countdown; // reference to the countdown game object
    GameObject pauseScreen; // reference to the pause screen game object
    GameObject winScreen; // reference to the win screen game object
    DictionaryController dictController; // reference to the dictionary controller
    UIHandler uiHandler; // reference to the UI handler
    LineRenderer lr; // reference to the line renderer component
    Stopwatch arrowHoldTimer = Stopwatch.StartNew(); // stopwatch to track the time the arrow is held down
    Stopwatch onFloorFailsafe = Stopwatch.StartNew(); // stopwatch for failsafe for checking if the player is on the floor
    float onFloorY = 0f; // the y coordinate
        List<GameObject> floorCollisions = new List<GameObject>(); // list of all floor game objects that the player is currently colliding with
    
    // Overrides the OnNetworkSpawn method to disable the line renderer if the player is not the owner
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) {
            transform.GetChild(0).gameObject.SetActive(false); // Disable the line renderer
            enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if ((IsOwner || (!IsHost && !IsClient))) {
            player = gameObject; // get reference to the player game object
            lr = GetComponent<LineRenderer>(); // get reference to the line renderer component

            // get references to the pause screen, win screen and countdown game objects
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "Pause Screen") {
                    pauseScreen = t.gameObject;
                } else if (t.name == "Win Screen") {
                    winScreen = t.gameObject;
                }
            }

            // get reference to the UI handler
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                if (t.name == "UI") {
                    uiHandler = t.gameObject.GetComponent<UIHandler>();
                }
            }

            onFloorY = transform.position.y; // set the onFloorY value to the player's starting y coordinate
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((IsOwner || (!IsHost && !IsClient))) {
            // get references to the pause screen, win screen, countdown screen and dictionary controller if they are not already assigned
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

            transform.localRotation = new Quaternion(); // reset player rotation

            // check if the game is not paused, not won, not in countdown and the player is enabled
            if(!pauseScreen.activeSelf && !winScreen.activeSelf && !countdown.activeSelf && GetComponent<SpriteRenderer>().enabled) {
                // Reset velocity when the game gets unpaused
                if (pausedVel != Vector2.zero) {
                    transform.GetComponent<Rigidbody2D>().velocity = pausedVel;
                    pausedVel = Vector2.zero;
                }

                // If left click is pressed, calculate the jump angle and draw a line
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

                // If it has been more than 100 milliseconds since the last arrow hold event
                if (arrowHoldTimer.ElapsedMilliseconds > 100) {
                    // Add 1 to the angle and redraw the line.
                    if (Input.GetKey(KeyCode.LeftArrow)) {
                        arrowHoldTimer = Stopwatch.StartNew();
                        currentJumpAngle += 1;
                        SetLineAngle();

                        // Trigger the arrow tutorial condition
                        if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                            GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.AngleNudge);
                        }
                    }

                    // Subtract  1 to the angle and redraw the line.
                    if (Input.GetKey(KeyCode.RightArrow)) {
                        arrowHoldTimer = Stopwatch.StartNew();
                        currentJumpAngle -= 1;
                        SetLineAngle();

                        // Trigger the arrow tutorial condition
                        if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                            GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.AngleNudge);
                        }
                    }
                }

                // If enter or space are pressed and the player is on the floor or glitch mode is on (enables flying)
                if ((Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space)) && (onFloor || dictController.modeIndex == 2)) {
                    // If the word is a valid dictionary word or if the difficulty is not hard mode
                    if (dictController.validWord(uiHandler.currentWord.ToLower()) || dictController.modeIndex != 1) {
                        // Adds to your stats and set the jump power then trigger the jump function
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
                        // Add error text when inputting an incorrect word
                        uiHandler.errorText.SetActive(true);
                        uiHandler.errorText.GetComponent<TMPro.TextMeshProUGUI>().text = uiHandler.currentWord.ToLower() + " is not a valid word!";
                        uiHandler.currentWord = "";
                    }
                } else {
                    clickedLastTick = false;
                }

                // Add horizontal air resistance to gradually slow the player
                if (xVel > 0) {
                    xVel -= stoppingForce * Time.deltaTime;
                    xVel = xVel < 0 ? 0 : xVel;
                } else {
                    xVel += stoppingForce * Time.deltaTime;
                    xVel = xVel > 0 ? 0 : xVel;
                }

                // Add gravity to gradually slow the player
                if (!stuck) {
                    yVel -= stoppingForce * Time.deltaTime;

                    if (yVel == 0) {
                        yVel = -0.01f;
                    }
                }

                // Set the onFloor variable to whether or not the player is on the floor
                if (onFloorFailsafe.ElapsedMilliseconds > 200) {
                    if (transform.position.y == onFloorY && !onFloor) {
                        onFloor = true;
                        yVel = 0;
                    }
                    onFloorFailsafe = Stopwatch.StartNew();
                    onFloorY = transform.position.y;
                }

                // Set the player velocity
                GetComponent<Rigidbody2D>().velocity = new Vector2(xVel, yVel);

            } else {
                // Save the velocity when the game is paused
                pausedVel = transform.GetComponent<Rigidbody2D>().velocity;
                transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }

    // Sets the angle text and draws a line between the player position and the mouse (Clamped to length of 0.9)
    void SetLineAngle() {
        if ((IsOwner || (!IsHost && !IsClient))) {
            // Get the player's current position
            Vector3 playerPos = player.transform.position;

            // Calculate the direction vector based on the current jump angle (converted to radians)
            var direction = new Vector3(Mathf.Cos(currentJumpAngle*(Mathf.PI/180)), Mathf.Sin(currentJumpAngle*(Mathf.PI/180)), 0f);

            // Calculate the end position of the line by adding the direction vector to the player's position
            var endPosition = playerPos + direction * angleDistance;

            // Set the line renderer's starting and ending color to black
            lr.startColor = Color.black;
            lr.endColor = Color.black;

            // Set the position count to 2, and set the positions to the player's position and the end position
            lr.positionCount = 2;
            lr.SetPositions(new Vector3[]{playerPos, endPosition});

            // Calculate the text position, clamped to a length of 0.9
            var textPosition = direction * 0.9f;

            // Set the text position's z value to 0
            textPosition = new Vector3(textPosition.x, textPosition.y, 0f);

            // Set the child object's local position to the text position
            transform.GetChild(0).localPosition = textPosition;
            
            // Set the text to the current jump angle
            transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = currentJumpAngle.ToString();

            // Enable the child object
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    // Triggers when player presses Enter or Space
    // Calculates degrees and uses that as a ratio for the velocity
    public void Jump() {
        if ((IsOwner || (!IsHost && !IsClient))) {
            // Calculate the degree angle of the jump by subtracting 90 from the current jump angle and then multiplying that value by -1
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
                if (!GetComponent<AudioSource>().isPlaying || (GetComponent<AudioSource>().clip.name != "Death" && GetComponent<AudioSource>().clip.name != "Win" && GetComponent<AudioSource>().clip.name != "Slime" && GetComponent<AudioSource>().clip.name != "GetKey")) {
                    GetComponent<AudioSource>().clip = jump;
                    GetComponent<AudioSource>().Play();
                }
            }

            // Start stopwatch for failsafe
            onFloorFailsafe = Stopwatch.StartNew();
            // Check if the current scene is a tutorial scene
            if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                // Trigger tutorial text
                GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.Jump);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if ((IsOwner || (!IsHost && !IsClient))) {
            // Get the contact normal of the collision
            Vector2 direction = collision.GetContact(0).normal;

            // Check if the collision object is tagged as "Kill"
            if (collision.gameObject.tag == "Kill") {
                GetComponent<AudioSource>().clip = death;
                GetComponent<AudioSource>().Play();
                // Move the player to the starting position
                transform.position = new Vector3(0,0,-0.49f);
                onFloor = false;
                xVel = 0;
                yVel = 0;
                // Check if the scene is "Map 3" and if so, activate the key door if it is inactive
                if (SceneManager.GetActiveScene().name.Contains("Map 3")) {
                    if (IsOwner) {
                        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>()) {
                            if ((t.name == "Key Door(Clone)" || t.name == "Key Door 1(Clone)") && t.GetComponent<NetworkObject>().IsOwner && !t.gameObject.activeSelf) {
                                t.gameObject.SetActive(true);
                            }
                        }
                    } else if ((!IsHost && !IsClient)) {
                        GameObject.Find("Surfaces").transform.Find("Key Door").gameObject.SetActive(true);
                    }
                }
            }
            // Check if the player is stuck to an object
            if (stuck) {
                return;
            }
            // Check if the collision object is tagged as "Sticky"
            if (collision.gameObject.tag == "Sticky") {
                xVel = 0;
                yVel = 0;
                // Set the player's velocity to 0
                transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                // Set the player as stuck
                stuck = true;
                if (!GetComponent<AudioSource>().isPlaying || (GetComponent<AudioSource>().clip.name != "Death" && GetComponent<AudioSource>().clip.name != "Win")) {
                    GetComponent<AudioSource>().clip = slime;
                    GetComponent<AudioSource>().Play();
                }
            }

            // Check the collision direction
            if(Mathf.Round(direction.x) != 0) {
                // Collision on the side of the player
                if (collision.gameObject.tag == "Bounce") {
                    xVel *= -1;
                    if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                        GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.WallBounce);
                    }
                } else {
                    xVel = 0;
                }
                if (!GetComponent<AudioSource>().isPlaying || (GetComponent<AudioSource>().clip.name != "Death" && GetComponent<AudioSource>().clip.name != "Win" && GetComponent<AudioSource>().clip.name != "Slime" && GetComponent<AudioSource>().clip.name != "GetKey")) {
                    GetComponent<AudioSource>().clip = hitBounce;
                    GetComponent<AudioSource>().Play();
                }
            } else if(Mathf.Round(direction.y) == 1) {
                // Collision under the player
                if (collision.gameObject.tag == "Finish") {
                    if ((IsOwner || (!IsHost && !IsClient))) {
                        if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                            GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.Finish);
                        }
                        winScreen.GetComponent<WinController>().EndLevel();

                        GetComponent<AudioSource>().clip = win;
                        GetComponent<AudioSource>().Play();
                    } else {
                        // Check if the player has lost
                        foreach (WinController win in Resources.FindObjectsOfTypeAll<WinController>()) {
                            win.LoseLevel(transform.gameObject.GetComponent<PlayerNetwork>().timeForLevel.Value, 
                                transform.gameObject.GetComponent<PlayerNetwork>().jumpCount.Value);
                        }
                    }
                } else {
                    if (!GetComponent<AudioSource>().isPlaying || (GetComponent<AudioSource>().clip.name != "Death" && GetComponent<AudioSource>().clip.name != "Win" && GetComponent<AudioSource>().clip.name != "Slime" && GetComponent<AudioSource>().clip.name != "GetKey")) {
                        GetComponent<AudioSource>().clip = hitFloor;
                        GetComponent<AudioSource>().Play();
                    }
                }
                yVel = 0;
                onFloor = true;
                floorCollisions.Add(collision.gameObject);
            } else if(Mathf.Round(direction.y) == -1) {
                //Above Player
                yVel *= -1;
                if (!GetComponent<AudioSource>().isPlaying || (GetComponent<AudioSource>().clip.name != "Death" && GetComponent<AudioSource>().clip.name != "Win" && GetComponent<AudioSource>().clip.name != "Slime" && GetComponent<AudioSource>().clip.name != "GetKey")) {
                    GetComponent<AudioSource>().clip = hitBounce;
                    GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    // When the player exits a collision (Used to determine if the player is on the floor)
    void OnCollisionExit2D(Collision2D collision) {
        if ((IsOwner || (!IsHost && !IsClient))) {
            // Check if the collision object was in the floorCollisions list
            if (floorCollisions.Contains(collision.gameObject)) {
                // If so, set onFloor to false
                onFloor = false;

                // Remove the collision object from the floorCollisions list
                floorCollisions.Remove(collision.gameObject);
            }
        }
    }

    // Called when the player touches a key
    void OnTriggerEnter2D(Collider2D col) {
        if ((IsOwner || (!IsHost && !IsClient))) {
            // Check if the player is the owner and the parent object of the collider is also the owner
            if ((IsOwner || (!IsHost && !IsClient)) && col.transform.parent.GetComponent<NetworkObject>().IsOwner && col.IsTouching(transform.GetComponent<CircleCollider2D>())) {
                // If so, deactivate the parent object
                col.transform.parent.gameObject.SetActive(false);

                GetComponent<AudioSource>().clip = getKey;
                GetComponent<AudioSource>().Play();

                // Check if the scene is "Tutorial" and if so, trigger the "GrabKey" condition
                if (SceneManager.GetActiveScene().name.Contains("Tutorial")) {
                    GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTextHandler>().TriggerCondition(TutorialTextHandler.passConditions.GrabKey);
                }
            }
        }
    }
}
