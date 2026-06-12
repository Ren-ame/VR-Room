using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Rigidbody doorRigidbody;
    [SerializeField] private float torqueMultiplier = 2000f; // 200 → 2000

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
        Debug.Log("문 손잡이 잡음!");
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        handTransform = null;
        doorRigidbody.angularVelocity = Vector3.zero;
    }

   void FixedUpdate()
   {
        if (!isGrabbed || handTransform == null) return;

        Vector3 currentPos = handTransform.position;
        Vector3 delta = currentPos - lastHandPosition;

        Debug.Log($"delta: {delta}");

        if (delta.magnitude < 0.0001f)
        {
            lastHandPosition = currentPos;
            return;
        }

        Vector3 handleDir = (transform.position - doorRigidbody.transform.position).normalized;
        float torqueY = (handleDir.x * delta.z - handleDir.z * delta.x) * torqueMultiplier;

        doorRigidbody.AddTorque(new Vector3(0, torqueY, 0));
        lastHandPosition = currentPos;
   }
}