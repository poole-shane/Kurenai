using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMain : MonoBehaviour
{
    private const int CARDS_FLIPPABLE_AMOUNT = 2;
    private const int MATCH_POINTS = 100, COMBO_POINTS = 50, SUPER_COMBO_POINTS = 100;

    private int _cardsAmount = 8, _cardsSolvedAmount = 0;
    private int _turns = 0, _score = 0, _consecutiveMatchCnt = 0;

    private List<GameCard> _cards = new List<GameCard>();
    private List<GameCard.EntityParams> _checkingEntities = new List<GameCard.EntityParams>();

    public TableLayout Layout;
    public RectTransform Field;
    public GameObject CardPrefab;

    public TextMeshProUGUI Matches;
    public TextMeshProUGUI Turns;
    public TextMeshProUGUI Score;

    public EndCard EndCard;

    public GameSoundController SoundController;

    // Start is called before the first frame update
    void Start()
    {
        // When testing the game scene on its own
        if(MainManager.Instance == null)
        {
            StartNew();
            PlaceCards();
            return;
        }

        Layout = (TableLayout)MainManager.Instance.SessionData.LayoutType;

        if (MainManager.Instance.SaveDataAvailable)
        {
            Resume();
        }
        else
        {
            StartNew();
        }
        PlaceCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetCardsAmount()
    {
        switch (Layout)
        {
            default:
            case TableLayout.TwoByTwo:
                _cardsAmount = GlobalConstants.TWO_BY_TWO_CARDS_AMOUNT;
                break;
            case TableLayout.ThreeByTwo:
                _cardsAmount = GlobalConstants.THREE_BY_TWO_CARDS_AMOUNT;
                break;
            case TableLayout.FourByThree:
                _cardsAmount = GlobalConstants.FOUR_BY_THREE_CARDS_AMOUNT;
                break;
            case TableLayout.SixByFive:
                _cardsAmount = GlobalConstants.SIX_BY_FIVE_CARDS_AMOUNT;
                break;
        }
    }

    private void StartNew()
    {
        SetCardsAmount();

        int cnt = 0;
        int halfAmount = (int)(_cardsAmount * .5f);
        for (int i = 0; i < _cardsAmount; i++)
        {
            GameCard.EntityParams entity = new GameCard.EntityParams();
            entity.Index = i;
            entity.Solved = false;

            if (i >= halfAmount)
                entity.Type = (CardType)(i - halfAmount);
            else
                entity.Type = (CardType)i;

            var card = Instantiate(CardPrefab, Field);
            GameCard cardInfo = card.GetComponent<GameCard>();
            cardInfo.Player = this;
            cardInfo.SetEntity(entity);

            _cards.Add(cardInfo);

            cnt++;
        }

        // Shuffle cards using the Fisher-Yates shuffle algorithm.
        int n = _cards.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            GameCard randomCard = _cards[k];
            _cards[k] = _cards[n];
            _cards[n] = randomCard;
        }
    }

    private void Resume()
    {
        SetCardsAmount();

        _turns = MainManager.Instance.SessionData.Turns;
        _score = MainManager.Instance.SessionData.Score;

        // Create cards and insert them at loaded positions
        int[] indexArray = MainManager.Instance.SessionData.CardIndexes;
        bool[] solvedArray = MainManager.Instance.SessionData.CardStates;
        int cnt = 0;
        int halfAmount = (int)(_cardsAmount * .5f);
        for (int i = 0; i < _cardsAmount; i++)
        {
            GameCard.EntityParams entity = new GameCard.EntityParams();
            entity.Index = indexArray[i];
            entity.Solved = solvedArray[i];
            _cardsSolvedAmount += entity.Solved ? 1 : 0;

            int indexNo = indexArray[i];
            if (indexNo >= halfAmount)
                entity.Type = (CardType)(indexNo - halfAmount);
            else
                entity.Type = (CardType)indexNo;

            var card = Instantiate(CardPrefab, Field);
            GameCard cardInfo = card.GetComponent<GameCard>();
            cardInfo.Player = this;
            cardInfo.SetEntity(entity);

            _cards.Add(cardInfo);

            cnt++;
        }

        // Update info text
        UpdateText();
    }

    private void PlaceCards()
    {
        // Set up cards
        RectTransform rect = _cards[0].transform as RectTransform;
        Vector2 originalCardSize = new Vector2(rect.rect.width, rect.rect.height);

        Vector2 fieldSize = new Vector2(Field.rect.width, Field.rect.height);

        int cardsPerRow = 0;
        switch (Layout)
        {
            default:
            case TableLayout.TwoByTwo:
                cardsPerRow = 2;
                break;
            case TableLayout.ThreeByTwo:
                cardsPerRow = 3;
                break;
            case TableLayout.FourByThree:
                cardsPerRow = 4;
                break;
            case TableLayout.SixByFive:
                cardsPerRow = 6;
                break;
        }
        int cardsPerColumn = _cards.Count / cardsPerRow;

        int cardCnt = 0;
        int rowCnt = 0;

        float spacing = 40f;
        float addX = 0, addY = 0;

        Vector2 spaceForCards = new Vector2(fieldSize.x - (spacing * cardsPerRow), fieldSize.y - (spacing * cardsPerColumn));
        float newScale = spaceForCards.y / cardsPerColumn / originalCardSize.y;
        Vector2 newCardSize = new Vector2(originalCardSize.x * newScale, originalCardSize.y * newScale);

        // Position and scale cards accordingly
        for(int i = 0; i < _cards.Count; i++)
        {
            _cards[i].transform.localPosition = new Vector3((newCardSize.x * .5f) + addX, -(newCardSize.y * .5f) - addY);
            _cards[i].transform.localScale = new Vector3(newScale, newScale, 1f);

            addX += newCardSize.x + spacing;
            cardCnt++;
            if(cardCnt == cardsPerRow)
            {
                cardCnt = 0;
                addX = 0;
                addY += newCardSize.y + spacing;
                rowCnt++;
            }
        }
    }

    public void CardTapped(GameCard.EntityParams entity)
    {
        if(_checkingEntities.Count < CARDS_FLIPPABLE_AMOUNT)
        {
            _checkingEntities.Add(entity);
        }

        if(_checkingEntities.Count == CARDS_FLIPPABLE_AMOUNT)
        {
            if (_checkingEntities[0].Type == _checkingEntities[1].Type)
            {
                _cardsSolvedAmount += 2;

                foreach (var solvedEntity in _checkingEntities)
                {
                    foreach (var card in _cards)
                    {
                        if (card.Entity == solvedEntity)
                        {
                            card.Solve();
                            break;
                        }
                    }
                }

                SoundController.PlayAudio(GameSoundController.AudioType.Match);
                _score += MATCH_POINTS;

                // Give the player extra points with consecutive matches
                _consecutiveMatchCnt++;
                if (_consecutiveMatchCnt >= 3)
                    _score += SUPER_COMBO_POINTS;
                else if (_consecutiveMatchCnt > 1)
                    _score += COMBO_POINTS;
            }
            else
            {
                foreach (var solvedEntity in _checkingEntities)
                {
                    foreach (var card in _cards)
                    {
                        if (card.Entity == solvedEntity)
                        {
                            card.FlipToBack();
                            break;
                        }
                    }
                }
                SoundController.PlayAudio(GameSoundController.AudioType.Mismatch);
                _consecutiveMatchCnt = 0;
            }
            _checkingEntities.Clear();

            _turns++;

            UpdateText();

            if (_cardsSolvedAmount == _cardsAmount)
            {
                SoundController.PlayAudio(GameSoundController.AudioType.Victory);
                GameEnd();
            }
        }
    }

    private void UpdateText()
    {
        Turns.text = _turns.ToString();
        Score.text = _score.ToString();
        Matches.text = ((int)(_cardsSolvedAmount * .5f)).ToString();
    }

    public void GameEnd()
    {
        EndCard.EntityParams entity = new EndCard.EntityParams();
        entity.TurnsAmount = _turns;
        entity.FinalScore = _score;

        EndCard.SetEntity(entity);
        EndCard.Activate();
    }

    /// <summary>
    /// Save and quit the game. The player can resume again from
    /// the start menu.
    /// </summary>
    public void SaveAndQuit()
    {
        if(MainManager.Instance == null)
        {
            SceneManager.LoadScene(GlobalConstants.START_MENU);
            return;
        }

        MainManager.Instance.SessionData.Turns = _turns;
        MainManager.Instance.SessionData.Score = _score;
        
        // Save card indexes for replacing them in order when the game is loaded
        for(int i = 0; i < _cardsAmount; i++)
        {
            MainManager.Instance.SessionData.CardIndexes[i] = _cards[i].Entity.Index;
            MainManager.Instance.SessionData.CardStates[i] = _cards[i].Entity.Solved;
        }

        MainManager.Instance.SaveGame();

        SceneManager.LoadScene(GlobalConstants.START_MENU);
    }
}
