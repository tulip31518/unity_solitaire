using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class Card : Draggable, IPointerClickHandler {
	public Enums.CardColor cardColor;
	public Enums.CardValue cardValue;
	public int cardID;

	//Card features
	public bool isRed { get; private set; }

	public bool isReversed {
		get
		{
			return _isReversed;
		}
		set
		{
			_isReversed = value;
			if (_isReversed)
			{
				cardImage.sprite = cardBackSprite;
			}
			else
			{
				cardImage.sprite = cardFrontSprite;
			}
		}
	}

	//Components
	private Image cardImage;

	private Sprite cardBackSprite;
	private Sprite cardFrontSprite;
	private Action OnReverseAnimationFinishCB;

	private void ResetState() {
		RectTransform rt = (transform as RectTransform);
		rt.pivot = Constants.vectorHalf;
		rt.anchorMax = Constants.vectorHalf;
		rt.anchorMin = Constants.vectorHalf;
		isReversed = true;
		lastGoodParametres.lastCardAbove = null;
		KillAllAnimations();
		gameObject.SetActive(false);
		gameObject.SetActive(true);
		previousParent = stock.transform;
		canvasGroup.blocksRaycasts = false;
		ResetActions();
	}

	public void AddAnimation(Coroutine anim) {
		animations.Add(anim);
	}

	public void KillAllAnimations() {
		for (int i = 0; i < animations.Count; i++)
		{
			TweenAnimator.instance.Kill(animations[i]);
		}
		animations.Clear();
	}

	public void RegisterOnReverseAnimationFinishCB(Action cb) {
		OnReverseAnimationFinishCB = cb;
	}

	public override void Awake() {
		base.Awake();
	}

	private void Start() {
		isRed = ((cardColor == Enums.CardColor.diamonds) || (cardColor == Enums.CardColor.hearts)) ? true : false;
		cardImage = GetComponent<Image>();
		LoadGraphics();
		ResetState();
	}

	private void LoadGraphics() {
		string path = "Cards" + "/" + cardColor.ToString() + "/" + cardColor.ToString() + "_" + cardValue.ToString();

		cardFrontSprite = Resources.Load<Sprite>(path);

		cardImage.sprite = cardFrontSprite;
		path = "Cards/covers/Cover";
		cardBackSprite = Resources.Load<Sprite>(path);
	}

	public override void OnBeginDrag(PointerEventData eventData) {
		if (transform.parent.GetComponent<DropZone>() != null || transform.parent.GetComponent<Card>() != null)
		{
			if (IsDropzoneChildAndLocked())
			{
				return;
			}
			TryStopParentAnimation();
			StopCardAnim();
			ParentCardsBelow();
			base.OnBeginDrag(eventData);
		}
		else
		{
			canDrag = false;
		}
	}

	private bool IsDropzoneChildAndLocked() {
		//Block waste while animation
		if (transform.parent.GetComponent<DropZone>() != null && animationQueueController.IsCurrentActionUndoOrGetCard())
		{
			canDrag = false;
			return true;
		}
		return false;
	}

	public void RotateCard(float time) {
		ResetCardPositionAndRotation();
		Coroutine rotAnim = TweenAnimator.instance.RunRotationAnimation(transform, new Vector3(0, 90, 0), time, onComplete: FlipCard);
		animations.Add(rotAnim);
		Coroutine rotAnim2 = TweenAnimator.instance.RunRotationAnimation(transform, new Vector3(0, 180, 0), time, time, FinishFlipCardAnim);
		animations.Add(rotAnim2);
	}

	private void FlipCard() {
		isReversed = !isReversed;
		transform.localScale = new Vector3(-1, 1, 1);
	}

	private void FinishFlipCardAnim() {
		ResetCardPositionAndRotation();
		Invoke("RunOnReverseCallback", Time.deltaTime);
	}

	private void RunOnReverseCallback() {
		OnReverseAnimationFinishCB.RunAction();
	}

	private void ResetCardPositionAndRotation() {
		transform.localScale = Vector3.one;
		transform.localRotation = Quaternion.identity;
		canvasGroup.blocksRaycasts = !_isReversed;
		SetLastGoodParametres();
	}

	public override void BackCardToParent() {
		StopCardAnim();
		ResetCardPositionAndRotation();
		base.BackCardToParent();
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (CheckIfCardHasProperParent())
		{
			animationQueueController.AddActionToQueue(ClickOnCard);
		}
		else
		{
			if (TryStopParentAnimation())
			{
				animationQueueController.AddActionToQueue(ClickOnCard);
				
			}
		}
	}

	private bool TryStopParentAnimation() {
		Card c = transform.parent.GetComponent<Card>();
		if (c != null)
		{
			if (c.StopCardAnim())
			{
				return true;
			}
		}
		return false;
	}

	private void ClickOnCard() {
		if (CheckIfCardHasProperParent())
		{
			if (!BoardManager.instance.TryAutoFitCard(this))
			{
				animationQueueController.CastNextAnimation();
				ShakeCardAnim();
			}
		}
		else
		{
			animationQueueController.CastNextAnimation();
		}
	}

	private bool CheckIfCardHasProperParent() {
		if (transform.parent.GetComponent<Waste>() != null && ItIsTopWasteCard())
			return true;
		if (transform.parent.GetComponent<Tableau>() != null)
			return true;
		return false;
	}

	private bool ItIsTopWasteCard() {
		return transform.GetSiblingIndex() == (transform.parent.childCount - 1);
	}

	public void ResetActions() {
		OnAnimationFinishCB = null;
		OnReverseAnimationFinishCB = null;
	}
}