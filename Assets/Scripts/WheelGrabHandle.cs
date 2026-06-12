using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WheelGrabHandle : MonoBehaviour
{
    [SerializeField] private Rigidbody wheelRigidbody;
    [SerializeField] private float torqueMultiplier = 50f;

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

        // Y 회전만 남기고 나머지 진동 제거
        Vector3 av = wheelRigidbody.angularVelocity;
        Debug.Log($"놓을 때 angularVelocity: {av}");
        wheelRigidbody.angularVelocity = new Vector3(0, 0, av.z);
        Debug.Log("놓음!");
    }

    void FixedUpdate()
    {
        if (!isGrabbed || handTransform == null) return;

        Vector3 currentPos = handTransform.position;
        Vector3 delta = currentPos - lastHandPosition;

        if (delta.magnitude < 0.0001f)
        {
            lastHandPosition = currentPos;
            return;
        }

        Vector3 handleDir = (transform.position - wheelRigidbody.transform.position).normalized;

        Vector3 fullTorque = Vector3.Cross(handleDir, delta) * torqueMultiplier;

        // Z축 토크만 적용
        wheelRigidbody.AddTorque(new Vector3(0, 0, fullTorque.z));

        lastHandPosition = currentPos;
    }
}