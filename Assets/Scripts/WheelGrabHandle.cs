using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WheelGrabHandle : MonoBehaviour
{
    [SerializeField] private Rigidbody wheelRigidbody;
    [SerializeField] private float torqueMultiplier = 500f;

    private bool isGrabbed = false;
    private Transform handTransform;
    private Vector3 lastHandPosition;

    private XRBaseInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        handTransform = args.interactorObject.transform;
        lastHandPosition = handTransform.position;
        Debug.Log("잡음!");
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        handTransform = null;
        Debug.Log("놓음!");
    }

    void FixedUpdate()
    {
        if (!isGrabbed || handTransform == null) return;

        Vector3 currentPos = handTransform.position;
        Vector3 delta = currentPos - lastHandPosition;

        Vector3 torque = Vector3.Cross(
            (transform.position - wheelRigidbody.transform.position).normalized,
            delta
        ) * torqueMultiplier;

        Debug.Log($"delta: {delta}, torque: {torque}"); // 추가

        wheelRigidbody.AddTorque(torque);
        lastHandPosition = currentPos;
    }
}