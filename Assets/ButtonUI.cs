using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonUI : MonoBehaviour
{
    public GameObject ResumeButton;

    private void Start()
    {
        ResumeButton.SetActive(MainManager.Instance.SaveDataAvailable);
    }

    public void StartNewGame(int layout)
    {
        MainManager.Instance.DeleteSaveData();
        MainManager.Instance.SessionData.LayoutType = layout;
        SceneManager.LoadScene(GlobalConstants.GAME_SCENE);
    }

    public void ResumeGame()
    {
        SceneManager.LoadScene(GlobalConstants.GAME_SCENE);
    }

    public void QuitToDesktop()
    {
        UnityEngine.Application.Quit();
    }
}
