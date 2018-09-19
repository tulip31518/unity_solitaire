using UnityEngine;
using System.Collections;

public class IHUD : MonoBehaviour {


	public void SetActive(bool state)
	{
		gameObject.SetActive(state);
	}

	public virtual void OnShow()
	{
		DimScreen.instance.MakeScreenDark();
		SetActive(true);
	}

	public virtual void OnBack()
	{
		DimScreen.instance.MakeScreenLight();
		SetActive(false);
	}
}
