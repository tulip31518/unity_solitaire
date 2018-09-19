using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Waste : DropZone {
	
	private float offset = 50;
	private Action OnRefreshWaste;

	public void RegisterOnRefreshWasteAction(Action cb)
	{
		OnRefreshWaste = cb;
	}
	
	protected override void Awake()
	{
		base.Awake();
	}

	public override void OnDrop(PointerEventData eventData)
	{
	  //Can't put other cards to waste
	}

	public override void AssignNewChild(Transform child)
	{
		base.AssignNewChild(child);
		RefreshChildren();
	}

	public void RefreshChildren()
	{
		if (IsInvoking("Refresh"))
		{
			CancelInvoke("Refresh");
		}

		Invoke("Refresh", Time.fixedDeltaTime);
	}

	private void Refresh()
	{
		if (IsInvoking("RefreshCardsPos"))
		{
			CancelInvoke("RefreshCardsPos");
		}
		float posModifier = 0;
		float animSpeed = Constants.STOCK_ANIM_TIME / 2;
		CleanChildrenFromRaycastHelpers();
		int noOfChilds = transform.childCount;

		for (int i = 0; i < noOfChilds ; i++)
		{
			Transform t = transform.GetChild(i);
		  
			t.SendMessage("UnlockCard",false);

			if (i== noOfChilds -1)
			{
				posModifier = 0;
				t.SendMessage("UnlockCard",true);
			}
			else if (i == noOfChilds - 2)
			{
				posModifier = 1;
			}
			else
			{
				posModifier = 2;

			}
			MoveOnPosition(t, Constants.vectorRight * posModifier * offset, animSpeed);
		}
		Invoke("RefreshCardsPos", animSpeed + Time.deltaTime);
	}

	private void CleanChildrenFromRaycastHelpers()
	{
		List<Transform> childrenToRemove = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform t = transform.GetChild(i);
			if (CheckIfItIsValidChild(t) == false)
			{
				childrenToRemove.Add(t);
				continue;
			}
		}

		for (int i = 0; i < childrenToRemove.Count; i++)
		{
			childrenToRemove[i].SetParent(transform.root);
			Destroy(childrenToRemove[i].gameObject);
		}
	}

	private bool CheckIfItIsValidChild(Transform t)
	{
		if (t.CompareTag("RaycastHelper"))
			return false;
		return true;
	}

	void RefreshCardsPos()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Card card = transform.GetChild(i).GetComponent<Card>();
			if (card != null)
				card.SetLastGoodParametres();
		}
		OnRefreshWaste.RunAction();
	}

	private void MoveOnPosition(Transform t, Vector3 pos, float animSpeed)
	{
		t.localPosition = -pos;
	}

	public bool IsWasteEmpty()
	{
		return transform.childCount == 0 ;
	}
}
