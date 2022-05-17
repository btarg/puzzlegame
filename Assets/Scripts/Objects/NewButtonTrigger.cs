using UnityEngine;
using UnityEngine.Events;
public class NewButtonTrigger : MonoBehaviour
{
    public bool isBeingPressed = false;
    GameObject playerObject;
    Animator anim;
    public AudioSource positiveSound;
    public AudioSource negativeSound;

    public MeshRenderer buttonRend;
    public Material positiveTexture;
    public Material negativeTexture;
    public Light buttonLight;

    public UnityEvent OnButtonEnter;
    public UnityEvent OnButtonExit;

    int objectsInTrigger = 0;

    void OnTriggerEnter(Collider other)
    {
        objectsInTrigger++;
        ButtonTriggerEntered(other);

        Debug.Log(objectsInTrigger);
    }

    void OnTriggerExit(Collider other)
    {
        objectsInTrigger--;

        if (objectsInTrigger < 1) {
            ButtonTriggerExit();
        }

        Debug.Log(objectsInTrigger);
        
    }


    // Use this for initialization
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        buttonRend.material = negativeTexture;
        buttonLight.color = Color.red;
    }


    public void ButtonTriggerEntered(Collider other)
    {
        // Check if the object colliding with the button is the player or a TimeBody
        if (other.gameObject.GetComponent<TimeBody>() || other.gameObject == playerObject && !isBeingPressed)
        {
            // Invoke event and play animation
            isBeingPressed = true;

            buttonRend.material = positiveTexture;
            buttonLight.color = Color.green;

            positiveSound.Play();
            OnButtonEnter.Invoke();
        }

    }

    public void ButtonTriggerExit()
    {
        isBeingPressed = false;
        negativeSound.Play();

        buttonRend.material = negativeTexture;
        buttonLight.color = Color.red;

        OnButtonExit.Invoke();
    }

}

