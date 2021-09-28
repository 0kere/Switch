using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class sInfoHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltip;
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true);
    }

}
