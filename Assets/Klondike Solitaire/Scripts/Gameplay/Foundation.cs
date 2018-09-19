using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Foundation : DropZone {

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
            //If there is a card
            if (transform.childCount > 0)
            {
                decision = TryPutDroppedCardOnLastCard(droppedCard);

            }
            else
            {
                decision = TryPutDroppedCardOnEmptyStack(droppedCard);
            }
            

            if (decision && droppedCard.transform.childCount == 0)
            {
                int points = CalculatePoints(droppedCard);
                MatchStatistics.instance.AddScore(points);
                MatchStatistics.instance.vegasScore += Constants.VEGAS_SCORE_PER_CARD;
                MatchStatistics.instance.moves++;
                base.OnDrop(droppedCard);
            }
        }
        else
        {
            Debug.LogError("There is no Card component in dropped object");
        }
    }

    public override void AssignNewChild(Transform child)
    {
        base.AssignNewChild(child);
    }


    private int CalculatePoints(Card droppedCard)
    {
        if(droppedCard.previousParent.GetComponent<Waste>() != null)
        {
            return Constants.WASTE2FOUNDATIONS_POINTS;
        }
        else if(droppedCard.previousParent.GetComponent<Tableau>() != null)
        {
            return Constants.TABLEAU2FOUNDATIONS_POINTS;
        }
        return 0;
    }

    private bool TryPutDroppedCardOnEmptyStack(Card droppedCard)
    {
        if ((int)droppedCard.cardValue == 1)
            return true;
        else
            return false;
    }

    private bool TryPutDroppedCardOnLastCard(Card droppedCard)
    {

        Card lastCardInTableau = transform.GetChild(transform.childCount - 1).GetComponent<Card>();

		if (droppedCard.cardColor == lastCardInTableau.cardColor && (int)droppedCard.cardValue == (int)lastCardInTableau.cardValue + 1)
            return true;
        return false;
    }

    public bool CheckIsFoundationComplete()
    {
        if(transform.childCount == Constants.NUMBER_OF_FULL_COLOR_CARDS)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
