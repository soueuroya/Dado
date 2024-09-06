using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Col : MonoBehaviour
{
    public ModButton up;
    public Die die;
    public ModButton down;

    public void Init(InputReader _inputReader, int row, int col)
    {
        die.Init(_inputReader, row, col);
        up.Init(_inputReader, row, col, true);
        down.Init(_inputReader, row, col, false);
    }

    public void Hide()
    {

    }

    public void Show()
    {

    }
}
