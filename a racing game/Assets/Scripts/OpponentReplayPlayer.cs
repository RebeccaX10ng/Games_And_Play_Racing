using UnityEngine;

public class OpponentReplayPlayer : MonoBehaviour
{
    public OpponentReplayRecorder recorder;
    public float playbackSpeed = 1f;
    private int currentIndex = 0;
    private bool isReplaying = false;
    public GameObject endGamePanel;
    public void StartReplay()
    {
        currentIndex = 0;
        isReplaying = true;
        recorder.isRecording = false;
        Debug.Log("replay opponent ");
        if (recorder.recordedData.Count > 0)
        {
            Debug.Log("moved car");
            // 立刻把车移动到起点
            transform.position = recorder.recordedData[0].position;
            transform.rotation = recorder.recordedData[0].rotation;
        }
        else
        {
            Debug.Log("no replay recorded");
        }
    }

    void FixedUpdate()
    {
        if (!isReplaying || recorder.recordedData.Count == 0) return;
        if (endGamePanel != null)
            endGamePanel.SetActive(false);
        if (currentIndex < recorder.recordedData.Count)
        {
            var data = recorder.recordedData[currentIndex];
            transform.position = data.position;
            transform.rotation = data.rotation;
            currentIndex++;
        }
        else
        {
            isReplaying = false;
        }
    }
}