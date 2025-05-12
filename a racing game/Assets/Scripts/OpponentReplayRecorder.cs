using UnityEngine;
using System.Collections.Generic;

public class OpponentReplayRecorder : MonoBehaviour
{
    public bool isRecording = true;
    public List<TransformData> recordedData = new List<TransformData>();

    void FixedUpdate()
    {
        if (isRecording)
        {
            recordedData.Add(new TransformData(transform.position, transform.rotation));
        }
    }

    public void ClearReplayData()
    {
        recordedData.Clear();
    }
}