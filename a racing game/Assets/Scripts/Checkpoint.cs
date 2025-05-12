using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;  


    private void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;
        Debug.Log("Entered checkpoint trigger: " + root.name);

        // 玩家
        if (root.CompareTag("Player"))
        {
            Debug.Log("Player entered checkpoint.");

            LapTracker tracker = root.GetComponent<LapTracker>();
            if (tracker != null)
            {
                tracker.ReachCheckpoint(checkpointIndex);
            }
            else
            {
                Debug.LogWarning("LapTracker not found on root of " + root.name);
            }
        }

        // 对手
        else if (root.CompareTag("Opponent"))
        {
            Debug.Log("Opponent entered checkpoint.");

            OpponentLapTracker opponentTracker = root.GetComponent<OpponentLapTracker>();
            if (opponentTracker != null)
            {
                opponentTracker.ReachCheckpoint(checkpointIndex);
            }
            else
            {
                Debug.LogWarning("OpponentLapTracker not found on root of " + root.name);
            }
        }
    }
}