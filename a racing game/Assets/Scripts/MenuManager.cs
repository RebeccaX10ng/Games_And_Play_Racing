using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private string selectedCar = "A";
    private string selectedMap = "A";

    public SelectionImageScaler carAImage;
    public SelectionImageScaler carBImage;
    public SelectionImageScaler mapAImage;
    public SelectionImageScaler mapBImage;

    public void SelectCarA()
    {
        selectedCar = "A";
        carAImage.SetSelected(true);
        carBImage.SetSelected(false);
    }

    public void SelectCarB()
    {
        selectedCar = "B";
        carAImage.SetSelected(false);
        carBImage.SetSelected(true);
    }

    public void SelectMapA()
    {
        selectedMap = "A";
        mapAImage.SetSelected(true);
        mapBImage.SetSelected(false);
    }

    public void SelectMapB()
    {
        selectedMap = "B";
        mapAImage.SetSelected(false);
        mapBImage.SetSelected(true);
    }

    public void StartGame()
    {
        string sceneName = $"Scene_Car{selectedCar}_Map{selectedMap}";
        Debug.Log($"加载场景：{sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}