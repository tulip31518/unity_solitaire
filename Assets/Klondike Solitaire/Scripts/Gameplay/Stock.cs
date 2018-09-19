using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class Stock : MonoBehaviour, IPointerClickHandler {
	public int stockLimit { get; set; }
	public Transform stockGraphics { get; private set; }
	private Waste waste;
	private Image stockImage;
	private AnimationQueueController animationQueueController;
	private GameObject refreshImage;
	private float noOfAnimations;
	private List<Transform> wasteOrderList;
	private Transform undoHolder;
	private bool sortStock;
	private bool _stockState;
	private LayoutElement layoutElement;

	public bool stockState {
		get
		{
			return _stockState;
		}
	}

	public void RefreshStockState(bool state, bool shouldRefreshImage = true) {
		if (state != _stockState)
		{
			_stockState = state;
			if (_stockState)
				ShowStock();
			else
				HideStock();
		}

		if (IsStockEmpty())
		{
			HideStock();
			_stockState = false;
		}
		if (shouldRefreshImage)
		{
			RefreshStockRefreshImage();
		}
	}

	public void RefreshStockRefreshImage() {
		refreshImage.SetActive(!_stockState && !(MatchStatistics.instance.IsVegasGame() && stockLimit <= 1));
		if (transform.childCount == 0 && waste.transform.childCount == 0)
			refreshImage.SetActive(false);
	}

	private void Start() {
		waste = FindObjectOfType<Waste>();
		stockImage = GetComponent<Image>();
		animationQueueController = FindObjectOfType<AnimationQueueController>();
		stockGraphics = transform.parent.Find("Graphics");
		wasteOrderList = new List<Transform>();
		undoHolder = GameObject.FindGameObjectWithTag("UndoHolder").transform;
		refreshImage = GameObject.Find("ResetStock");
		ResetState();
	}

	private void ResetState() {
		RefreshStockState(true);
		stockLimit = MatchStatistics.instance.isDraw3 ? 3 : 1;
	}

	public void OnPointerClick(PointerEventData eventData) {
		animationQueueController.AddActionToQueue(GetCard);
	}

	public void GetCard() {
		int cards = MatchStatistics.instance.isDraw3 ? 3 : 1;
		waste.RegisterOnRefreshWasteAction(FinishReturnWasteAnimation);
		noOfAnimations = 1;
		SaveManager.instance.Save();
		MatchStatistics.instance.moves++;

		for (int i = 0; i < cards; i++)
		{
			if (transform.childCount > 0)
			{
				if (transform.childCount == 1)
					RefreshStockState(false);

				animationQueueController.SetAnimationStatus(AnimationStatus.inProgress);
				SoundManager.instance.PlayPickCardSound();
				Card card = transform.GetChild(0).GetComponent<Card>();
				card.transform.SetParent(undoHolder);
				card.SetReturnPoint(waste.transform, Constants.vectorZero);
				card.RegisterOnReverseAnimationFinishCB(FinishReturnWasteAnimation);
				noOfAnimations++;
				card.RotateCard(Constants.STOCK_ANIM_TIME);
				card.RegisterOnAnimationFinishCB(FinishReturnWasteAnimation);
				noOfAnimations++;
				card.MoveCard(Constants.STOCK_ANIM_TIME * 2);
			}
			else
			{
				if (i == 0) //If first card return stock
					ReturnCardsFromWasteToStock();
				break;
			}
		}
	}

	private void ReturnCardsFromWasteToStock() {
		stockLimit--;
		MatchStatistics.instance.moves--;
		if (MatchStatistics.instance.IsVegasGame() && stockLimit <= 0)
		{
			stockLimit = 0;
			RefreshStockState(false);
			SaveManager.instance.RemoveLastSave();
			animationQueueController.CastNextAnimation();
		}
		else
		{
			int childCount = waste.transform.childCount;
			wasteOrderList.Clear();
			if (childCount > 0)
			{
				SoundManager.instance.PlayPickCardSound();
				noOfAnimations = childCount;
				animationQueueController.SetAnimationStatus(AnimationStatus.inProgress);
				sortStock = true;
			}
			for (int i = 0; i < childCount; i++)
			{
				Transform child = waste.transform.GetChild(0);
				wasteOrderList.Add(child);
				Card card = child.GetComponent<Card>();

				card.transform.SetParent(transform.parent);
				card.SetReturnPoint(transform, Constants.vectorZero);
				card.RegisterOnReverseAnimationFinishCB(FinishReturnWasteAnimation);
				noOfAnimations++;
				card.RotateCard(Constants.STOCK_ANIM_TIME);
				card.RegisterOnAnimationFinishCB(FinishReturnWasteAnimation);
				card.MoveCard(Constants.STOCK_ANIM_TIME * 2);
			}
			if (childCount == 0)
			{
				animationQueueController.CastNextAnimation();
				RefreshStockState(false);
				SaveManager.instance.RemoveLastSave();
			}
		}
	}

	private void FinishReturnWasteAnimation() {
		noOfAnimations--;
		if (noOfAnimations <= 0)
		{
			RefreshStockState(false);

			if (sortStock)
				RestoreGoodStockOrder();
			animationQueueController.CastNextAnimation();
		}
	}

	private void RestoreGoodStockOrder() {
		sortStock = false;
		for (int i = 0; i < wasteOrderList.Count; i++)
		{
			wasteOrderList[i].SetAsLastSibling();
			ResetStockCardsPos();
		}
	}

	public void ResetStockCardsPos() {
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).localPosition = Constants.vectorZero;
		}
	}

	private void ShowStock() {
		stockImage.color = new Color(1, 1, 1, 1);
	}

	private void HideStock() {
		stockImage.color = new Color(1, 1, 1, 0);
	}

	private bool IsStockEmpty() {
		return transform.childCount == 0;
	}

	public void CheckStockState() {
		if (IsStockEmpty())
			RefreshStockState(false);
		else
			RefreshStockState(false);
	}

	public void RefreshStockSize() {
		if (layoutElement == null)
			layoutElement = GetComponent<LayoutElement>();

		(transform as RectTransform).sizeDelta = new Vector2(layoutElement.minWidth, layoutElement.minHeight);
		transform.localPosition = Constants.vectorZero;
	}
}