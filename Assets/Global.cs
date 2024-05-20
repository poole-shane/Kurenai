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
    TwoByThree,
    FiveBySix,
}

public static class Global
{
    public const int CARD_TYPES_MAX = 15;
}

static class MyExtensions
{
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