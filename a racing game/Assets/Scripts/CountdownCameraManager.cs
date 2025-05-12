using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownCameraManager : MonoBehaviour
{
    public AudioListener countdownListener;  
    public AudioListener carListener;  
    [Header("Cameras & Targets")]
    public Camera[] previewCameras;                 
    public Transform[] rotationTargets;              
    public Camera mainCamera;                       
    private int currentPreviewIndex = -1;

    [Header("Countdown UI")]
    public GameObject countdownPanel;
    public TMP_Text countdownText;

    [Header("Player & Timer")]
    public GameObject playerCar;

    public GameObject enemyCar;
    public TimerManager timerManager;

    [Header("Audio")]
    public AudioClip countdownBeep;
    public AudioClip goSound;
    private AudioSource audioSource;

    [Header("Scale Animation")]
    public float scaleUpSize = 1.5f;
    public float scaleDuration = 0.2f;
    private Coroutine scaleCoroutine;

    [Header("Camera Rotation")]
    public float rotationSpeed = 20f;
    private bool isRotating = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        playerCar.SetActive(false);
        enemyCar.SetActive(false);
        mainCamera.enabled = false;
        StartCoroutine(CountdownSequence());
        countdownListener.enabled = true;
        carListener.enabled = false;

    }

    void Update()
    {
        if (isRotating && currentPreviewIndex >= 0 && currentPreviewIndex < previewCameras.Length)
        {
            Camera cam = previewCameras[currentPreviewIndex];
            Transform target = rotationTargets[currentPreviewIndex];
            if (cam != null && target != null)
            {
                cam.transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);
                cam.transform.LookAt(target);
            }
        }
    }

    IEnumerator CountdownSequence()
    {
        countdownPanel.SetActive(true);

        for (int i = 0; i < previewCameras.Length; i++)
        {
            currentPreviewIndex = i;
            ActivateOnlyCamera(previewCameras[i]);

            countdownText.text = (3 - i).ToString();
            PlaySound(countdownBeep);

            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
            scaleCoroutine = StartCoroutine(AnimateCountdownText());

            isRotating = true;
            yield return new WaitForSeconds(1f);
            isRotating = false;
        }

        countdownText.text = "GO!";
        PlaySound(goSound);

        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(AnimateCountdownText());

        yield return new WaitForSeconds(1f);

        countdownPanel.SetActive(false);
        ActivateOnlyCamera(mainCamera);
        currentPreviewIndex = -1;

        playerCar.SetActive(true);
        enemyCar.SetActive(true);
        if (timerManager != null)
            timerManager.StartTimer();
        countdownListener.enabled = false;
        carListener.enabled = true;

    }

    void ActivateOnlyCamera(Camera camToActivate)
    {
        foreach (Camera cam in previewCameras)
            cam.enabled = (cam == camToActivate);
        mainCamera.enabled = (camToActivate == mainCamera);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    IEnumerator AnimateCountdownText()
    {
        RectTransform rt = countdownText.GetComponent<RectTransform>();
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = originalScale * scaleUpSize;

        float t = 0f;
        while (t < scaleDuration)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(originalScale, targetScale, t / scaleDuration);
            yield return null;
        }

        t = 0f;
        while (t < scaleDuration)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(targetScale, originalScale, t / scaleDuration);
            yield return null;
        }

        rt.localScale = originalScale;
    }
}
