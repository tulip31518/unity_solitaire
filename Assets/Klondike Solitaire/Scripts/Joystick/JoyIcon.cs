using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyIcon : MonoBehaviour{

	public float moveSpeed = 8f;
	public bool b_trigger_enable = false;
	public GameObject joyButton;
    public Joystick joystick;
	public GameObject option;
	public GameObject saveManager;
	public GameObject hintManager;
	public GameObject uiManager;
	public GameObject chooseGameMode;
	public GameObject helpScreen;
	public GameObject draw1Toggle;
	public GameObject draw3Toggle;
	public GameObject normalScoreToggle;
	public GameObject vegasScoreToggle;
	public GameObject stock;
	public GameObject T0;
	public GameObject T1;
	public GameObject T2;
	public GameObject T3;
	public GameObject T4;
	public GameObject T5;
	public GameObject T6;
	public GameObject waste;
	public GameObject autocomplete;
	public GameObject finish;
	
	private string currentGameObjectName = "";
	private string oldString = "";
	public float timeInterval = 0.5f;
	private float time = 0f;
	private Card selectedCard;

	void Start()
	{
		time = Time.time;
	}
    private void Update()
    {
		// if(Time.time - time < timeInterval) return;
		// oldString = "";

		Vector3 v_right = new Vector3(Input.GetAxis("Horizontal") * 40f, 0, 0);
		Vector3 v_up = new Vector3(0, Input.GetAxis("Vertical") * 40f, 0);
        Vector3 moveVector = (Vector3.right * joystick.Horizontal * 40f + v_right
			 + Vector3.up * joystick.Vertical * 40f + v_up);

        if (moveVector != Vector3.zero)
        {             
            transform.Translate(moveVector * moveSpeed * Time.deltaTime, Space.World);
        }

		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		Vector2 pos = this.transform.position;
		pointerEventData.position = pos;		

		if(b_trigger_enable)
		{
			if(collided_object != null)
			{
				if(collided_object.gameObject.GetComponent<Card>() != null)
				{			
					Card card = collided_object.gameObject.GetComponent<Card>();
					if(collided_object.transform.parent.gameObject.GetComponent<Tableau>() != null && !card.isReversed)
					{
						collided_object.gameObject.GetComponent<Card>().OnPointerClick(pointerEventData);
					}
				}
				refreshAllCollider();
				//collided_object = null;
			}
			switch(currentGameObjectName)
			{
				// Bottom Menu
				case "Setting"	: 	option.SetActive(true);	break;
				case "Undo"		: 	saveManager.GetComponent<SaveManager>().TryUndo(); break;
				case "Hint"		: 	hintManager.GetComponent<HintManager>().TryHint(); break;

				// Option Dialog
				case "NewGame"	: 	chooseGameMode.SetActive(true);  
									option.SetActive(false);									
									uiManager.GetComponent<UIManager>().setPopupID(1);
									break;
				case "HelpBtn"	:	option.SetActive(false);
									helpScreen.SetActive(true);									
									break;
				case "Restart"	:	uiManager.GetComponent<BoardManager>().Replay();
									option.SetActive(false);
									break;
				case "Back"	:		option.SetActive(false);
									break;
				case "ExitGame"	:	uiManager.GetComponent<BoardManager>().Exit();
									option.SetActive(false);
									break;
				// Choose Game Mode Dialog
				case "Draw1Toggle"	:		//uiManager.GetComponent<MatchStatistics>().Exit();
											draw1Toggle.GetComponent<Toggle>().isOn = false;
											draw3Toggle.GetComponent<Toggle>().isOn = true;
											uiManager.GetComponent<MatchStatistics>().OnDraw3ToggleSetting(true);
											break;
				case "Draw3Toggle"	:		uiManager.GetComponent<MatchStatistics>().OnDraw3ToggleSetting(true);
											draw1Toggle.GetComponent<Toggle>().isOn = true;
											draw3Toggle.GetComponent<Toggle>().isOn = false;
											break;
				case "NormalScoreToggle"	:	uiManager.GetComponent<MatchStatistics>().OnNormalScoreToggleSetting(true);
											vegasScoreToggle.GetComponent<Toggle>().isOn = true;
											normalScoreToggle.GetComponent<Toggle>().isOn = false;
											break;
				case "VegasScoreToggle"	:	uiManager.GetComponent<MatchStatistics>().OnVegasScoreToggleSetting(true);
											vegasScoreToggle.GetComponent<Toggle>().isOn = false;
											normalScoreToggle.GetComponent<Toggle>().isOn = true;
											break;
				case "Start"	:			uiManager.GetComponent<BoardManager>().NewGame();
											chooseGameMode.SetActive(false);
											break;
				case "CloseGameSetup"	:	uiManager.GetComponent<UIManager>().OnClosePopup();
											chooseGameMode.SetActive(false);
											break;
				// Choose Game Mode Dialog
				case "CloseHelp"		:	option.SetActive(true);
											helpScreen.SetActive(false);
											break;

				// Click Stock
				case "StockCards"	:	if(stock.GetComponent<Stock>() != null)
										{		
											PointerEventData pEventData = new PointerEventData(EventSystem.current);
											Vector2 p = this.transform.position;
											pEventData.position = p;								
											stock.GetComponent<Stock>().OnPointerClick(pEventData);
										}
										break;
				// Click Waste Card
				case "Waste"	:	if(waste.GetComponent<Waste>() != null)
									SelectwasteLastCard(waste.GetComponentsInChildren<Card>(), pointerEventData);
								break;
				// Click AutoComplete
				case "AutocompleteBtn"	:	if(autocomplete.GetComponent<Autocomplete>() != null)
												autocomplete.GetComponent<Autocomplete>().StartAutocomplete();
								break;
				// Click NewGameBtn
				case "NewGameBtn"	:chooseGameMode.SetActive(true);
									finish.SetActive(false);
									uiManager.GetComponent<UIManager>().setPopupID(2);
									break;

				default: break;
			}

			b_trigger_enable = false;
		}
    }
	
	private Collider2D collided_object;
	void OnTriggerEnter2D(Collider2D coll)
	{		
		currentGameObjectName = coll.gameObject.name;
		collided_object= coll;
    }

	void OnTriggerExit2D(Collider2D coll)
	{		
		currentGameObjectName = "";	
		collided_object = null;
    }

	void SelectLastCard(Card[] cards)
	{
		if(cards.Length == 0) return;
		
		Card c = cards[0];
		for(int i = 0; i < cards.Length; i++)
		{		
			BoxCollider2D bc = cards[i].gameObject.GetComponent<BoxCollider2D>();
			Vector2 sz = bc.size;
			Vector2 offset = bc.offset;
			if(i == cards.Length - 1)
			{
				sz.y = 180;
				offset.y = 0;
			}	
			else
			{
				sz.y = 55;
				offset.y = 65;
			}			
			bc.size = sz;
			bc.offset = offset;		
			
		}
		
	}

	void refreshAllCollider()
	{
		SelectLastCard(T0.GetComponentsInChildren<Card>());
		SelectLastCard(T1.GetComponentsInChildren<Card>());
		SelectLastCard(T2.GetComponentsInChildren<Card>());
		SelectLastCard(T3.GetComponentsInChildren<Card>());
		SelectLastCard(T4.GetComponentsInChildren<Card>());
		SelectLastCard(T5.GetComponentsInChildren<Card>());
		SelectLastCard(T6.GetComponentsInChildren<Card>());
	}

	void SelectwasteLastCard(Card[] cards, PointerEventData pointerEventData)
	{
		if(cards.Length == 0) return;
		Card c = cards[0];
		for(int i = 0; i < cards.Length; i++)
		{
			if(c.transform.position.x < cards[i].transform.position.x)
				c = cards[i];
		}
		c.OnPointerClick(pointerEventData);	
	}
}
