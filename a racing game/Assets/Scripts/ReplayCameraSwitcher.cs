using UnityEngine;

public class ReplayCameraSwitcher : MonoBehaviour
{
    public Camera[] cameras;
    private int currentIndex = 0;

    void Start()
    {
        SwitchToCamera(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToCamera(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToCamera(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchToCamera(2);
        if (Input.GetKeyDown(KeyCode.Q)) SwitchToCamera(3); // Rear
        if (Input.GetKeyDown(KeyCode.E)) SwitchToCamera(4); // Hood
    }

    void SwitchToCamera(int index)
    {
        currentIndex = index;
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == index);
        }
    }
}