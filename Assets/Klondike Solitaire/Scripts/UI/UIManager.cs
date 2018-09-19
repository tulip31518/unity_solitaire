using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour {

	IHUD[] UIScreens;
	public static UIManager instance;
	private AnimationQueueController animationQueueController;

	void Awake()
	{

		instance = this;
		UIScreens = GetComponentsInChildren<IHUD>(true);
		animationQueueController = FindObjectOfType<AnimationQueueController>();
		for (int i = 0; i < UIScreens.Length; i++)
		{
			UIScreens[i].SetActive(true);
		}
	}

	public void ResetGameState()
	{
		BroadcastMessage("ResetState");
		animationQueueController.ResetState();
	}

    public int popupId = 0;
    public GameObject option, nomore;
    public void OnClosePopup()
    {
        if (popupId == 1)
        {//option
            option.SetActive(true);
        }
        else if (popupId == 2)
        {//no more
            nomore.SetActive(true);
        }
    }
    public void setPopupID(int id)
    {
        popupId = id;
    }
}
