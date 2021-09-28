using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class sOnHoverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI textToUpdate;
    private TMPro.FontStyles defaultStyle;

    [HideInInspector] public bool disableHover;

    [SerializeField]private bool selected; //dont want to update style if option is selected. This is here to support the currently selected paint brush being bold
    private void Awake()
    {
        if (textToUpdate is object)
        {
            defaultStyle = textToUpdate.fontStyle;
        }
        else
        {
            Debug.Log(gameObject.name);
        }
    }

    private void OnDisable()
    {
        textToUpdate.fontStyle = defaultStyle;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
        { 
            textToUpdate.fontStyle = defaultStyle;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!disableHover && !selected)
            textToUpdate.fontStyle = FontStyles.Bold;
    }

    public void ToggleSelected()
    {
        selected = !selected;
        if (selected)
        {
            textToUpdate.fontStyle = FontStyles.Bold;
        }
        else
        {
            textToUpdate.fontStyle = defaultStyle;
        }
    }

}
