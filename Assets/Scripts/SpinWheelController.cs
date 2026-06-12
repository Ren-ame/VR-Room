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

    private bool betPlaced = false; // 베팅 중복 방지
    private float stillTime = 0f;
    private bool isSpinning = false;

    void Update()
    {
        float speed = wheelRigidbody.angularVelocity.magnitude;

        if (speed > stopThreshold)
        {
            // 돌기 시작할 때 베팅 차감
            if (!isSpinning && !betPlaced)
            {
                if (!bettingManager.PlaceBet())
                {
                    Debug.Log("잔고 부족!");
                    return;
                }
                betPlaced = true;
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
        // 즉시 멈추기
        wheelRigidbody.angularVelocity = Vector3.zero;

        WheelSectorBuilder builder = GetComponent<WheelSectorBuilder>();

        float angle = (wheelRigidbody.transform.eulerAngles.z % 360f + 360f) % 360f;
        int totalSectors = builder.builtSectors.Count;
        float sectorSize = 360f / totalSectors;
        int sectorIndex = Mathf.FloorToInt(angle / sectorSize) % totalSectors;

        SectorData result = builder.builtSectors[sectorIndex];

        if (result.multiplier == bettingManager.selectedMultiplier)
        {
            bettingManager.Win(result.multiplier);
            Debug.Log($"당첨! x{result.multiplier}");
        }
        else
        {
            Debug.Log($"꽝! 결과 x{result.multiplier} / 선택 x{bettingManager.selectedMultiplier}");
        }

        if (audioSource != null && resultClip != null)
            audioSource.PlayOneShot(resultClip);
    }
}