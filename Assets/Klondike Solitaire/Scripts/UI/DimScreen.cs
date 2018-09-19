using UnityEngine;
using System.Collections;

public class DimScreen : MonoBehaviour {
	public static DimScreen instance;
	public float speed;
	public float currentDim {
		get { return cg.alpha; }
		private set { cg.alpha = value; }
	}
	
	private Canvas topCanvas;
	private RectTransform rt;
	
	protected CanvasGroup cg;

	protected virtual void Awake() {
		instance = this;
		rt = transform as RectTransform;
		cg = GetComponent<CanvasGroup>();
		GameObject topUI = GameObject.FindGameObjectWithTag("TopUI");
		if (topUI != null)
			topCanvas = topUI.GetComponent<Canvas>();
	}

	public void MakeScreenDark(float darkDimValue = 0.65f)
	{
		cg.blocksRaycasts = true;
		TweenAnimator.instance.RunFadeAnimation(cg, darkDimValue, speed);
	}

	public void MakeScreenLight(float lightDimValue = 0f)
	{
		TweenAnimator.instance.RunFadeAnimation(cg, lightDimValue, speed, onComplete: () => cg.blocksRaycasts = false);
	}
	

	protected virtual void Update()
	{
		if(cg.alpha > 0)
			RefreshBar();
	}

	private void RefreshBar()
	{
		float top =  (topCanvas.scaleFactor / BehaviourSettings.instance.GetScaleFactor()) * Constants.TOP_BAR_SIZE;
		rt.offsetMax = new Vector2 (rt.offsetMax.x, -top);
	}
}
