using UnityEngine;
using UnityEngine.UI;

public class ShieldDisplay : MonoBehaviour
{
    public Image shieldFill;

    public void UpdateDisplay(float percentage)
    {
        if (shieldFill != null)
        {
            shieldFill.fillAmount = percentage / 100f;
        }
    }
}