using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject endGamePanel;
    public GameObject replayPanel;
    public GameObject activeDuringGamePanel;

    [Header("Replay Scripts")]
    public CarReplayPlayer replayPlayer;            
    public CarReplayPlayer opponentReplayPlayer;     

    [Header("Race Logic")]
    public LapTracker lapTracker;                    
    public OpponentLapTracker opponentTracker;      

    [Header("Replay Camera")]
    public ReplayCameraSwitcher replayCameraSwitcher;

    public void OnReplayButtonClicked()
    {
       
        if (endGamePanel != null)
            endGamePanel.SetActive(false);
        
        if (replayPanel != null)
            replayPanel.SetActive(true);
        
        if (activeDuringGamePanel != null)
            activeDuringGamePanel.SetActive(false);
        
        if (lapTracker != null)
            lapTracker.enabled = false;

        if (opponentTracker != null)
            opponentTracker.enabled = false;
        
        if (replayPlayer != null)
            replayPlayer.StartReplay();
        
        if (opponentReplayPlayer != null)
            opponentReplayPlayer.StartReplay();
        
        if (replayCameraSwitcher != null)
            replayCameraSwitcher.enabled = true;
    }
}