using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CardFlipped(GameCard.EntityParams entity)
    {
        Debug.Log("Card " + entity.Type + " at index " + entity.Index + " was flipped.");
    }
}
