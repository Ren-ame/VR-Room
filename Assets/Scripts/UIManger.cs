using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text betText;

    [SerializeField] private Button[] multiplierBtns;
    private Color[] originalColors;

    void Start()
    {
        originalColors = new Color[multiplierBtns.Length];
        for (int i = 0; i < multiplierBtns.Length; i++)
        {
            originalColors[i] = multiplierBtns[i].image.color;
            Debug.Log($"버튼 {i} 색상: {originalColors[i]}");
        }
    }

    public void UpdateMoney(int amount)
    {
        moneyText.text = $"Money : {amount}";
    }

    public void UpdateBet(int amount)
    {
        betText.text = $"Bet : {amount}";
    }

    public void UpdateSelectedMultiplier(float multiplier)
    {
        float[] multipliers = { 2f, 3f, 5f, 10f };

        for (int i = 0; i < multiplierBtns.Length; i++)
        {
            multiplierBtns[i].image.color =
                multipliers[i] == multiplier ? originalColors[i] : Color.gray;
        }
    }
}