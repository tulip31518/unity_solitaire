using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Tableau : DropZone {

    public override void OnDrop(PointerEventData eventData)
    {
        Card droppedCard = eventData.pointerDrag.GetComponent<Card>();
        transform.root.BroadcastMessage("DeactivateExtraZone", SendMessageOptions.DontRequireReceiver);
        OnDrop(droppedCard);
    }

    public override void OnDrop(Card droppedCard)
    {
        if (droppedCard != null)
        {
            bool decision = false;
            if (transform.childCount > 0)
            {
                decision = TryPutDroppedCardOnLastCard(droppedCard);
            }
            else
            {
                decision = TryPutDroppedCardOnEmptyStack(droppedCard);
            }

            if (decision)
            {
                int points = CalculatePoints(droppedCard);
                MatchStatistics.instance.AddScore(points);
                MatchStatistics.instance.moves++;
                base.OnDrop(droppedCard);
            }
        }
        else
        {
            Debug.LogError("There is no Card component in dropped object");
        }
    }

    private int CalculatePoints(Card droppedCard)
    {
        if (droppedCard.previousParent == null)
        {
            return 0;
        }
        if (droppedCard.previousParent.GetComponent<Waste>() != null)
        {
            return Constants.WASTE2TABLEAU_POINTS;
        }
        else if (droppedCard.previousParent.GetComponent<Foundation>() != null)
        {
            MatchStatistics.instance.vegasScore -= Constants.VEGAS_SCORE_PER_CARD;
            return Constants.FOUNDATION2TABLEAU_POINTS;
        }
        return 0;
    }

    private bool TryPutDroppedCardOnEmptyStack(Card droppedCard)
    {
        if (droppedCard.cardValue == Enums.CardValue.King)
            return true;
        else
            return false;
    }

    public bool TryPutDroppedCardOnLastCard(Card droppedCard)
    {
        Card lastCardInTableau = transform.GetChild(transform.childCount - 1).GetComponent<Card>();
        if (droppedCard.isRed != lastCardInTableau.isRed && (int)droppedCard.cardValue == (int)lastCardInTableau.cardValue - 1)
            return true;
        return false;
    }

    public bool CheckIfTableauHasAllCardExposed()
    {
        Card[] cards = GetComponentsInChildren<Card>();
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].isReversed)
                return false;
        }
        return true;
    }
}
