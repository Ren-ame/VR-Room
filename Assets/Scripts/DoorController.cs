using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Transform doorTransform;
    [SerializeField] private float rotateSpeed = 300f;
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 90f;

    private bool isGrabbed = false;
    private Transform handTransform;
    private Vector3 lastHandPosition;
    private float currentAngle = 0f;
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
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        handTransform = null;
    }

    void Update()
    {
        if (!isGrabbed || handTransform == null) return;

        Vector3 currentPos = handTransform.position;
        Vector3 delta = currentPos - lastHandPosition;

        float angle = delta.x * rotateSpeed;
        currentAngle = Mathf.Clamp(currentAngle + angle, minAngle, maxAngle);

        doorTransform.localRotation = Quaternion.Euler(0, currentAngle, 0);
        lastHandPosition = currentPos;
    }
}