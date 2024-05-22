using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndCard : MonoBehaviour
{
    [SerializeField]
    private string _menuScene = "StartMenu";

    [SerializeField]
    public class EntityParams
    {
        public int TurnsAmount;
        public int FinalScore;
    }

    public EntityParams Entity;
    public TextMeshProUGUI Turns;
    public TextMeshProUGUI Score;

    public void SetEntity(EntityParams entity)
    {
        Entity = entity;

        Turns.text = Entity.TurnsAmount.ToString();
        Score.text = Entity.FinalScore.ToString();
    }

    public void BackToStart()
    {
        if(MainManager.Instance != null)
        {
            MainManager.Instance.DeleteSaveData();
        }

        SceneManager.LoadScene(_menuScene);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
