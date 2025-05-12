using UnityEngine;
using TMPro;
using UnityStandardAssets.Vehicles.Car;

public class CarSpeedUI : MonoBehaviour
{
    [Header("References")]
    public CarController carController; // 修改类名为 CarControllerNew
    public TMP_Text speedText;
    //public TMP_Text timeText;
    
    [Header("Settings")]
    public float updateInterval = 0.1f;

    private float timer = 0f;
    private float gameStartTime;
    private bool isTimerRunning = true;

    private void Start()
    {
        gameStartTime = Time.time;
        
        if (speedText == null) Debug.LogWarning("Speed Text is not assigned!");
        //if (timeText == null) Debug.LogWarning("Time Text is not assigned!");
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            UpdateSpeedDisplay();
            // UpdateTimeDisplay();
            timer = 0f;
        }
    }

    private void UpdateSpeedDisplay()
    {
        if (carController != null && speedText != null)
        {
            int speed = Mathf.RoundToInt(carController.CurrentSpeed*1.8f);

            speedText.text = $"Speed: {speed} km/h";
        }
    }

    // private void UpdateTimeDisplay()
    // {
    //     if (timeText != null && isTimerRunning)
    //     {
    //         float elapsedTime = Time.time - gameStartTime;
    //         int minutes = Mathf.FloorToInt(elapsedTime / 60f);
    //         int seconds = Mathf.FloorToInt(elapsedTime % 60f);
    //         timeText.text = $"time: {minutes:00}:{seconds:00}";
    //     }
    // }

    public void SetTimerRunning(bool running)
    {
        isTimerRunning = running;
    }
}