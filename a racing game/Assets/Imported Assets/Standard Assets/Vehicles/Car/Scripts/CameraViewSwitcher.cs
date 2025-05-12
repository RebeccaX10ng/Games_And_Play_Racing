using UnityEngine;

public class CameraViewSwitcher : MonoBehaviour
{
    public Transform frontView;
    public Transform backView;
    public Transform mirrorView;
    public Transform cameraTransform; // 绑定主摄像机 Transform

    private enum ViewMode { Front, Back, Mirror }
    private ViewMode currentView = ViewMode.Front;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentView == ViewMode.Back)
                SetView(ViewMode.Front);
            else
                SetView(ViewMode.Back);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentView == ViewMode.Mirror)
                SetView(ViewMode.Front);
            else
                SetView(ViewMode.Mirror);
        }
    }

    void SetView(ViewMode view)
    {
        currentView = view;

        Transform targetView = frontView;
        switch (view)
        {
            case ViewMode.Front: targetView = frontView; break;
            case ViewMode.Back: targetView = backView; break;
            case ViewMode.Mirror: targetView = mirrorView; break;
        }

        cameraTransform.position = targetView.position;
        cameraTransform.rotation = targetView.rotation;
    }
}