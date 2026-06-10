using UnityEngine;

public class SpinWheelController : MonoBehaviour
{
    [SerializeField] private Rigidbody wheelRigidbody;
    [SerializeField] private float stopThreshold = 0.1f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip resultClip;
    [SerializeField] private float pointerAngle = 90f; // 12시 방향 각도 (조절 가능)

    private bool isSpinning = false;

    void Update()
    {
        if (isSpinning && wheelRigidbody.angularVelocity.magnitude < stopThreshold)
        {
            isSpinning = false;
            JudgeResult();
        }

        if (wheelRigidbody.angularVelocity.magnitude > stopThreshold)
            isSpinning = true;
    }

    private void JudgeResult()
    {
        WheelSectorBuilder builder = GetComponent<WheelSectorBuilder>();

        // 현재 바퀴 Y 회전값
        float currentAngle = (wheelRigidbody.transform.eulerAngles.y % 360f + 360f) % 360f;

        // 12시 방향 기준으로 역산
        float targetAngle = (pointerAngle - currentAngle + 360f) % 360f;

        int totalSectors = builder.builtSectors.Count;
        float sectorSize = 360f / totalSectors;
        int sectorIndex = Mathf.FloorToInt(targetAngle / sectorSize) % totalSectors;

        SectorData result = builder.builtSectors[sectorIndex];
        Debug.Log($"당첨 : x{result.multiplier}");

        if (audioSource != null && resultClip != null)
            audioSource.PlayOneShot(resultClip);
    }
}