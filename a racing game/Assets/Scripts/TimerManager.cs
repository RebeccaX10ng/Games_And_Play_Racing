using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TMP_Text timerText;
    public GameObject resultPanel;
    public TMP_Text resultText;

    private float timer = 0f;
    private bool isTiming = false;

    public static TimerManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (isTiming)
        {
            timer += Time.deltaTime;
            timerText.text = "Time: " + timer.ToString("F2") + "s";
        }
    }

    public void StartTimer()
    {
        timer = 0f;
        isTiming = true;
    }

    public void StopTimerAndShowResult()
    {
        isTiming = false;
        resultPanel.SetActive(true);
        resultText.text = "Finish Time " + timer.ToString("F2") + "s";
    }
}