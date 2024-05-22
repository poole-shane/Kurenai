using System.Collections;
using System.Collections.Generic;

public enum CardType
{
    TypeA,
    TypeB,
    TypeC,
    TypeD,
    TypeE,
    TypeF,
    TypeG,
    TypeH,
    TypeI,
    TypeJ,
    TypeK,
    TypeL,
    TypeM,
    TypeN,
    TypeO,
}

public enum TableLayout
{
    TwoByTwo,
    ThreeByTwo,
    FourByThree,
    SixByFive,
}

public static class GlobalConstants
{
    public const int CARD_TYPES_MAX = 15;

    public const int TWO_BY_TWO_CARDS_AMOUNT = 4;
    public const int THREE_BY_TWO_CARDS_AMOUNT = 6;
    public const int FOUR_BY_THREE_CARDS_AMOUNT = 12;
    public const int SIX_BY_FIVE_CARDS_AMOUNT = 30;

    public const string START_MENU = "StartMenu";
    public const string GAME_SCENE = "GameScene";
}

static class MyExtensions
{
    /// <summary>
    /// The Fisher-Yates shuffle algorithm.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}