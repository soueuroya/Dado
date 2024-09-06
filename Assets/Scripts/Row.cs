using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Row : MonoBehaviour
{
    public List<Col> cols = new List<Col>();
    public TextMeshProUGUI sumLabel;

    public void Init(InputReader _inputReader, int _row)
    {
        for (int i = 0; i < cols.Count; i++)
        {
            cols[i].Init(_inputReader, _row, i);
        }
    }
}
