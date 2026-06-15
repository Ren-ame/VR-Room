using UnityEngine;
using UnityEngine.Events;

public class BettingManager : MonoBehaviour
{
    [SerializeField] private PlayerWallet wallet;
    [SerializeField] private int betAmount = 100;
    [SerializeField] private int[] betSteps = { 100, 500 }; // 증감 단위 배열
    [SerializeField] private int minBet = 100;
    [SerializeField] private int maxBet = 1000;

    public int BetAmount => betAmount;
    public float selectedMultiplier = 2f;
    public UnityEvent<float> onMultiplierChanged;
    public UnityEvent<int> onBetChanged; // UI 업데이트용

    public void IncreaseBet(int stepIndex)
    {
        int step = betSteps[stepIndex];
        Debug.Log($"step: {step}, betAmount: {betAmount}, Balance: {wallet.Balance}");

        if (betAmount + step > wallet.Balance)
        {
            Debug.Log("잔고 초과로 막힘!");
            return;
        }
        betAmount = Mathf.Min(betAmount + step, maxBet);
        Debug.Log($"새 betAmount: {betAmount}");
        onBetChanged.Invoke(betAmount);
    }

    public void DecreaseBet(int stepIndex)
    {
        int step = betSteps[stepIndex];
        betAmount = Mathf.Max(betAmount - step, minBet);
        onBetChanged.Invoke(betAmount);
    }

    public bool PlaceBet()
    {
        return wallet.Spend(betAmount); // 잔고에서 베팅금 차감
    }

    public void Payout(float multiplier)
    {
        int winAmount = Mathf.RoundToInt(betAmount * multiplier);
        wallet.Earn(winAmount);
    }

    public void SelectMultiplier(float multiplier)
    {
        selectedMultiplier = multiplier;
        onMultiplierChanged.Invoke(multiplier);
        Debug.Log($"선택 배율 : x{multiplier}");
    }

    public void Win(float multiplier)
    {
        int winAmount = Mathf.RoundToInt(betAmount * multiplier);
        wallet.Earn(winAmount);
    }
}