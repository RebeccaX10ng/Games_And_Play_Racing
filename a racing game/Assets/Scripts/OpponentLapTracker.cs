using UnityEngine;

public class OpponentLapTracker : MonoBehaviour
{
    public int currentCheckpoint = 0;
    public int currentLap = 0;
    public int totalLaps = 3;
    private bool raceFinished = false;

    private CheckpointManager checkpointManager;
    public LapTracker playerTracker;  // 💡 拖入玩家对象

    private void Start()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
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

                Debug.Log("对手完成圈数：" + currentLap);

                if (currentLap >= totalLaps)
                {
                    raceFinished = true;
                    //GetComponent<OpponentReplayRecorder>().isRecording=false;

                    Debug.Log("对手完成比赛！");
                    TimerManager.instance.StopTimerAndShowResult();
                    // ✅ 正确通知玩家脚本判断谁赢
                    playerTracker.OpponentFinished();
                }
            }
        }
    }
}