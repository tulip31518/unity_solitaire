using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HintPossibilities {
	public List<Card> possibleCardsTableau { get; private set; }
	public List<Card> possibleCardsFoundation { get; private set; }
	public List<Card> possibleOnTopCardsTableau { get; private set; }
	public List<Transform> possibleEmptyTableau;
	public List<Transform> possibleEmptyFoundation;
	public List<Card> possibleWasteCards { get; private set; }
	public List<Card> possibleStockCards { get; private set; }

	public HintPossibilities(bool searchAllStockAndWaste) {
		if (BoardManager.instance != null)
		{
			possibleCardsTableau = BoardManager.instance.GetTableauCards();
			possibleOnTopCardsTableau = BoardManager.instance.GetOnTopTableauCards(out possibleEmptyTableau);
			possibleCardsFoundation = BoardManager.instance.GetFoundationCards(out possibleEmptyFoundation);
			possibleWasteCards = BoardManager.instance.GetCurrentWasteCards(searchAllStockAndWaste);
			if (searchAllStockAndWaste)
				possibleStockCards = BoardManager.instance.GetCurrentStockCards();
		}
	}

	private void DebugInfo() {

		foreach (var item in possibleCardsTableau)
		{
			Debug.Log(item.name);
		}
		Debug.Log("Possbile empty tableaus");

		foreach (var item in possibleEmptyTableau)
		{
			Debug.Log(item.name);
		}
		Debug.Log("Possbile foundation cards");

		foreach (var item in possibleCardsFoundation)
		{
			Debug.Log(item.name);
		}
		Debug.Log("Possbile empty founds");

		foreach (var item in possibleEmptyFoundation)
		{
			Debug.Log(item.name);
		}
		Debug.Log("Possbile waste card");

		foreach (var item in possibleWasteCards)
		{
			Debug.Log(item.name);
		}

		Debug.Log("Possbile stock card");

		foreach (var item in possibleStockCards)
		{
			Debug.Log(item.name);
		}
	}
}

public class HintManager : MonoBehaviour {

	[Serializable]
	public class HintMove {
		public Card card;
		public Vector3 offset;
		public Transform destParent;
		public bool parentCardsBelow;
		public Transform customHintElement;
	}

	public static HintManager instance;
	private HintPossibilities hintPossibilities;
	private Queue<HintMove> hintMoves;
	private Transform undoHolder;
	private bool isInHintMode;
	private Card currentCard;
	private Stock stock;
	private Waste waste;
	private AnimationQueueController animationQueueController;
	private HintDimScreen dimScreen;

	private void Awake() {
		instance = this;
		stock = FindObjectOfType<Stock>();
		waste = FindObjectOfType<Waste>();
		animationQueueController = FindObjectOfType<AnimationQueueController>();
		hintMoves = new Queue<HintMove>();
		undoHolder = GameObject.FindGameObjectWithTag("UndoHolder").transform;
		dimScreen = FindObjectOfType<HintDimScreen>();
	}

	private void Update() {
		if (isInHintMode && Input.GetMouseButtonDown(0))
		{
			isInHintMode = false;
		}
	}

	public void TryHint() {
		animationQueueController.AddActionToQueue(OnHint);
	}

	private void OnHint() {
		BoardManager.instance.LockBoard();
		SearchForPossibilitiesOfMove(false);
		if (hintMoves.Count == 0)
		{
			if (stock.transform.childCount > 0)
			{
				//If there is no hints try to highlight stock if any card available
				hintMoves.Enqueue(new HintMove()
				{
					card = stock.transform.GetChild(0).GetComponent<Card>(),
					destParent = stock.transform,
					offset = Constants.vectorZero
				});
			}
			else if (waste.transform.childCount > 0)
			{
				//If there is no cards on stock but cards are on waste highlight stock too
				hintMoves.Enqueue(new HintMove()
				{
					customHintElement = stock.stockGraphics
				});
			}
		}

		if (hintMoves.Count > 0)
		{
			dimScreen.MakeScreenDark(0.8f);
			animationQueueController.SetAnimationStatus(AnimationStatus.inProgress);
			isInHintMode = true;
			DelayedHint();
		}
		else
		{
			Debug.Log("No hints!");
			NoHints();
		}
	}

	public bool SearchForPossibilitiesOfMove(bool searchAllStockAndWaste) {
		hintPossibilities = new HintPossibilities(searchAllStockAndWaste);
		hintMoves.Clear();
		SearchForPossibleMovesInFoundation(hintPossibilities.possibleWasteCards);
		SearchForPossibleMovesInTableau(hintPossibilities.possibleWasteCards);
		if (searchAllStockAndWaste)
		{
			SearchForPossibleMovesInFoundation(hintPossibilities.possibleStockCards);
			SearchForPossibleMovesInTableau(hintPossibilities.possibleStockCards);
		}
		SearchForPossibleMovesInFoundation(hintPossibilities.possibleOnTopCardsTableau);
		SearchForPossibleMovesInTableau(hintPossibilities.possibleCardsTableau);

		return hintMoves.Count > 0;
	}

