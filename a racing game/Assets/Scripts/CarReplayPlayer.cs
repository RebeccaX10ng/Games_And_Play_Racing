using UnityEngine;
using System.Collections;

public class CarReplayPlayer : MonoBehaviour
{
    public CarReplayRecorder recorder;
    public float playbackSpeed = 1f;
    private int currentIndex = 0;
    private bool isReplaying = false;
    public void StartReplay()
    {
        currentIndex = 0;
        isReplaying = true;
        recorder.isRecording = false;
    }

    void FixedUpdate()
    {
        if (!isReplaying || recorder.recordedData.Count == 0) return;

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