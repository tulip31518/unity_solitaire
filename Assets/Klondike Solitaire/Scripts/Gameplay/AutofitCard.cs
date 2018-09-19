using UnityEngine;
using System.Collections.Generic;
using System;

public class AutofitCard  {
    
    public List<Card> possibleCardsFoundation { get; private set; }
    public List<Card> possibleOnTopCardsTableau { get; private set; }
    private List<Transform> possibleEmptyTableau;
    private List<Transform> possibleEmptyFoundation;

    void GetPossibleCards()
    {
        possibleOnTopCardsTableau = BoardManager.instance.GetOnTopTableauCards(out possibleEmptyTableau);
        possibleCardsFoundation = BoardManager.instance.GetFoundationCards(out possibleEmptyFoundation);
    }

    public bool TryAutoFitCard(Card card)
    {
        GetPossibleCards();
        if(SearchInFoundation(card))
        {
            return true;
        }
        else if(SearchInTableau(card))
        {
            return true;
        }
        return false;
    }



    private bool SearchInFoundation(Card card)
    {
        //if this is not last card in tableau return false
        if (card.transform.GetSiblingIndex() != card.transform.parent.childCount - 1)
        {
            return false;		
        }    

        if (card.cardValue == Enums.CardValue.Ace)
        {
            if (possibleEmptyFoundation.Count > 0)
            {
                StartCardAutoFitAnimation(card, possibleEmptyFoundation[0]);
                return true;
            }
        }
        else
        {
            //Dont Autofit while shake anim 
            if (card.transform.childCount > 0)
                return false;
            for (int i = 0; i < possibleCardsFoundation.Count; i++)
            {
                Card pCard = possibleCardsFoundation[i];
                if (card.cardColor == pCard.cardColor && (int)(card.cardValue) == (int)(pCard.cardValue + 1))
                {
                    StartCardAutoFitAnimation(card, pCard.transform.parent);
                    return true;
                }
            }
        }
        return false;
    }

    private bool SearchInTableau(Card card)
    {
        if (card.cardValue == Enums.CardValue.King)
        {
            //Check if it is valid hint 
            if (IsCardTableauParented(card) &&  card.transform.GetSiblingIndex() == 0)
            {
                return false;
            }

            //If card is a king search empty places in tableau
            for (int i = 0; i < possibleEmptyTableau.Count; i++)
            {
                if (card.transform.parent != possibleEmptyTableau[i])
                {
                    StartCardAutoFitAnimation(card, possibleEmptyTableau[i]);
                    return true;
                }
            }
        }
        else
        {
            for (int i = 0; i < possibleOnTopCardsTableau.Count; i++)
            {
                Card pCard = possibleOnTopCardsTableau[i];
                //Dont Autofit while shake anim 
                if (pCard == null || pCard.transform.childCount > 0)
                    continue;
                if (card.isRed != pCard.isRed && (int)(card.cardValue + 1) == (int)pCard.cardValue && card.transform.parent != pCard.transform.parent)
                {
                
                    StartCardAutoFitAnimation(card, pCard.transform.parent);
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsCardTableauParented(Card card)
    {
        return card.transform.parent.GetComponent<Tableau>() != null;
    }

    private void StartCardAutoFitAnimation(Card card,Transform zoneParent)
    {
        card.OnBeginDrag(null);
        if (zoneParent.GetComponent<Tableau>() != null)
        {
            zoneParent.GetComponent<Tableau>().OnDrop(card);
            card.StartAutoFit();
        }
        else if (zoneParent.GetComponent<Foundation>() != null)
        {
            zoneParent.GetComponent<Foundation>().OnDrop(card);
            card.StartAutoFit();
        }
        else
        {
            card.OnEndDrag(null);
        }
    }
}
