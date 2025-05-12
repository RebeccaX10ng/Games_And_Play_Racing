using TMPro;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class LapTracker : MonoBehaviour
{
    public int currentCheckpoint = 0;
    public int currentLap = 0;
    public int totalLaps = 3;
    private bool raceFinished = false;
    public GameObject victorySound;
    
    [Header("UI")]
    public TMP_Text resultText;  // 拖入你创建的 ResultText

    
    private CheckpointManager checkpointManager;

    [Header("Reset Settings")]
    public Transform startPoint;
    public GameObject carObject;
    [Header("Opponent")]
    public GameObject opponentCar;
    public OpponentLapTracker opponentTracker;  // 关联对手脚本
    private bool hasDeclaredWinner = false;

    private void Start()
    {
        TimerManager.instance.StartTimer();
        checkpointManager = FindObjectOfType<CheckpointManager>();

        if (carObject == null)
            carObject = this.gameObject;
    }

    public void ReachCheckpoint(int index)
    {
        if (raceFinished) return;

        if (index == currentCheckpoint)
        {
            currentCheckpoint++;

            if (currentCheckpoint >= checkpointManager.checkpoints.Count)
            {
                currentCheckpoint = 0;
                currentLap++;
                Debug.Log("玩家完成圈数：" + currentLap);

                if (currentLap >= totalLaps)
                {
                    raceFinished = true;
                    Debug.Log("玩家完成比赛！");
                    TimerManager.instance.StopTimerAndShowResult();

                    CheckWinner(isPlayer: true);
                }
            }
        }
    }

    public void CheckWinner(bool isPlayer)
    {
        if (hasDeclaredWinner) return;
        
        

        hasDeclaredWinner = true;

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);

            if (isPlayer)
            {
                resultText.text = "YOU WIN!";
                Debug.Log("玩家赢得比赛！");
                victorySound.SetActive(true);
            }
            else
            {
                resultText.gameObject.SetActive(true);

                resultText.text = "YOU LOSE!";
                Debug.Log("对手赢得比赛！");
            }
        }
    }


    public void ResetToStart()
    {
        if (startPoint == null || carObject == null)
        {
            Debug.LogWarning("未设置起点或车辆！");
            return;
        }

        carObject.transform.position = startPoint.position;
        carObject.transform.rotation = startPoint.rotation;

        Rigidbody rb = carObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        currentCheckpoint = 0;
        Debug.Log("玩家重置完成！");
    }

    // 被对手调用
    public void OpponentFinished()
    {
        if (!raceFinished)
        {
            Debug.Log("对手完成比赛，但玩家未完成！");
            CheckWinner(isPlayer: false);
            
        }
    }
}
