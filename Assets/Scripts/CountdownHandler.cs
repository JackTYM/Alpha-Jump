using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics; // for using Stopwatch class

public class CountdownHandler : MonoBehaviour
{
    public Stopwatch countdownTimer; // Stopwatch to keep track of the countdown time

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf) { // check if the gameObject the script is attached to is active
            if (countdownTimer.ElapsedMilliseconds/1000 > 3) { // check if the countdown time is greater than 3.5 seconds
                gameObject.SetActive(false); // deactivate the gameObject
                countdownTimer.Stop(); // stop the countdown timer

                GameObject winScreen = transform.parent.GetChild(1).gameObject; // get the win screen gameObject

                winScreen.GetComponent<WinController>().levelStopwatch = Stopwatch.StartNew(); //start a new stopwatch for the level time
                winScreen.GetComponent<WinController>().jumpCount = 0; // reset the jump count
                winScreen.GetComponent<WinController>().letterCount = 0; // reset the letter count
            } else if (countdownTimer.ElapsedMilliseconds/1000 > 2.5) { // check if the countdown time is greater than 3 seconds
                transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "GO!"; // change the text to "GO!"
            } else if (countdownTimer.ElapsedMilliseconds/1000 > 1.5) { // check if the countdown time is greater than 2 seconds
                transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "1"; // change the text to "1"
            } else if (countdownTimer.ElapsedMilliseconds/1000 > 0.5) { // check if the countdown time is greater than 1 second
                transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "2"; // change the text to "2"
            }
        }
    }

    public void startCountdown() { // public function to start the countdown
        transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "3"; // reset text to "1"
        countdownTimer = new Stopwatch(); // create a new stopwatch
        countdownTimer.Start(); // start the stopwatch
        gameObject.SetActive(true); // activate the gameObject
    }
}