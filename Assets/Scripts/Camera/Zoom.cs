using UnityEngine;
using UnityEngine.InputSystem;

public class Zoom : MonoBehaviour
{
    public float zoom = 15f;
    float normal = 60f;
    float normalSens = 1.0f;
    public float sensitivityMultiplier = 0.5f;
    public int smooth = 10;
    bool isZoomed = false;
    public Camera cam;
    public SimpleCharacterController player;

    // Use this for initialization
    void Start()
    {
        // Get initial ("normal") FOV and sensitivity
        normal = cam.fieldOfView;
        normalSens = player.m_LookSensitivity;
    }

    public void OnZoom(InputAction.CallbackContext value) {
        isZoomed = value.ReadValueAsButton();
    }

    // Update is called once per frame
    void Update()
    {
        if (isZoomed)
        {
            // Zoom in the camera (Gradually increases FOV)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom, Time.deltaTime * smooth);
            player.m_LookSensitivity = normalSens * sensitivityMultiplier;
        }
        else
        {
            // Revert FOV (Zoom out)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normal, Time.deltaTime * smooth);
            player.m_LookSensitivity = normalSens;
        }
    }
}
