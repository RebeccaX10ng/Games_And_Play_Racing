using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    [Header("Race Settings")]
    public int totalLaps = 3;
    public int totalCheckpoints = 2;

    [Header("UI References")]
    public TMP_Text lapText;
    public TMP_Text checkpointText;
    public GameObject raceCompleteUI;
    public TMP_Text resultText; // 用于显示胜负结果

    private bool playerFinished = false;
    private bool opponentFinished = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        Debug.Log($"绑定的LapText ID: {lapText?.GetInstanceID()}");
        Debug.Log($"场景中可见文本的ID: {GameObject.Find("lap")?.GetComponent<TMP_Text>()?.GetInstanceID()}");
    }

    public void OnLapCompleted(int currentLap)
    {
        if (lapText != null)
        {
            lapText.text = $"Lap: {currentLap}/{totalLaps}";
        }

        if (currentLap >= totalLaps)
        {
            PlayerFinished();
        }
    }

    public void OnCheckpointPassed(int checkpointNumber)
    {
        if (checkpointText != null)
        {
            checkpointText.text = $"Checkpoint: {checkpointNumber}/{totalCheckpoints}";
        }
    }

    private void PlayerFinished()
    {
        if (playerFinished) return;
        playerFinished = true;

        Debug.Log("玩家完成比赛");

        if (opponentFinished)
        {
            ShowResult("You Win! 🎉");
        }
        else
        {
            // 先完成，等待对手
            StartCoroutine(WaitForOpponent());
        }
    }

    public void OpponentFinished()
    {
        if (opponentFinished) return;
        opponentFinished = true;

        Debug.Log("对手完成比赛");

        if (playerFinished)
        {
            ShowResult("You Lose 😢");
        }
        else
        {
            // 对手先完成，等待玩家
            StartCoroutine(WaitForPlayer());
        }
    }

    private System.Collections.IEnumerator WaitForOpponent()
    {
        float waitTime = 2f; // 可选：等待2秒对手
        yield return new WaitForSeconds(waitTime);

        if (!opponentFinished)
        {
            ShowResult("You Win! 🎉");
        }
    }

    private System.Collections.IEnumerator WaitForPlayer()
    {
        float waitTime = 2f;
        yield return new WaitForSeconds(waitTime);

        if (!playerFinished)
        {
            ShowResult("You Lose 😢");
        }
    }

    private void ShowResult(string result)
    {
        Time.timeScale = 0f;
        if (raceCompleteUI != null)
        {
            raceCompleteUI.SetActive(true);
        }
        if (resultText != null)
        {
            resultText.text = result;
        }
        Debug.Log(result);
    }
}
