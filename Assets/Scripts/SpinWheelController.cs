using UnityEngine;

public class SpinWheelController : MonoBehaviour
{
    [SerializeField] private Rigidbody wheelRigidbody;
    [SerializeField] private float stopThreshold = 0.1f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip resultClip;
    [SerializeField] private float pointerAngle = 90f; // 12시 방향 각도 (조절 가능)
    [SerializeField] private float stillDuration = 0.5f; // 0.5초 이상 멈춰야 판정
    [SerializeField] private BettingManager bettingManager;
    [SerializeField] private ResultUI resultUI;
    [SerializeField] private Transform pointerArrow;


    private bool betPlaced = false; // 베팅 중복 방지
    private float stillTime = 0f;
    private bool isSpinning = false;

    void Update()
    {
        float speed = wheelRigidbody.angularVelocity.magnitude;

        if (speed > stopThreshold)
        {
            if (!isSpinning && !betPlaced)
            {
                if (!bettingManager.PlaceBet())
                {
                    Debug.Log("잔고 부족!");
                    return;
                }
                betPlaced = true;
                resultUI.Hide(); // 돌리기 시작할 때 숨기기
            }
            isSpinning = true;
            stillTime = 0f;
        }

        if (isSpinning && speed < stopThreshold)
        {
            stillTime += Time.deltaTime;

            if (stillTime >= stillDuration)
            {
                isSpinning = false;
                stillTime = 0f;
                betPlaced = false; // 다음 베팅 허용
                JudgeResult();
            }
        }
    }

    private void JudgeResult()
    {
        wheelRigidbody.angularVelocity = Vector3.zero;

        WheelSectorBuilder builder = GetComponent<WheelSectorBuilder>();

        Vector3 localPointerDir = wheelRigidbody.transform
            .InverseTransformPoint(pointerArrow.position).normalized;

        int closestIndex = 0;
        float maxDot = float.MinValue;

        for (int i = 0; i < builder.sectorCenterDirections.Count; i++)
        {
            float dot = Vector3.Dot(localPointerDir, builder.sectorCenterDirections[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                closestIndex = i;
            }
        }

        SectorData result = builder.builtSectors[closestIndex];

        if (result.multiplier == bettingManager.selectedMultiplier)
        {
            int winAmount = Mathf.RoundToInt(bettingManager.BetAmount * result.multiplier);
            bettingManager.Win(result.multiplier);
            resultUI.ShowWin(result.multiplier, winAmount);
        }
        else
        {
            resultUI.ShowLose(result.multiplier, bettingManager.BetAmount);
        }

        if (audioSource != null && resultClip != null)
            audioSource.PlayOneShot(resultClip);
    }
}