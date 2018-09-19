using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ScreenOrientationSwitcher : MonoBehaviour {

	public static ScreenOrientationSwitcher instance;
	ScreenOrientation currentOrientation;
	RectTransform tableauTransform;
	VerticalLayoutGroup[] tableauLayoutGroups;
	RectTransform topGridTransform;
	GridLayoutGroup topGridLayoutGroup;
	static bool isPortrait = true;
	bool inited;
	float landscapeWidth;
	HintManager hintManager;
	bool cooldown;

	public bool LockOrientation { get; set;}
	public static Action OnOrientationSwitchToPortrait;
	public static Action OnOrientationSwitchToLandscape;

	float tableauPositionX;


	bool locked;

	private void Awake()
	{
		instance = this;
	}

	void Start() {
		isPortrait = true;
		hintManager = FindObjectOfType<HintManager>();
		tableauTransform = GameObject.FindGameObjectWithTag("Tableau").GetComponent<RectTransform>();
		tableauLayoutGroups = tableauTransform.GetComponentsInChildren<VerticalLayoutGroup>();
		topGridTransform = GameObject.FindGameObjectWithTag("TopGrid").GetComponent<RectTransform>();
		topGridLayoutGroup = topGridTransform.GetComponent<GridLayoutGroup>();

		tableauPositionX = tableauTransform.anchoredPosition.x;

		landscapeWidth = Screen.width > Screen.height ? Screen.width : Screen.height;

		inited = true;


		currentOrientation = ScreenOrientation.AutoRotation;
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.Landscape || Screen.orientation == ScreenOrientation.LandscapeLeft)
		{
			currentOrientation = ScreenOrientation.LandscapeLeft;
			Screen.orientation = currentOrientation;
			SwitchScreenToLandscape();
		}
		else if (Input.deviceOrientation == DeviceOrientation.LandscapeRight || Screen.orientation == ScreenOrientation.LandscapeRight)
		{
			currentOrientation = ScreenOrientation.LandscapeRight;
			Screen.orientation = currentOrientation;
			SwitchScreenToLandscape();
		}
		else if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
		{
			currentOrientation = ScreenOrientation.PortraitUpsideDown;
			Screen.orientation = currentOrientation;
			SwitchScreenToPortrait();
		}
		else if (Input.deviceOrientation == DeviceOrientation.FaceDown || Input.deviceOrientation == DeviceOrientation.FaceUp || Input.deviceOrientation == DeviceOrientation.Portrait)
		{
			currentOrientation = ScreenOrientation.Portrait;
			Screen.orientation = currentOrientation;
			SwitchScreenToPortrait();
		}
	}

	void Update()
	{
	   if(locked && !LockOrientation)
		{
			ResetCooldown();
			locked = false;
		}

		if (cooldown) 
			return;


		if (LockOrientation)
		{
			ForceLock();
			return;
		}

		switch (Input.deviceOrientation)
		{
			case DeviceOrientation.Unknown:
				break;
			case DeviceOrientation.Portrait:
				if( currentOrientation != ScreenOrientation.Portrait)
				{
					currentOrientation = ScreenOrientation.Portrait;
				   Screen.orientation = currentOrientation;
					SwitchScreenToPortrait();
				}
				break;
			case DeviceOrientation.PortraitUpsideDown:
				if (currentOrientation != ScreenOrientation.PortraitUpsideDown)
				{
					currentOrientation = ScreenOrientation.PortraitUpsideDown;
					Screen.orientation = currentOrientation;
					SwitchScreenToPortrait();
				}
				break;
			case DeviceOrientation.LandscapeLeft:
				if (currentOrientation != ScreenOrientation.LandscapeLeft)
				{
					currentOrientation = ScreenOrientation.LandscapeLeft;
					Screen.orientation = currentOrientation;
					SwitchScreenToLandscape();
				}
				break;
			case DeviceOrientation.LandscapeRight:
				if (currentOrientation != ScreenOrientation.LandscapeRight)
				{
					currentOrientation = ScreenOrientation.LandscapeRight;
					Screen.orientation = currentOrientation;
					SwitchScreenToLandscape();
				}
				break;
			case DeviceOrientation.FaceUp:
				break;
			case DeviceOrientation.FaceDown:
				break;
			default:
				break;
		}
	}

	private void ForceLock()
	{
		Screen.orientation = currentOrientation;
		cooldown = true;
		locked = true;
	}

	public static bool IsPortrait()
	{
		return isPortrait;
	}

	public void SwitchScreenToLandscape() {
		if (!inited)
			return;
		cooldown = true;
		isPortrait = false;
		if (hintManager != null)
			hintManager.StopHintMode();
		BroadcastMessage("StopCardAnim");
		RefreshLandscape();
		OnOrientationSwitchToLandscape.RunAction();
		new Timer(1f, ResetCooldown);
	}

	private void RefreshLandscape()
	{
		float landscapeOffset = 100;
		tableauTransform.anchoredPosition = new Vector2(tableauPositionX - landscapeOffset, -100);
		SetVerticalTableauOffset(Constants.LANDSCAPE_TABLEAU_SPACING);
		float topGridHeight = 900;
		topGridTransform.sizeDelta = new Vector2(topGridTransform.sizeDelta.x, topGridHeight);
		topGridTransform.anchoredPosition = new Vector2(topGridTransform.anchoredPosition.x, -100);
		topGridLayoutGroup.padding = new RectOffset(30, 0, 0, 0);
		topGridLayoutGroup.spacing = new Vector2((landscapeWidth / BehaviourSettings.instance.GetScaleFactor()) - 170 + 2 * Constants.TABLEAU_SPACING, 20);
		topGridLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
		topGridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
	}

	private void SwitchScreenToPortrait()
	{
		cooldown = true;
		isPortrait = true;
		hintManager.StopHintMode();
		BroadcastMessage("StopCardAnim");
		tableauTransform.anchoredPosition = new Vector2(tableauPositionX, -340);
		SetVerticalTableauOffset(Constants.TABLEAU_SPACING);
		topGridTransform.anchoredPosition = new Vector2(topGridTransform.anchoredPosition.x, -100);
		topGridLayoutGroup.padding = new RectOffset(0, 0, 0, 0);
		topGridLayoutGroup.spacing = new Vector2(20, 0);
		topGridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
		topGridLayoutGroup.childAlignment = TextAnchor.UpperCenter;
			OnOrientationSwitchToPortrait.RunAction();
		new Timer(1f, ResetCooldown);
	}

	void ResetCooldown()
	{
		Screen.orientation = ScreenOrientation.AutoRotation;
		cooldown = false;

	}

	private void SetVerticalTableauOffset(float offset)
	{
		for (int i = 0; i < tableauLayoutGroups.Length; i++)
		{
			tableauLayoutGroups[i].spacing = offset;
		}
	}
}
