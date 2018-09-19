using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class Autocomplete : MonoBehaviour {
	public static Autocomplete instance;
	private Button newGameBtn;
	private Button restartGameBtn;
	private Stock stock;
	private CanvasGroup cg;
	private bool canShowAgain = true;

	private void Awake() {
		instance = this;
		cg = GetComponent<CanvasGroup>();
		Time.timeScale = 1f;
		stock = FindObjectOfType<Stock>();
		SetupGameRestartBtns();
		ChangeVisibility(false);
	}

	public void OnShow() {
		ChangeVisibility(true);
	}

	public void OnHide() {
		ChangeVisibility(false);
	}

	private void ChangeVisibility(bool state) {
		cg.interactable = state;
		cg.blocksRaycasts = state;
		cg.alpha = state ? 1 : 0;
	}

	private void SetupGameRestartBtns() {
		try
		{
			newGameBtn = GameObject.Find("NewGameBtn").GetComponent<Button>();
			restartGameBtn = GameObject.Find("ReplayBtn").GetComponent<Button>();
		}
		catch (Exception e)
		{
			// Debug.LogError("Empty object " + e.Message);
		}
	}

	public void SetButtonState(bool state) {
		if (state && canShowAgain)
		{
			OnShow();
			canShowAgain = false;
		}
	}

	public void StartAutocomplete() {
		OnHide();
		BoardManager.instance.LockBoard();
		StartCoroutine(AutocompleteCoroutine());
	}

	private void SetInteractableOfResetGameBtns(bool state) {
		if (restartGameBtn != null && newGameBtn != null)
		{
			restartGameBtn.interactable = state;
			newGameBtn.interactable = state;
		}
	}

	private IEnumerator AutocompleteCoroutine() {
		Time.timeScale = Constants.AUTOCOMPLETE_TIME_SCALE;
		SetInteractableOfResetGameBtns(false);
		do
		{
			stock.GetCard();
			yield return new WaitForSeconds(Constants.ANIM_TIME);
			List<Card> tableauCards = BoardManager.instance.GetOnTopTableauCards();
			for (int i = 0; i < tableauCards.Count; i++)
			{
				yield return TryFitCard(tableauCards[i]);
			}

			Card wasteCard = BoardManager.instance.GetCurrentWasteCards(false).FirstOrDefault();

			yield return TryFitCard(wasteCard);
			BoardManager.instance.CheckIfGameComplete();
		} while (!BoardManager.instance.isGameWon);
		Time.timeScale = 1f;
		SetInteractableOfResetGameBtns(true);
		BoardManager.instance.UnlockBoard();
	}

	private IEnumerator TryFitCard(Card card) {
		if (card != null)
		{
			if (BoardManager.instance.TryAutoFitCard(card))
				yield return new WaitForSeconds(Constants.ANIM_TIME);
		}
	}

	private void ResetState() {
		canShowAgain = true;
	}
}