using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Die : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image img;
    public Image blocker;
    public GameObject label;
    public int value = 0;
    public int blockerTween;
    public int labelTween;
    public int colorTween;
    private float colorSpeed = 0.5f;
    public Action onClick;
    public bool isSelected = false;
    private InputReader inputReader;
    private int row;
    private int col;

    public void Init(InputReader _inputReader, int _row, int _col)
    {
        inputReader = _inputReader;
        row = _row;
        col = _col;
    }

    public void HideDie()
    {
        LeanTween.value(0, 1, colorSpeed)
            .setOnUpdate((float alpha) =>
            {
                Image img = blocker;
                Color fadeColor = img.color;
                fadeColor.a = alpha;
                img.color = fadeColor;
            });
    }

    public void ShowDie(float delay = 0)
    {
        LeanTween.cancel(blockerTween);
        blockerTween = LeanTween.value(1, 0, colorSpeed)
            .setOnUpdate((float alpha) =>
            {
                Image img = blocker;
                Color fadeColor = img.color;
                fadeColor.a = alpha;
                img.color = fadeColor;
            }).setDelay(delay).id;
    }

    public void HideLabel(float delay = 0)
    {
        LeanTween.cancel(labelTween);
        labelTween = LeanTween.value(1, 0, colorSpeed)
            .setOnUpdate((float value) =>
            {
                label.transform.localScale = Vector3.one * value;
            }).setDelay(delay).id;
    }

    public void ShowLabel()
    {
        LeanTween.cancel(labelTween);
        labelTween = LeanTween.value(0, 1, colorSpeed)
            .setOnUpdate((float value) =>
            {
                label.transform.localScale = Vector3.one * value;
            }).id;
    }

    public void SetSelected(bool _selected)
    {
        if (_selected)
        {
            if (!isSelected)
            {
                //select
                isSelected = true;
                Paint(Color.cyan, 0, 3);
            }
            else
            {
                // do nothing
            }
        }
        else
        {
            if (isSelected)
            {
                isSelected = false;
                Paint(Color.white, 0, 3);
                // unselect
            }
            else
            {
                // do nothing
            }
        }
    }

    public void Paint(Color color, float delay = 0, float div = 1)
    {
        LeanTween.cancel(colorTween);
        colorTween = LeanTween.value(img.gameObject, setColorCallback, img.color, color, colorSpeed / div).setDelay(delay).id;
    }

    private void setColorCallback(Color c)
    {
        img.color = c;
        // For some reason it also tweens my image's alpha so to set alpha back to 1 (I have my color set from inspector). You can use the following
        var tempColor = img.color;
        tempColor.a = 1f;
        img.color = tempColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inputReader != null)
        {
            if (!isSelected && inputReader.currentTry == row && inputReader.animating == false)
            {
                Paint(Color.gray, 0, 3);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inputReader != null)
        {
            if (!isSelected && inputReader.currentTry == row && inputReader.animating == false)
            {
                Paint(Color.white, 0, 3);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inputReader != null)
        {
            if (!isSelected && inputReader.currentTry == row && inputReader.animating == false)
            {
                Paint(Color.cyan, 0, 3);
                onClick?.Invoke();
            }
        }
    }

    public void SetOnClick(Action _onClick)
    {
        onClick = _onClick;
    }
}