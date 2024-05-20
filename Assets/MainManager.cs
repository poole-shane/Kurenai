using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    private const string LAYOUT_LABEL = "Table Layout";

    public static MainManager Instance;

    public TableLayout LayoutValue;

    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt(LAYOUT_LABEL, (int)LayoutValue);
        PlayerPrefs.Save();
        Debug.Log("Game data saved!");
    }

    public void LoadGame()
    {
        if(PlayerPrefs.HasKey(LAYOUT_LABEL))
        {
            LayoutValue = (TableLayout)PlayerPrefs.GetInt(LAYOUT_LABEL);
        }
    }
}
