using UnityEngine;
using UnityEngine.InputSystem;

public class PickupObject : MonoBehaviour
{
    public SelectObject selectObject;
    public GameObject tempParent;
    public Camera fpsCam;
    public float distanceFromCamera = 2f;
    public float breakThreshold = 0.5f;
    public bool isCarryingObject = false;
    public float throwForce = 2000f;
    public bool disableCrosshair = true;
    Rigidbody pickupRigidbody;
    float distance;
    GameObject item;
    Quaternion originalRotation;

    PlayerControls controls;

    void Start() {
        controls = new PlayerControls();
        controls.Enable();
        controls.Gameplay.Select.performed += ThrowObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (isCarryingObject)
        {

            if (distance <= breakThreshold)
            {
                pickupRigidbody.useGravity = false;
                pickupRigidbody.detectCollisions = true;
            }
            else
            {
                StopCarrying();
                return;
            }

            UpdateParentPosition();

            distance = Vector3.Distance(item.transform.position, tempParent.transform.position);

            // Remove velocity
            pickupRigidbody.velocity = Vector3.zero;
            pickupRigidbody.angularVelocity = Vector3.zero;

            // Keep original rotation
            item.transform.rotation = originalRotation;

            item.transform.SetParent(tempParent.transform);
        }

    }

    void UpdateParentPosition()
    {
        Vector3 inFrontOfCamera = fpsCam.transform.position + fpsCam.transform.forward * distanceFromCamera;

        // Set hand position to be in front of the camera
        tempParent.transform.position = inFrontOfCamera;
        item.transform.position = Vector3.Lerp(item.transform.position, tempParent.transform.position, Time.deltaTime * 5f);

    }


    public void StartCarrying(GameObject go)
    {
        TimeBody pickupTimebody = go.GetComponent<TimeBody>();

        if (!pickupTimebody.canCurrentlyBeCarried || isCarryingObject)
            return;

        item = go;
        pickupRigidbody = item.GetComponent<Rigidbody>();
        originalRotation = item.transform.rotation;

        UpdateParentPosition();
        item.transform.position = tempParent.transform.position;

        isCarryingObject = true;
        pickupTimebody.isBeingCarried = true;

        if (disableCrosshair)
        {
            selectObject.SetCrosshair(false);
        }
    }

    public void StopCarrying()
    {
        TimeBody pickupTimebody = item.GetComponent<TimeBody>();
        if (isCarryingObject)
        {
            item.transform.SetParent(null);
            pickupRigidbody.useGravity = true;

            isCarryingObject = false;

            pickupTimebody.isBeingCarried = false;

        }

        for (int i = 0; i < tempParent.transform.childCount; i++)
        {
            tempParent.transform.GetChild(i).SetParent(null);
        }

        distance = 0f;
        isCarryingObject = false;

        if (disableCrosshair)
        {
            selectObject.SetCrosshair(true);
        }

    }

    public void ThrowObject(InputAction.CallbackContext value)
    {
        if (!isCarryingObject) {
            return;
        }

        // Put the object down and apply force ("throws" the object using throwForce as a multiplier)
        StopCarrying();
        pickupRigidbody.AddForce(fpsCam.transform.forward * throwForce);
    }

}
