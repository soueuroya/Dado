using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputReader : MonoBehaviour
{
    public TextMeshProUGUI sumLabel;
    public List<Row> rows;
    public List<Sprite> sprites;
    public Row headers;
    public Row points;
    [SerializeField] private List<int> dice = new List<int>() { 1, 0, 2, 1, 4, 2 };
    public bool openRows = true;
    public bool closedRows = true;
    public bool showHeader = false;
    public bool collapseFirst = false;
    public bool collapseRest = false;
    public bool gameover = false;
    private int found = 0;
    private int score = 0;
    private int currentOpenRow = -1;
    public int currentTry = 0;
    private int currentRow = 0;
    private int currentCol = 0;
    private float colorDelay = 0.5f;
    public bool animating = false;
    public GameObject controls;

    Action callbackTemp;

    //[DllImport("__Internal")]
    //private static extern bool IsMobile();
    //
    public bool isMobile()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        return false;
        //return IsMobile();
#elif UNITY_IOS || UNITY_ANDROID
        return true;
#endif
        //controls.SetActive(false);
        return false;
    }

    private void OnValidate()
    {
        if (showHeader)
        {
            headers.gameObject.SetActive(true);
        }
        else
        {
            headers.gameObject.SetActive(false);
        }

        if (collapseRest)
        {
            ColapseAllButtons();   
        }
        else
        {
            OpenAllButtons();
        }

        if (!collapseFirst)
        {
            OpenButtonsAtRow(0);
        }
    }

    private void Start()
    {
        Initiate(); // gets date -> creates random sequence -> setup dice for header
        UpdateHeaders(); // updates header dice sprites and color
        UnlockCurrentTryRow(); // Unlock the visual of dice for the current row
        if (isMobile())
        OpenButtonsAtRow(currentTry); // unlocks the controls for the current row
        HighlightCurrentDie(); // Highlights current die
        UpdateCurrentTryRowSum(); // Counts current row sum and updates the row label
    }

    void Update()
    {
        if (gameover)
            return;

        if (Input.GetKeyDown(KeyCode.O))
        {
            OpenAllButtons();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ShowAnswer();
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            HideAnswer();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitCurrentTryRow();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            IncreaseCurrent();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DecreaseCurrent();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveToPrev();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveToNext();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            UpdateHeaders();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            ColapseAllButtons();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //OpenButtonsAtRow(0);
            SetCurrent(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //OpenButtonsAtRow(1);
            SetCurrent(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //OpenButtonsAtRow(2);
            SetCurrent(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //OpenButtonsAtRow(3);
            SetCurrent(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            //OpenButtonsAtRow(4);
            SetCurrent(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            //OpenButtonsAtRow(5);
            SetCurrent(5);
        }
    }
    public void Initiate()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].Init(this, i);
        }

        int date = int.Parse(DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day);
        System.Random random = new System.Random(date);
        int sum = 0;
        for (int i = 0; i < headers.cols.Count; i++)
        {
            dice[i] = random.Next(5);
            sum += dice[i] + 1;
        }
        headers.sumLabel.gameObject.SetActive(true);
        headers.sumLabel.text = (sum).ToString(); // Number at the end of the row
        sumLabel.gameObject.SetActive(true);
        sumLabel.text = (sum).ToString(); // Number at the top of the game
    }

    public void UpdateHeaders()
    {
        for (int i = 0; i < headers.cols.Count; i++)
        {
            headers.cols[i].die.img.sprite = sprites[dice[i]];
            headers.cols[i].die.value = dice[i];
        }
    }

    public void SubmitCurrentTryRow()
    {
        if (animating)
            return;

        CloseButtonsAtRow(currentTry);
        animating = true;
        PaintColors(() => { 
            currentTry++;
            currentRow++;
            currentCol = 0;

            if (found >= 6)
            {
                FinishGame(); // congratz!
            }
            else
            if (currentTry >= 0 && currentTry < rows.Count)
            {
                UnlockCurrentTryRow();
                UpdateCurrentTryRowSum();
                if(isMobile())
                OpenButtonsAtRow(currentTry);
                HighlightCurrentDie();
            }
            else
            {
                FinishGame(); // game over
            }
            animating = false;
        });
    }

    public void ShowAnswer()
    {
        for (int i = 0; i < headers.cols.Count; i++)
        {
            headers.cols[i].die.ShowDie();
            headers.cols[i].die.HideLabel();
        }
    }

    public void HideAnswer()
    {
        for (int i = 0; i < headers.cols.Count; i++)
        {
            headers.cols[i].die.HideDie();
            headers.cols[i].die.ShowLabel();
        }
    }

    public void PaintColors(Action callback)
    {
        callbackTemp = callback;
        int index = 0;
        for (int i = 0; i < rows[currentTry].cols.Count; i++)
        {
            if (rows[currentTry].cols[i].die.value == dice[i]) // correct, paint green
            {
                float delay = index * colorDelay;
                if (rows[currentTry].cols[i].die.img.color != Color.green)
                {
                    index++;
                    delay = index * colorDelay;
                    rows[currentTry].cols[i].die.Paint(Color.green, delay);
                }

                if (headers.cols[i].die.img.color != Color.blue)
                {
                    score += (6 - currentTry);
                    found++;
                    headers.cols[i].die.Paint(Color.blue, delay); // 0.5f bcs we have to add the animation time as well
                    headers.cols[i].die.ShowDie(delay);
                    headers.cols[i].die.HideLabel(delay);
                }
            }
            else if (dice.Contains(rows[currentTry].cols[i].die.value)) // wrong position, paint yellow
            {
                if (rows[currentTry].cols[i].die.img.color != Color.yellow)
                {
                    index++;
                    float delay = index * colorDelay;
                    rows[currentTry].cols[i].die.Paint(Color.yellow, delay);
                }
            }
            else
            {
                float delay = index * colorDelay;
                if (rows[currentTry].cols[i].die.isSelected)
                    rows[currentRow].cols[currentCol].die.SetSelected(false);
            }
        }

        Invoke("CallCallback", index * colorDelay + 0.5f);
    }

    public void FinishGame()
    {
        UpdateScoreRow();
        gameover = true;
    }

    public void CallCallback()
    {
        callbackTemp?.Invoke();
    }

    public void SetSelectedDie(int row, int col)
    {
        if (animating)
            return;

        if (row != currentTry)
            return;

        rows[currentRow].cols[currentCol].die.SetSelected(false); // Unhighlight current selected

        currentRow = row;
        currentCol = col;
        HighlightCurrentDie();
    }

    public void MoveToNext()
    {
        if (animating)
            return;

        if (currentRow == currentTry)
            rows[currentRow].cols[currentCol].die.SetSelected(false); // unhighlight

        currentCol++;
        if (currentCol >= rows[currentRow].cols.Count)
        {
            currentCol = 0;
            currentRow++;

            if (currentRow > currentTry) // change to >  rows count if want to make able to highlight future rows
            {
                currentRow--;
                currentCol = rows[currentRow].cols.Count - 1;
                //went forward all the way
            }
        }

        if (currentRow == currentTry)
            rows[currentRow].cols[currentCol].die.SetSelected(true);
    }

    public void MoveToPrev()
    {
        if (animating)
            return;

        if (currentRow == currentTry)
            rows[currentRow].cols[currentCol].die.SetSelected(false); // unhighlight

        currentCol--;
        if (currentCol < 0)
        {
            currentRow--;

            if (currentRow < currentTry) // change to < 0 if want to make able to highlight prev rows 
            {
                currentRow++;
                currentCol = 0;
                //went back all the way
            }
            else
            {
                currentCol = rows[currentRow].cols.Count - 1;
            }
        }

        if (currentRow == currentTry)
            rows[currentRow].cols[currentCol].die.SetSelected(true);
    }

    public void IncreaseCurrent()
    {
        if (animating)
            return;

        if (currentTry == currentRow)
        {
            rows[currentTry].cols[currentCol].die.value++;
            if (rows[currentTry].cols[currentCol].die.value > rows[currentTry].cols.Count-1)
            {
                rows[currentTry].cols[currentCol].die.value = 0;
            }
            rows[currentTry].cols[currentCol].die.img.sprite = sprites[rows[currentTry].cols[currentCol].die.value];
            UpdateCurrentTryRowSum();
        }
    }

    public void DecreaseCurrent()
    {
        if (animating)
            return;

        if (currentTry == currentRow)
        {
            rows[currentTry].cols[currentCol].die.value--;
            if (rows[currentTry].cols[currentCol].die.value < 0)
            {
                rows[currentTry].cols[currentCol].die.value = rows[currentTry].cols.Count - 1;
            }
            rows[currentTry].cols[currentCol].die.img.sprite = sprites[rows[currentTry].cols[currentCol].die.value];
            UpdateCurrentTryRowSum();
        }
    }

    public void IncreaseSpecific(int row, int col)
    {
        if (animating)
            return;

        if (currentTry == row)
        {
            rows[currentTry].cols[col].die.value++;
            if (rows[currentTry].cols[col].die.value > rows[currentTry].cols.Count - 1)
            {
                rows[currentTry].cols[col].die.value = 0;
            }
            rows[currentTry].cols[col].die.img.sprite = sprites[rows[currentTry].cols[col].die.value];
            UpdateCurrentTryRowSum();
        }
    }

    public void DecreaseSpecific(int row, int col)
    {
        if (animating)
            return;

        if (currentTry == row)
        {
            rows[currentTry].cols[col].die.value--;
            if (rows[currentTry].cols[col].die.value < 0)
            {
                rows[currentTry].cols[col].die.value = rows[currentTry].cols.Count - 1;
            }
            rows[currentTry].cols[col].die.img.sprite = sprites[rows[currentTry].cols[col].die.value];
            UpdateCurrentTryRowSum();
        }
    }

    public void SetCurrent(int value)
    {
        if (animating)
            return;

        if (currentTry == currentRow)
        {
            rows[currentTry].cols[currentCol].die.value = value;
            rows[currentTry].cols[currentCol].die.img.sprite = sprites[rows[currentTry].cols[currentCol].die.value];
            UpdateCurrentTryRowSum();
        }
    }

    public void UnlockCurrentTryRow()
    {
        for (int i = 0; i < rows[currentRow].cols.Count; i++)
        {
            int col = i;
            int row = currentTry;
            rows[currentTry].gameObject.SetActive(true);
            rows[currentTry].cols[i].die.ShowDie();
            rows[currentTry].cols[i].die.SetOnClick(() => {
                SetSelectedDie(row, col);
            });
        }
    }

    public void HighlightCurrentDie()
    {
        if (animating)
            return;

        if (currentRow == currentTry)
            rows[currentRow].cols[currentCol].die.SetSelected(true);
    }

    public void UpdateCurrentTryRowSum()
    {
        int sum = 0;
        for (int i = 0; i < rows[currentTry].cols.Count; i++)
        {
            sum += rows[currentRow].cols[i].die.value + 1;
        }
        rows[currentTry].sumLabel.gameObject.SetActive(true);
        rows[currentTry].sumLabel.text = sum.ToString();
    }

    public void UpdateScoreRow()
    {
        points.gameObject.SetActive(true);
        points.sumLabel.gameObject.SetActive(true);
        points.sumLabel.text = score.ToString();
    }

    public void ColapseAllButtons()
    {
        if (!openRows)
            return;

        openRows = false;
        closedRows = true;

        if (currentOpenRow < 0)
        {
            foreach (var row in rows)
            {
                foreach (var col in row.cols)
                {
                    col.up.gameObject.SetActive(false);
                    col.down.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            foreach (var col in rows[currentOpenRow].cols)
            {
                col.up.gameObject.SetActive(false);
                col.down.gameObject.SetActive(false);
            }
            currentOpenRow = -1;
        }
    }

    public void OpenAllButtons()
    {
        if (!closedRows)
            return;

        closedRows = false;
        openRows = true;

        for (int i = 0; i < rows.Count; i++) // open all rows
        {
            if (i != currentOpenRow) // except the one already open
            foreach (var col in rows[i].cols)
            {
                col.up.gameObject.SetActive(true);
                col.down.gameObject.SetActive(true);
            }
        }

        currentOpenRow = -1;
    }

    public void OpenButtonsAtRow(int row)
    {
        if (currentOpenRow == row) // if row is already open, cancel
            return;

        openRows = true;

        if (row < rows.Count) // open target row
        {
            foreach (var col in rows[row].cols)
            {
                col.up.gameObject.SetActive(true);
                col.down.gameObject.SetActive(true);
            }
        }

        if (currentOpenRow < 0) // if no prev row, stop here
        {
            currentOpenRow = row;
            return;
        }

        foreach (var col in rows[currentOpenRow].cols) // close prev row
        {
            col.up.gameObject.SetActive(false);
            col.down.gameObject.SetActive(false);
        }
        currentOpenRow = row;
    }

    public void CloseButtonsAtRow(int row)
    {
        if (row < rows.Count) // close target row
        {
            foreach (var col in rows[row].cols)
            {
                col.up.gameObject.SetActive(false);
                col.down.gameObject.SetActive(false);
            }
        }

        if (currentOpenRow == row) // if closed the current open row
        {
            currentOpenRow = -1;
            return;
        }
    }
}