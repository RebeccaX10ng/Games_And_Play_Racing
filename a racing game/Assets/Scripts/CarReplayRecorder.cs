using UnityEngine;
using System.Collections.Generic;

public class CarReplayRecorder : MonoBehaviour
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

[System.Serializable]
public struct TransformData
{
    public Vector3 position;
    public Quaternion rotation;

    public TransformData(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }
}