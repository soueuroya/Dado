using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI label;
    public string original;
    public string target;

    private void OnValidate()
    {
        label = GetComponent<TextMeshProUGUI>();
        original = label.text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        label.text = target;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        label.text = original;
    }
}
