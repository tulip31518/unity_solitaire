using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class BoardManager : MonoBehaviour {
	public static BoardManager instance { get; private set; }

	//public static GameObject[] cardPrefabs;
	public bool isGameWon { get; private set; }

	public GameObject noMoreMovesPopup;
    public GameObject finishPopup;

	private GameObject cardPrefab;
	private List<GameObject> cardsPool;
	private bool isReplay;

	//private GameObject[] cardsPool;
	private Tableau[] tableaus;

	private Foundation[] foundations;
	private Waste waste;
	private Stock stock;
	private CanvasGroup canvasGroup;
	private AutofitCard autofitCard;
	private AnimationQueueController animationQueueController;
	private bool tableauCardsExposed;
	private bool probablyLostGameContinued;

	private IEnumerator dealCardsCoroutine;

	private void Awake() {
		instance = this;
		try
		{
			cardPrefab = Resources.Load<GameObject>("CardPrefabs/CardPrefab");
			tableaus = FindObjectsOfType<Tableau>();
			tableaus = tableaus.OrderBy((x) => x.name).ToArray();
			foundations = FindObjectsOfType<Foundation>();
			foundations = foundations.OrderBy((x) => x.name).ToArray();
			animationQueueController = FindObjectOfType<AnimationQueueController>();
			waste = FindObjectOfType<Waste>();
			stock = FindObjectOfType<Stock>();
			canvasGroup = GetComponent<CanvasGroup>();
			autofitCard = new AutofitCard();
		}
		catch (System.Exception e)
		{
			Debug.LogError(e.Message);
		}
	}

	private void Start() {
		PrepareObjectsPool();
		ResetState();
	}

	private void ResetState() {
		Autocomplete.instance.SetButtonState(false);
		isGameWon = false;
		tableauCardsExposed = false;
		probablyLostGameContinued = false;
		if (!isReplay)
		{
			ShuffleCards();
		}

		BackCardsToInitialState();
		SaveManager.instance.SetGameCardsSet(cardsPool);
		if (dealCardsCoroutine != null)
			StopCoroutine(dealCardsCoroutine);
		dealCardsCoroutine = DealCards();
		StartCoroutine(dealCardsCoroutine);
	}

    public void Exit()
    {
        Application.Quit();
    }

	public void NewGame() {
		isReplay = false;
		UIManager.instance.ResetGameState();
	}

	public void Replay() {
		isReplay = true;
		UIManager.instance.ResetGameState();
	}

	public void ContinueGame() {
		probablyLostGameContinued = true;
	}

	private void ShuffleCards() {
		do
		{
			for (int i = cardsPool.Count - 1; i > 0; --i)
			{
				int index = Randomizer.GetRandomNumber(0, i);
				GameObject temp = cardsPool[i];
				cardsPool[i] = cardsPool[index];
				cardsPool[index] = temp;
			}
		} while (!CheckIfGameIsPossibleToPlay());
		Resources.UnloadUnusedAssets();
	}

	private bool CheckIfGameIsPossibleToPlay() {
		Card[] cards = new Card[cardsPool.Count];
		for (int i = 0; i < cards.Length; i++)
		{
			cards[i] = cardsPool[i].GetComponent<Card>();
		}
		return ThereIsAnyAceAvailable(cards) || ThereIsAnyMovePossible(cards);
	}

	private bool ThereIsAnyAceAvailable(Card[] cards) {
		int j = 0;
		for (int i = 0; i < cards.Length; i++)
		{
			if (i < Constants.STOCK_START_INDEX)
			{
				if (i == Constants.availableCardsIndexes[j])
				{
					j++;
					if (cards[i].cardValue == Enums.CardValue.Ace)
						return true;
				}
			}
			else
			{
				if (cards[i].cardValue == Enums.CardValue.Ace)
					return true;
			}
		}
		return false;
	}

	private bool ThereIsAnyMovePossible(Card[] cards) {
		int j = 0;

		for (int i = 0; i < cards.Length; i++)
		{
			if (i < Constants.STOCK_START_INDEX)
			{
				if (i == Constants.availableCardsIndexes[j])
				{
					j++;
					if (CheckIfCardFitToAnyAvailablePlace(cards[i], cards))
						return true;
				}
			}
			else
			{
				if (CheckIfCardFitToAnyAvailablePlace(cards[i], cards))
					return true;
			}
		}
		return false;
	}

	private bool CheckIfCardFitToAnyAvailablePlace(Card cardToFit, Card[] cards) {
		for (int i = 0; i < Constants.availableCardsIndexes.Length; i++)
		{
			Card cardToCheck = cards[Constants.availableCardsIndexes[i]];
			if (cardToFit.isRed != cardToCheck.isRed && (int)(cardToFit.cardValue + 1) == (int)cardToCheck.cardValue)
			{
				return true;
			}			
		}
		return false;
	}

	private void PrepareObjectsPool() {
		List<Enums.CardValue> valuesAsArray = Enum.GetValues(typeof(Enums.CardValue)).Cast<Enums.CardValue>().ToList();
		List<Enums.CardColor> colorsAsArray = Enum.GetValues(typeof(Enums.CardColor)).Cast<Enums.CardColor>().ToList();

		cardsPool = new List<GameObject>();
		int colorIndex = -1;

		for (int i = 0; i < Constants.CARDS_IN_DECK; i++)
		{
			if (i % Constants.CARDS_IN_COLOR == 0)
				colorIndex = (colorIndex + 1) % Constants.CARDS_COLORS;
			GameObject cardGO = Instantiate(cardPrefab, transform);
			Card card = cardGO.GetComponent<Card>();
			card.cardID = i;
			card.cardValue = valuesAsArray[i % Constants.CARDS_IN_COLOR];
			card.cardColor = colorsAsArray[colorIndex];
			card.name = card.cardColor + "_" + card.cardValue;
			cardsPool.Add(cardGO);
		}
		BackCardsToInitialState();
	}

	private void BackCardsToInitialState() {
		for (int i = 0; i < cardsPool.Count; i++)
		{
			cardsPool[i].transform.SetParent(stock.transform, transform);
			cardsPool[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			cardsPool[i].transform.localScale = Vector3.one;
			cardsPool[i].transform.SetAsLastSibling();
		}
	}

	private IEnumerator DealCards() {
		LockBoard();
		animationQueueController.SetAnimationStatus(AnimationStatus.inProgress);
		yield return new WaitForSeconds(0.1f);
		int index = 0;
		for (int i = 0; i < Constants.NUMBER_OF_TABLEAUS; i++)
		{
			for (int j = 0; j <= i; j++)
			{
				SoundManager.instance.PlayPickCardSound();
				Card card = cardsPool[index].GetComponent<Card>();
				if (j == i) //If this card is on top card uncover it
				{
					card.isReversed = false;					
						BoxCollider2D bc = card.gameObject.GetComponent<BoxCollider2D>();
						Vector2 sz = bc.size;
						sz.y = 180;
						Vector2 offset = bc.offset;
						offset.y = 0;
						bc.size = sz;
						bc.offset = offset;
				}	

				card.transform.SetParent(stock.transform.parent);
				card.SetReturnPoint(tableaus[i].transform, -Vector3.up * j * BehaviourSettings.instance.CalculateCardsOffset(card.size.y) * BehaviourSettings.instance.GetScaleFactor());

				card.MoveCard(Constants.MOVE_ANIM_TIME / 3);
				index++;
				if (index == 28)
					card.RegisterOnAnimationFinishCB(CardDealt);
				else
					yield return new WaitForSeconds(0.05f);
			}
		}
		dealCardsCoroutine = null;
	}

	private void CardDealt() {
		animationQueueController.SetAnimationStatus(AnimationStatus.none);
		SaveManager.instance.ClearSaveList();
		UnlockBoard();
	}

	public void LockBoardForOneFrame() {
		LockBoard();
		Invoke("UnlockBoard", Time.fixedDeltaTime);
	}

	public void LockBoard() {
		canvasGroup.blocksRaycasts = false;
	}

	public void UnlockBoard() {
		canvasGroup.blocksRaycasts = true;
	}

	public List<Card> GetCurrentWasteCards(bool searchAllStockAndWaste) {
		List<Card> wasteCards = new List<Card>();

		if (waste.transform.childCount == 0)
			return wasteCards;

		if (searchAllStockAndWaste)
		{
			for (int i = 0; i < waste.transform.childCount; i++)
			{
				Card c = waste.transform.GetChild(i).GetComponent<Card>();
				wasteCards.Add(c);
			}
		}
		else
		{
			Card currentWasteCard = waste.transform.GetChild(waste.transform.childCount - 1).GetComponent<Card>();
			wasteCards.Add(currentWasteCard);
		}
		return wasteCards;
	}

	internal List<Card> GetCurrentStockCards() {
		List<Card> stockCards = new List<Card>();

		if (stock.transform.childCount == 0)
			return stockCards;

		for (int i = 0; i < stock.transform.childCount; i++)
		{
			Card c = stock.transform.GetChild(i).GetComponent<Card>();
			stockCards.Add(c);
		}

		return stockCards;
	}

	public List<Card> GetTableauCards() {
		List<Card> cards = new List<Card>();

		for (int i = 0; i < tableaus.Length; i++)
		{
			if (tableaus[i] != null && tableaus[i].transform.childCount != 0)
			{
				Card[] tableauCards = tableaus[i].GetComponentsInChildren<Card>();
				for (int j = 0; j < tableauCards.Length; j++)
				{
					if (!tableauCards[j].isReversed)
					{
						cards.Add(tableauCards[j]);
					}					
				}
			}
		}
		return cards;
	}

	public List<Card> GetOnTopTableauCards(out List<Transform> emptyTabelau) {
		List<Card> cards = new List<Card>();
		emptyTabelau = new List<Transform>();
		for (int i = 0; i < tableaus.Length; i++)
		{
			if (tableaus[i].transform.childCount == 0)
			{
				emptyTabelau.Add(tableaus[i].transform);
			}
			else
			{
				Card card = tableaus[i].transform.GetChild(tableaus[i].transform.childCount - 1).GetComponent<Card>();
				cards.Add(card);
			}
		}
		return cards;
	}

	public List<Card> GetOnTopTableauCards() {
		List<Transform> emptyList = new List<Transform>();
		return GetOnTopTableauCards(out emptyList);
	}

	public List<Card> GetFoundationCards(out List<Transform> emptyFoundation) {
		List<Card> cards = new List<Card>();
		emptyFoundation = new List<Transform>();
		for (int i = 0; i < foundations.Length; i++)
		{
			if (foundations[i].transform.childCount == 0)
			{
				emptyFoundation.Add(foundations[i].transform);
			}
			else
			{
				Card card = foundations[i].transform.GetChild(foundations[i].transform.childCount - 1).GetComponent<Card>();
				cards.Add(card);
			}
		}
		return cards;
	}

	public bool TryAutoFitCard(Card card) {
		return autofitCard.TryAutoFitCard(card);
	}

	public bool CheckIfGameComplete() {
		for (int i = 0; i < foundations.Length; i++)
		{
			if (!foundations[i].CheckIsFoundationComplete())
			{
				isGameWon = false;
				return false;
			}
		}
		FinishGame();

		return true;
	}

	public void FinishGame() {
		if (!isGameWon)
		{
			isGameWon = true;
			MatchStatistics.instance.StopTime();
			Autocomplete.instance.SetButtonState(false);
            finishPopup.SetActive(true);
        }
	}

	public bool CheckIfGameCanBeAutocomplete() {
		if (canvasGroup.blocksRaycasts)
		{
			if (!probablyLostGameContinued && !tableauCardsExposed && HintManager.instance.SearchForPossibilitiesOfMove(true) == false)
			{
				noMoreMovesPopup.SetActive(true);
				return false;
			}

			for (int i = 0; i < tableaus.Length; i++)
			{
				if (!tableaus[i].CheckIfTableauHasAllCardExposed())
				{
					Autocomplete.instance.SetButtonState(false);
					tableauCardsExposed = false;
					return false;
				}
			}
			tableauCardsExposed = true;

			if (!CheckDraw3OrVegasStockState())
			{
				Autocomplete.instance.SetButtonState(false);
				return false;
			}

			CheckIfGameComplete();
			Autocomplete.instance.SetButtonState(true);
			return true;
		}
		return false;
	}

	private bool CheckDraw3OrVegasStockState() {
		if (MatchStatistics.instance.isDraw3 || MatchStatistics.instance.IsVegasGame())
		{
			if (waste.transform.childCount + stock.transform.childCount > 1)
				return false;
			return true;
		}
		return true;
	}
}