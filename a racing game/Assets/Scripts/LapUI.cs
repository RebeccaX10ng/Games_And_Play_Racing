using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LapUI : MonoBehaviour
{
    public LapTracker lapTracker;
    public TMP_Text lapText;

    void Update()
    {
        lapText.text = "Lap: " + (lapTracker.currentLap + 1) + " / " + lapTracker.totalLaps;
    }
}