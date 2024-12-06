using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    float elapsedTime;
    public bool timerStop = false;
    public PlayerInventory playerInventory;
    public KeyCode RestartKey = KeyCode.R;

    // Update is called once per frame
    void Update()
    {
        if (playerInventory.NumCollected == 5) {
            timerStop = true;
            timerText.text = "Final time: " + elapsedTime.ToString("F3") + ". R to restart";
            if (Input.GetKeyDown(RestartKey)) {
                SceneManager.LoadScene("CityScene");
            }
        }

        if (!timerStop) {
            elapsedTime += Time.deltaTime;
            timerText.text = elapsedTime.ToString("F3");
        }
    }
}
