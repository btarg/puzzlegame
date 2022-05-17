using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]

public class TimeBody : MonoBehaviour
{

    [Header("- Recording and Rewinding -")]
    public float recordTime = 10f;
    public float recordVelocityThreshold = 0.01f;
    Camera fpsCam;
    GameObject player;
    public bool canRewind = true;
    public bool canBeSelected = true;
    public bool isRewinding = false;
    PointInTime currentPointInTime;
    float speed;
    Vector3 oldPosition;

    SelectObject selectObject;
    GameObject selectedObject;

    [Header("- Physics -")]
    public bool canBeCarried = true;
    public bool isBeingCarried = false;

    public bool isFrozen = false;

    [Header("- Effects -")]

    
    public Material normalMaterial;
    public Shader normalShader;

    List<PointInTime> pointsInTime = new List<PointInTime>();

    Rigidbody rb;

    public bool canCurrentlyBeCarried;
    public bool canCurrentlyBeSelected;
    PlayerControls controls;

    public void DeleteHistory() {
        pointsInTime.Clear();
    }

    // Use this for initialization
    void Start()
    {
        // Use the new input system
        controls = new PlayerControls();
        controls.Enable();
        controls.Gameplay.Rewind.started += OnButtonHeld;
        controls.Gameplay.Rewind.canceled += OnButtonReleased;

        rb = GetComponent<Rigidbody>();
        GetComponent<Collider>().isTrigger = false; // Time bodies cannot be triggers!
        player = GameObject.FindGameObjectWithTag("Player");

        canCurrentlyBeCarried = canBeCarried;
        canCurrentlyBeSelected = canBeSelected;

        normalMaterial = gameObject.GetComponentInChildren<MeshRenderer>().material;
        normalShader = normalMaterial.shader;
    }

    // Update is called once per frame
    void Update()
    {
        // Get Select object script on Player, if a player object exists
        if (player)
        {
            selectObject = player.GetComponent<SelectObject>();
        }


        // Get currently selected object if it's not null
        if (selectObject.selectedObject != null)
        {
            selectedObject = selectObject.selectedObject;
        }


        if (isFrozen)
        {
            canRewind = false;

            if (rb)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            canCurrentlyBeCarried = false;
            canCurrentlyBeSelected = false;
        }
        else
        {
            canRewind = true;

            if (rb)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            canCurrentlyBeCarried = canBeCarried;
            canCurrentlyBeSelected = canBeSelected;
        }

    }

    void FixedUpdate()
    {

        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }

    }

    void Rewind()
    {
        if (pointsInTime.Count > 0)
        {
            PointInTime pointInTime = pointsInTime[0];
            transform.position = pointInTime.position;
            transform.rotation = pointInTime.rotation;

            pointsInTime.RemoveAt(0);
        }
        // If the list of previous points in time is empty, stop rewinding
        else
        {
            StopRewind();
        }
    }

    void Record()
    {
        // Get object speed without using rigidbody velocity
        speed = Vector3.Distance(oldPosition, transform.position) / Time.deltaTime;
        oldPosition = transform.position;

		// Too many points in time already recorded, so we start deleting some old ones
        if (pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }

        // We only record previous positions if the object is moving at a velocity above the threshold.
        if (speed > recordVelocityThreshold)
        {
            currentPointInTime = new PointInTime(transform.position, transform.rotation);

            // Insert value into list
            pointsInTime.Insert(0, currentPointInTime);
            //Debug.Log("Current position: " + currentPointInTime.position);
        }

    }

    private void OnButtonHeld(InputAction.CallbackContext value) {
        StartRewind();
    }
    private void OnButtonReleased(InputAction.CallbackContext value) {
        StopRewind();
    }

    public void StartRewind()
    {
        if (!canRewind) {
            return;
        }

        // Start rewinding if this object is selected, and if this object is not being carried
        if (gameObject == selectedObject && !isBeingCarried && !isFrozen && selectObject.hasSelected)
        {
			// isRewinding is queried in FixedUpdate
            isRewinding = true;

            if (rb)
                rb.isKinematic = true;

            PostRenderer post = selectObject.GetComponentInChildren<PostRenderer>();
            post.showEffect = true;
            post.showWhenRewinding.SetActive(true);
        }

    }

    public void StopRewind()
    {
        // Stop rewinding
        isRewinding = false;

        if (rb)
            rb.isKinematic = false;

        PostRenderer post = selectObject.GetComponentInChildren<PostRenderer>();
        post.showEffect = false;
        post.showWhenRewinding.SetActive(false);
    }

    private void OnDestroy() {

        controls.Gameplay.Rewind.started -= OnButtonHeld;
        controls.Gameplay.Rewind.canceled -= OnButtonReleased;

        controls.Disable();
        controls.Dispose();
    }

    private void OnDrawGizmos()
    {
		// Draw debug "gizmos" (spheres that only appear in the editor) so we can see an object's position history
        foreach (PointInTime point in pointsInTime)
        {
            Gizmos.color = Color.Lerp(Color.red, Color.green, pointsInTime.IndexOf(point) / Mathf.Round(recordTime / Time.fixedDeltaTime));
            Gizmos.DrawSphere(point.position, 0.1f);
        }

    }

}

