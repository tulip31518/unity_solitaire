using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BehaviourSettings : MonoBehaviour {

	public static BehaviourSettings instance { get; private set; }
	public bool isTestMode;
	private Canvas canvas;

	void Awake()
	{
		Application.targetFrameRate = 60;
		instance = this;
		Input.multiTouchEnabled = false;
		canvas = GetComponent<Canvas>();
	}

	public float GetScaleFactor()
	{
		return canvas.scaleFactor;
	}

	public float CalculateCardsOffset(float cardHeight)
	{
		float offset = ScreenOrientationSwitcher.IsPortrait() ? Constants.TABLEAU_SPACING : Constants.LANDSCAPE_TABLEAU_SPACING;
		return (cardHeight + offset);
	}

  
}
