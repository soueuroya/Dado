using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasScalerCalculator : MonoBehaviour
{
    // Component references
    [SerializeField] CanvasScaler canvasScaler;

    // Private properties
    private float averageRatio = 16.0f / 9.0f;

    #region Initialization
    private void OnValidate()
    {
        if (canvasScaler == null)
        {
            canvasScaler = GetComponent<CanvasScaler>();
        }
    }
    #endregion Initialization

    #region Private Helpers
    private void CalculateScreenScale()
    {
        if (Camera.main != null && canvasScaler != null)
            if (Camera.main.aspect >= averageRatio)
            {
                canvasScaler.matchWidthOrHeight = 1;
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 0.5f;
            }
    }

    private void OnEnable()
    {
        // This runs only once
        //CalculateScreenScale();

        // This runs every x seconds
        InvokeRepeating("CalculateScreenScale", 0, 1);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
    #endregion Private Helpers
}