	private void SearchForPossibleMovesInFoundation(List<Card> possibleCardsToCheck) {
		for (int j = 0; j < possibleCardsToCheck.Count; j++)
		{
			Card cardToCheck = possibleCardsToCheck[j];
			if (cardToCheck != null)
			{
				//Dont Autofit while shake anim
				if (cardToCheck.transform.childCount > 0)
					continue;
				if (cardToCheck.cardValue == Enums.CardValue.Ace)
				{
					//If card is a ace search empty places in foundation
					for (int i = 0; i < hintPossibilities.possibleEmptyFoundation.Count; i++)
					{
						if (CardsHaveDifferentParents(cardToCheck.transform.parent, hintPossibilities.possibleEmptyFoundation[i]))
						{
							hintMoves.Enqueue(new HintMove()
							{
								card = cardToCheck,
								destParent = hintPossibilities.possibleEmptyFoundation[i].transform,
								offset = Constants.vectorZero
							});
						}
					}
				}
				else
				{
					for (int i = 0; i < hintPossibilities.possibleCardsFoundation.Count; i++)
					{
						Card fCard = hintPossibilities.possibleCardsFoundation[i];
						if (CardsAreInTheSameColor(cardToCheck, fCard) && CardHasNextValue(cardToCheck, fCard)
							&& CardsHaveDifferentParents(cardToCheck.transform.parent, fCard.transform.parent))
						{
							hintMoves.Enqueue(new HintMove()
							{
								card = cardToCheck,
								destParent = fCard.transform,
								offset = Constants.vectorZero
							});
						}
					}
				}
			}
		}
	}

	private void SearchForPossibleMovesInTableau(List<Card> possibleCardsToCheck) {
		for (int j = 0; j < possibleCardsToCheck.Count; j++)
		{
			Card cardToCheck = possibleCardsToCheck[j];
			if (cardToCheck != null)
			{
				if (cardToCheck.cardValue == Enums.CardValue.King)
				{
					//Check if it is valid hint
					if (cardToCheck.transform.GetSiblingIndex() == 0)
					{
						continue;
					}
					//If card is a king search empty places in tableau
					for (int i = 0; i < hintPossibilities.possibleEmptyTableau.Count; i++)
					{
						if (CardsHaveDifferentParents(cardToCheck.transform.parent, hintPossibilities.possibleEmptyTableau[i]))
						{
							hintMoves.Enqueue(new HintMove()
							{
								card = cardToCheck,
								destParent = hintPossibilities.possibleEmptyTableau[i].transform,
								offset = Constants.vectorZero,
								parentCardsBelow = true
							});
						}
					}
				}
				else
				{
					for (int i = 0; i < hintPossibilities.possibleOnTopCardsTableau.Count; i++)
					{
						Card tCard = hintPossibilities.possibleOnTopCardsTableau[i];
						//Dont Autofit while shake anim
						if (tCard.transform.childCount > 0)
							continue;

						if (cardToCheck.isRed != tCard.isRed && CardHasNextValue(tCard, cardToCheck) && CardsHaveDifferentParents(cardToCheck.transform.parent, tCard.transform.parent))
						{
							//Chech if it is valid no redundant hint
							if (tCard == cardToCheck.lastGoodParametres.lastCardAbove)
							{
								continue;
							}

							hintMoves.Enqueue(new HintMove()
							{
								card = cardToCheck,
								destParent = tCard.transform,
								offset = -Vector3.up * CalculateCardsOffset(tCard.size.y),
								parentCardsBelow = true
							});
						}
					}
				}
			}
		}
	}

	private bool CardsAreInTheSameColor(Card c1, Card c2) {
		return c1.cardColor == c2.cardColor;
	}

	private bool CardHasNextValue(Card previousCard, Card nextCard) {
		return (int)previousCard.cardValue == (int)nextCard.cardValue + 1;
	}

	private bool CardsHaveDifferentParents(Transform p1, Transform p2) {
		return p1 != p2;
	}

	private float CalculateCardsOffset(float cardHeight) {
		float offset = ScreenOrientationSwitcher.IsPortrait() ? Constants.TABLEAU_SPACING : Constants.LANDSCAPE_TABLEAU_SPACING;
		return (cardHeight + offset) * BehaviourSettings.instance.GetScaleFactor();
	}

	private void RunHint() {
		new Timer(Constants.MOVE_ANIM_TIME, DelayedHint);
	}

	private void DelayedHint() {
		if (hintMoves.Count == 0 || !isInHintMode)
		{
			FinishHintAnim();
		}
		else
		{
			HintMove hm = hintMoves.Dequeue();
			if (hm.card != null)
			{
				currentCard = hm.card;
				if (hm.parentCardsBelow)
					currentCard.ParentCardsBelow();
				currentCard.SetReturnPoint(hm.card.transform.parent, hm.offset);
				currentCard.transform.SetParent(undoHolder, true);
				SoundManager.instance.PlayPickCardSound();
				currentCard.RegisterOnAnimationFinishCB(RunHint);
				currentCard.MoveCard(Constants.HINT_ANIM_TIME, hm.destParent);
			}
			else if (hm.customHintElement != null)
			{
				Transform prevParent = hm.customHintElement.parent;
				hm.customHintElement.transform.SetParent(undoHolder, true);
				SoundManager.instance.PlayPickCardSound();
				new Timer(Constants.HINT_ANIM_TIME, () =>
				   {
					   hm.customHintElement.transform.SetParent(prevParent, true);
					   hm.customHintElement.SetAsFirstSibling();
					   RunHint();
				   });
			}
		}
	}

	private void FinishHintAnim() {
		dimScreen.MakeScreenLight();
		BoardManager.instance.UnlockBoard();
		isInHintMode = false;
		animationQueueController.CastNextAnimation();
	}

	public void StopHintMode() {
		isInHintMode = false;
	}

	private void NoHints() {
		BoardManager.instance.UnlockBoard();
		animationQueueController.CastNextAnimation();
	}
}