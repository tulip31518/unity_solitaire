using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DropZone : MonoBehaviour, IDropHandler {
	private LayoutGroup layoutGroup;
	private CanvasGroup canvasGroup;
	GameObject raycastHelper;

	public virtual void AssignNewChild(Transform child)
	{
		SoundManager.instance.PlayDropCardSound();
		child.SetParent(transform,true);
	}

	protected virtual void Awake() {
		layoutGroup = GetComponent<LayoutGroup>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	private float CalculateSpacing()
	{
		if (layoutGroup is HorizontalOrVerticalLayoutGroup)
		{
			return  (layoutGroup as HorizontalOrVerticalLayoutGroup).spacing;
		}
		else if (layoutGroup is GridLayoutGroup)
		{
			return (layoutGroup as GridLayoutGroup).spacing.y;
		}
		return 0;
	}

	virtual public void OnDrop(PointerEventData eventData) {
		if (eventData != null) {
			Card c = eventData.pointerDrag.GetComponent<Card>();
			OnDrop (c);
		}
	}

	virtual public void OnDrop(Card card) {
		if (card != null) {
            card.SetReturnPoint(transform, CalculatePosition(card.size.y));
		}
	}

	private Vector3 CalculatePosition(float height) {
		if(transform.childCount > 0) {
			float spacing = CalculateSpacing();
			float offset =  (height + spacing)*BehaviourSettings.instance.GetScaleFactor();
			return transform.GetChild(transform.childCount - 1).position - new Vector3(0,offset, 0) - transform.position;
		}

		return Constants.vectorZero;
	}

	private void LockZone(float unlockTime) {
		canvasGroup.blocksRaycasts = false;
		CancelInvoke("UnlockZone");
		Invoke("UnlockZone",unlockTime);
	}

	private void UnlockZone() {
		canvasGroup.blocksRaycasts = true;
	}

	private void RefreshZone()
	{
		if (layoutGroup == null)
		{
			return;
		}

		layoutGroup.enabled = false;
		layoutGroup.enabled = true;
	}

	private void ActivateExtraZone()
	{
		Vector3 pos = transform.position;
		if (transform.childCount > 0)
			pos = transform.GetChild(transform.childCount - 1).position;
		pos += Vector3.up * CalculateSpacing() * BehaviourSettings.instance.GetScaleFactor()/2;
		raycastHelper = Instantiate(Resources.Load("RaycastHelper"), pos, Quaternion.identity, transform) as GameObject;
	}
	
	private void DeactivateExtraZone()
	{

		if (raycastHelper != null)
		{
			raycastHelper.transform.SetParent(transform.root);
			Destroy(raycastHelper);
		}
	}
}
