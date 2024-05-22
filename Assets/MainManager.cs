using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public class PlayerData
    {
        public int LayoutType;
        public int Turns;
        public int Score;
        public int[] CardIndexes;
        public bool[] CardStates;
    }

    private bool _dataLoaded = false;
    private string _saveFilePath;

    public PlayerData SessionData;

    public bool SaveDataAvailable => Instance._dataLoaded;

    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _saveFilePath = Application.persistentDataPath + "/SessionData.json";

        if (!LoadGame())
        {
            SessionData = new PlayerData();
            Initialize();
        }
    }

    public void SaveGame()
    {
        string saveSessionData = JsonUtility.ToJson(SessionData);
        File.WriteAllText(_saveFilePath, saveSessionData);

        Debug.Log("Data saved at: " + _saveFilePath);

    }

    public bool LoadGame()
    {
        if (File.Exists(_saveFilePath))
        {
            string loadSessionData = File.ReadAllText(_saveFilePath);
            SessionData = JsonUtility.FromJson<PlayerData>(loadSessionData);

            Instance._dataLoaded = true;
            Debug.Log("Data loading completed.");

            return true;
        }

        Instance._dataLoaded = false;
        Debug.Log("No saved data detected.");
        return false;
    }

    public void DeleteSaveData()
    {
        if (File.Exists(_saveFilePath))
        {
            File.Delete(_saveFilePath);

            Debug.Log("Saved data deleted.");
        }
        else
            Debug.Log("There is no save data to delete.");

        Instance._dataLoaded = false;
    }

    public void Initialize()
    {
        SessionData.LayoutType = (int)TableLayout.TwoByTwo;
        SessionData.Turns = 0;
        SessionData.Score = 0;
        SessionData.CardIndexes = new int[GlobalConstants.SIX_BY_FIVE_CARDS_AMOUNT];
        SessionData.CardStates = new bool[GlobalConstants.SIX_BY_FIVE_CARDS_AMOUNT];
    }
}
