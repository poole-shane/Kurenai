using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonUI : MonoBehaviour
{
    [SerializeField]
    private string _newGameLevel = "GameScene";

    public void StartNewGame(int layout)
    {
        MainManager.Instance.LayoutValue = (TableLayout)layout;
        SceneManager.LoadScene(_newGameLevel);
    }
}
