using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    private const int CARDS_FLIPPABLE_AMOUNT = 2;
    private int _cardsAmount = 8, _cardsSolvedAmount = 0;
    private int _turns = 0;

    private List<GameCard> _cards = new List<GameCard>();
    private List<GameCard.EntityParams> _entities = new List<GameCard.EntityParams>();

    private List<GameCard.EntityParams> _checkingEntities = new List<GameCard.EntityParams>();

    public TableLayout Layout;
    public RectTransform Field;
    public GameObject CardPrefab;

    public TextMeshProUGUI Matches;
    public TextMeshProUGUI Turns;

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
        //int cardsPerRow = (int)Math.Floor(fieldSize.x / cardSize.x);
        int cardsPerRow = 0;
        switch (Layout)
        {
            default:
            case TableLayout.TwoByTwo:
                cardsPerRow = 2;
                break;
            case TableLayout.ThreeByTwo:
                cardsPerRow = 2;
                break;
            case TableLayout.FourByThree:
                cardsPerRow = 4;
                break;
            case TableLayout.SixByFive:
                cardsPerRow = 6;
                break;
        }

        int cardCnt = 0;
        int rowCnt = 0;

        int mostCardsInRow = _cards.Count < cardsPerRow ? _cards.Count : cardsPerRow;
        
        float openSpaceX = fieldSize.x - (mostCardsInRow * cardSize.x);
        float spacingX = openSpaceX / mostCardsInRow;

        List<Vector3> positionsList = new List<Vector3>(_cards.Count);

        foreach (var card in _cards)
        {
            positionsList.Add(new Vector3((cardCnt * cardSize.x) + (cardSize.x * .5f) + (spacingX * cardCnt), -(rowCnt * cardSize.y) - (cardSize.y * .5f), 0));
            cardCnt++;
            if (cardCnt == cardsPerRow)
            {
                cardCnt = 0;
                rowCnt++;
            }
        }
        positionsList.Shuffle();

        for(int i = 0; i < _cards.Count; i++)
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
            }
            _turns++;
            Turns.text = _turns.ToString();
            _checkingEntities.Clear();

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

        EndCard.SetEntity(entity);
        EndCard.Activate();
    }
}
