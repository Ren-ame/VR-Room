using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text multiplierText;
    [SerializeField] private TMP_Text amountText;

    public void ShowWin(float multiplier, int amount)
    {
        gameObject.SetActive(true);
        resultText.text = "WIN!";
        resultText.color = Color.yellow;
        multiplierText.text = $"x{multiplier}";
        amountText.text = $"+{amount}";
        amountText.color = Color.green;
    }

    public void ShowLose(float multiplier, int amount)
    {
        gameObject.SetActive(true);
        resultText.text = "LOSE";
        resultText.color = Color.red;
        multiplierText.text = $"x{multiplier}";
        amountText.text = $"-{amount}";
        amountText.color = Color.red;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}