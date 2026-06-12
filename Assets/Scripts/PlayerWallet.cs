using UnityEngine;
using UnityEngine.Events;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private int startingBalance = 1000;

    public int Balance { get; private set; }

    public UnityEvent<int> onBalanceChanged; // UI ¾÷µ¥ÀÌÆ®¿ë

    void Awake()
    {
        Balance = startingBalance;
    }

    public bool Spend(int amount)
    {
        if (Balance < amount)
        {
            Debug.Log("ÀÜ°í ºÎÁ·!");
            return false;
        }
        Balance -= amount;
        onBalanceChanged.Invoke(Balance);
        return true;
    }

    public void Earn(int amount)
    {
        Balance += amount;
        onBalanceChanged.Invoke(Balance);
        Debug.Log($"È¹µæ! +{amount} / ÀÜ°í: {Balance}");
    }
}