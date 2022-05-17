using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SelectObject : MonoBehaviour
{
    [Header("- Selecting objects -")]

    // Range is for the main raycast
    public float range = 100f;
    public Camera fpsCam;
    public Shader selectedObjectShader;
    public GameObject selectedObject;
    TimeBody selectedTimebody;
    MeshRenderer rend;
    Shader normalShader;
    ParticleSystem particle;

    [Header("- Interacting with objects -")]

    public PickupObject pickupScript;
    Rigidbody pickupRigidbody;
    TimeBody pickupTimebody;
    TimeBody interactTimebody;
    bool couldBeCarried = true;
    bool couldBeSelected = true;

    GameObject interactObject;

    public float pickupRange = 5f;

    public GameObject bluePulse;
    public GameObject orangePulse;

    public List<GameObject> frozenObjects = new List<GameObject>();


    [Header("- UI -")]

    public GameObject crosshairObject;
    public bool hasSelected = false;
    Crosshair crosshair;

    public GameObject blackFade;


    // Use this for initialization
    void Start()
    {
        SetCrosshair(true);
        blackFade.SetActive(true);
        crosshair = crosshairObject.GetComponent<Crosshair>();
        crosshair.selectObject = this;
    }

    public void Kill()
    {
        gameObject.GetComponent<SimpleCharacterController>().enabled = false;
        SetCrosshair(false);
        StartCoroutine(PlayFadeAnimation(false));
    }
    public void CompleteLevel()
    {
        gameObject.GetComponent<SimpleCharacterController>().enabled = false;
        SetCrosshair(false);
        StartCoroutine(PlayFadeAnimation(true));
    }


    IEnumerator PlayFadeAnimation(bool nextLevel)
    {

        blackFade.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitUntil(() => blackFade.GetComponent<Image>().color.a == 1); // wait until fully faded out

        if (nextLevel)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // next scene
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload scene
        }

    }


    // Update is called once per frame
    void Update()
    {
        // Kill player below Y level -10
        if (gameObject.transform.position.y < -10f && gameObject.GetComponent<SimpleCharacterController>().enabled)
        {
            Kill();
            return;
        }

    }

    public void OnInteract(InputAction.CallbackContext value) {
        InteractBehaviour();
    }
    public void OnSelectObject(InputAction.CallbackContext value) {
        // Shoot a raycast if not carrying anything 
        if (pickupScript.isCarryingObject) {
            return;
        }
        Shoot();
    }

    // Shoot a raycast from the player's point of view, and determine what to do with the hit object.
    void Shoot()
    {
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, range))
        {
            // Deselect previous objects
            if (hasSelected)
            {
                rend = selectedObject.GetComponentInChildren<MeshRenderer>();

                // Set the previously selected object to its normal material
                rend.material.shader = normalShader;

                hasSelected = false;
            }

            // Get new selected object
            selectedObject = hit.transform.gameObject;
            selectedTimebody = selectedObject.GetComponent<TimeBody>();
            rend = selectedObject.GetComponentInChildren<MeshRenderer>();

            if (selectedTimebody == null)
            {
                selectedObject = null;
                return;
            }

            normalShader = selectedTimebody.normalShader;

            if (selectedTimebody.canBeSelected)
            {
                // Draw outline around object
                HighlightBlue(rend);

                // Play particle effect
                GameObject pulse = Instantiate(bluePulse, selectedObject.transform.position, Quaternion.identity);
                pulse.transform.SetParent(selectedObject.transform);
                particle = pulse.GetComponent<ParticleSystem>();

                particle.Play();

                hasSelected = true;
            }
            else
            {
                hasSelected = false;
            }

        }

    }

    // When the player presses the Interact button. Called via update.
    void InteractBehaviour()
    {
        // The player is holding an object
        if (pickupScript.isCarryingObject)
        {
            pickupScript.StopCarrying();
            return;
        }

        // Find object with raycast and pick it up
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, pickupRange))
        {
            interactObject = hit.transform.gameObject;

            // If the object has a Timebody component and the player is not holding an object, pick up the object
            if (hit.rigidbody && !pickupScript.isCarryingObject && !hit.rigidbody.isKinematic && interactObject.GetComponent<TimeBody>())
            {
                pickupScript.StartCarrying(interactObject);
            }

            // The player is interacting with an interactive object
            else if (interactObject.GetComponent<InteractiveObject>())
            {
                InteractiveObject interactButton = interactObject.GetComponent<InteractiveObject>();

                interactButton.PlayerInteract();
            }
            else
            {
                FailedInteraction();
            }
        }
        else
        {
            FailedInteraction();
        }

    }

    void FailedInteraction()
    {
        // Cannot interact with object
        // TODO: Play a sound or something when interaction fails (like in hl2)
        Debug.Log("Nope");
    }

    public void OnFreeze(InputAction.CallbackContext value) {
        TryFreezingObject();
    }
    public void OnUnfreeze(InputAction.CallbackContext value) {
        UnfreezeObjects();
    }

    public void TryFreezingObject()
    {
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, range))
        {
            interactObject = hit.transform.gameObject;
            interactTimebody = interactObject.GetComponent<TimeBody>();

            if (interactObject == selectedObject)
            {
                // Deselect this object
                if (hasSelected)
                {
                    hasSelected = false;
                }

            }

            if (interactTimebody)
            {
                if (!interactTimebody.isFrozen)
                {
                    FreezeObject(interactObject);
                }
                else
                {
                    Unfreeze(interactObject);
                }

                interactTimebody.StopRewind();
            }
        }
    }

    public void SetCrosshair(bool active)
    {
        if (crosshairObject)
        {
            crosshairObject.SetActive(active);
        }
    }


    public void FreezeObject(GameObject toFreeze)
    {
        pickupTimebody = toFreeze.GetComponent<TimeBody>();

        // If a rigidbody and timebody are found
        if (pickupTimebody.canBeSelected)
        {
            // Make the object kinematic, and make the player unable to select it
            if (pickupTimebody.isBeingCarried)
            {
                pickupScript.StopCarrying();
            }

            // Draw outline around object
            rend = toFreeze.GetComponentInChildren<MeshRenderer>();

            HighlightOrange(rend);

            // Play selection particle effect
            GameObject pulse = Instantiate(orangePulse, toFreeze.transform.position, Quaternion.identity);
            pulse.transform.SetParent(toFreeze.transform);
            particle = pulse.GetComponent<ParticleSystem>();
            particle.Play();

            Debug.Log("Frozen object: " + toFreeze);

        }

        // Add selected object to the list of frozen objects if it hasn't already been added
        if (!frozenObjects.Contains(toFreeze))
        {
            frozenObjects.Add(toFreeze);
        }

        pickupTimebody.isFrozen = true;
        couldBeSelected = pickupTimebody.canBeSelected;
        couldBeCarried = pickupTimebody.canBeCarried;
        pickupTimebody.canBeSelected = false;

    }

    public void HighlightOrange(MeshRenderer rend)
    {
        // Make it orange!
        rend.material.shader = selectedObjectShader;
        Color redColor = new Color(255, 150, 0, 0);
        rend.material.SetColor("_OutlineColor", redColor);
    }

    public void HighlightBlue(MeshRenderer rend)
    {
        // Make it blue!
        rend.material.shader = selectedObjectShader;
        Color blueColor = new Color(0, 210, 255, 0);
        rend.material.SetColor("_OutlineColor", blueColor);
    }

    public void UnfreezeObjects()
    {
        if (frozenObjects.Count > 0)
        {
            // For all frozen objects
            foreach (GameObject frozen in frozenObjects.ToArray())
            {
                Unfreeze(frozen);
            }

            // Clear the list of frozen objects
            frozenObjects.Clear();
        }

    }

    void Unfreeze(GameObject frozen)
    {
        TimeBody frozenTimebody = frozen.GetComponent<TimeBody>();

        if (frozenTimebody)
        {
            // Set the previously selected object to its normal shader
            rend = frozen.transform.GetComponentInChildren<MeshRenderer>();
            normalShader = frozenTimebody.normalShader;

            rend.material.shader = normalShader;

            // Play selection particle effect
            GameObject pulse = Instantiate(orangePulse, frozen.transform.position, Quaternion.identity);
            pulse.transform.SetParent(frozen.transform);
            particle = pulse.GetComponent<ParticleSystem>();
            particle.Play();

            frozenObjects.Remove(frozen);
            frozenTimebody.isFrozen = false;
            frozenTimebody.canBeSelected = couldBeSelected;
            frozenTimebody.canBeCarried = couldBeCarried;
        }

        Debug.Log("Unfroze object " + frozen);
    }

    public void SavePlayer()
    {
        PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);
        Debug.Log("Checkpoint Saved: " + PlayerPrefs.GetString("SavedScene", "SampleScene"));
    }

}
