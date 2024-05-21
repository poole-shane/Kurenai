using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    private const int CARDS_FLIPPABLE_AMOUNT = 2;
    private const int MATCH_POINTS = 100, COMBO_POINTS = 50, SUPER_COMBO_POINTS = 100;

    private int _cardsAmount = 8, _cardsSolvedAmount = 0;
    private int _turns = 0, _score = 0, _consecutiveMatchCnt = 0;

    private List<GameCard> _cards = new List<GameCard>();
    private List<GameCard.EntityParams> _entities = new List<GameCard.EntityParams>();

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
        if(MainManager.Instance != null)
        {
            Layout = MainManager.Instance.LayoutValue;
        }
        Initialize();
        PlaceCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        switch (Layout)
        {
            default:
            case TableLayout.TwoByTwo:
                _cardsAmount = 4;
                break;
            case TableLayout.ThreeByTwo:
                _cardsAmount = 6;
                break;
            case TableLayout.FourByThree:
                _cardsAmount = 12;
                break;
            case TableLayout.SixByFive:
                _cardsAmount = 30;
                break;
        }

        int cnt = 0;
        int halfAmount = (int)(_cardsAmount * .5f);
        for (int i = 0; i < _cardsAmount; i++)
        {
            GameCard.EntityParams entity = new GameCard.EntityParams();
            entity.Index = i;

            if (i >= halfAmount)
                entity.Type = (CardType)(i - halfAmount);
            else
                entity.Type = (CardType)i;

            var card = Instantiate(CardPrefab, Field);
            GameCard cardInfo = card.GetComponent<GameCard>();
            cardInfo.Player = this;
            cardInfo.SetEntity(entity);

            _entities.Add(entity);
            _cards.Add(cardInfo);

            cnt++;
        }
    }

    private void PlaceCards()
    {
        // Set up cards
        RectTransform rect = _cards[0].transform as RectTransform;
        Vector2 cardSize = new Vector2(rect.rect.width, rect.rect.height);

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

        int mostCardsInRow = _cards.Count < cardsPerRow ? _cards.Count : cardsPerRow;

        List<Vector3> positionsList = new List<Vector3>(_cards.Count);
        float spacing = 40f;
        float addX = 0, addY = 0;
        // Add offset to center cards to stage
        Vector2 offset = new Vector2((fieldSize.x - (cardsPerRow * (cardSize.x + spacing) + (cardSize.x * .5f))) * .5f, (fieldSize.y - (cardsPerColumn * (cardSize.y + spacing))) * .5f);

        for(int i = 0; i < _cards.Count; i++)
        {
            positionsList.Add(new Vector3((cardSize.x * .5f) + addX + offset.x, -(cardSize.y * .5f) - addY - offset.x));
            addX += cardSize.x + spacing;
            cardCnt++;
            if (cardCnt == cardsPerRow)
            {
                cardCnt = 0;
                addX = 0;
                addY += cardSize.y + spacing;
                rowCnt++;
            }
        }

        // shuffle cards by simply shuffling their positions
        positionsList.Shuffle();

        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].transform.localPosition = positionsList[i];
        }
    }

    public void CardTapped(GameCard.EntityParams entity)
    {
        //Debug.Log("Card " + entity.Type + " at index " + entity.Index + " was flipped.");

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
                            card.DelayDisappear();
                            break;
                        }
                    }
                }

                Matches.text = ((int)(_cardsSolvedAmount * .5f)).ToString();
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
                            card.DelayFlip();
                            break;
                        }
                    }
                }
                SoundController.PlayAudio(GameSoundController.AudioType.Mismatch);
                _consecutiveMatchCnt = 0;
            }
            _checkingEntities.Clear();

            _turns++;
            Turns.text = _turns.ToString();
            Score.text = _score.ToString();

            if (_cardsSolvedAmount == _cardsAmount)
            {
                //Debug.Log("You win the game!");
                SoundController.PlayAudio(GameSoundController.AudioType.Victory);
                GameEnd();
            }
        }
    }

    public void GameEnd()
    {
        EndCard.EntityParams entity = new EndCard.EntityParams();
        entity.TurnsAmount = _turns;
        entity.FinalScore = _score;

        EndCard.SetEntity(entity);
        EndCard.Activate();
    }
}
