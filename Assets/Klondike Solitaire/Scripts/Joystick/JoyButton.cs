using UnityEngine;
using UnityEngine.EventSystems;
public class JoyButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

	public GameObject joyicon;
    [HideInInspector]
    public bool Pressed = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire2"))
            joyicon.GetComponent<JoyIcon>().b_trigger_enable = true;        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
		joyicon.GetComponent<JoyIcon>().b_trigger_enable = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }
}
