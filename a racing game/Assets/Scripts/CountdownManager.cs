using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using TMPro;

public class CountdownManager : MonoBehaviour
{
    public TMP_Text countdownText;
    public GameObject countdownPanel; // 包住 Text 的 UI 面板
    public GameObject enemyCar;
    public GameObject playerCar; // 玩家赛车

    void Start()
    {
        //playerCar.GetComponent<CarController>().enabled = false;
        playerCar.SetActive(false); // 开始时禁用玩家车
        enemyCar.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        countdownPanel.SetActive(true);

        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        countdownPanel.SetActive(false);
        //playerCar.GetComponent<CarController>().enabled = true;
        playerCar.SetActive(true); // 启用玩家车
        enemyCar.SetActive(true);
        TimerManager.instance.StartTimer(); // 开始计时器
    }
}