using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool increase;
    public TextMeshProUGUI label;
    public List<Image> imgs;
    private float colorSpeed = 0.5f;
    public bool isSelected = false;
    public int colorTween;
    private InputReader inputReader;
    private int row;
    private int col;
    public bool controlButton = false;
    public UnityEvent onClick;
    public bool inverColor = false;

    public void Init(InputReader _inputReader, int _row, int _col, bool _increase)
    {
        inputReader = _inputReader;
        row = _row;
        col = _col;
        increase = _increase;
    }

    public void Paint(Color color, float delay = 0, float div = 1, Action callback = null)
    {
        LeanTween.cancel(colorTween);

        if (label != null)
        {
            colorTween = LeanTween.value(label.gameObject, setColorCallback, label.color, color, colorSpeed / div).setDelay(delay).setOnComplete(() => { callback?.Invoke(); }).id;
        }
        else
        {
            foreach (var item in imgs)
            {
                colorTween = LeanTween.value(item.gameObject, setColorCallback, item.color, color, colorSpeed / div).setDelay(delay).setOnComplete(() => { callback?.Invoke(); }).id;
            }
        }
    }

    private void setColorCallback(Color c)
    {
        if (label != null)
        {
            label.color = c;
            // For some reason it also tweens my image's alpha so to set alpha back to 1 (I have my color set from inspector). You can use the following
            var tempColor = label.color;
            tempColor.a = 1f;
            label.color = tempColor;
        }
        else
        {
            foreach (var item in imgs)
            {
                item.color = c;
                // For some reason it also tweens my image's alpha so to set alpha back to 1 (I have my color set from inspector). You can use the following
                var tempColor = item.color;
                tempColor.a = 1f;
                item.color = tempColor;
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            Paint(Color.gray, 0, 3);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            if (inverColor)
            {
                Paint(Color.black, 0, 3);
            }
            else
            {
                Paint(Color.white, 0, 3);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSelected && !controlButton)
        {
            Paint(Color.cyan, 0, 4, () => {
                Paint(Color.gray, 0, 4);
            });

            if (increase)
            {
                inputReader.IncreaseSpecific(row, col);
            }
            else
            {
                inputReader.DecreaseSpecific(row, col);
            }
        }
        else
        if (controlButton)
        {
            if (inverColor)
            {
                Paint(Color.cyan, 0, 4, () => {
                    Paint(Color.black, 0, 4);
                });
            }
            else
            {
                Paint(Color.cyan, 0, 4, () => {
                    Paint(Color.gray, 0, 4);
                });
            }
            onClick?.Invoke();
        }
    }
}
