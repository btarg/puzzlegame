using UnityEngine;
using UnityEngine.Events;
public class PedestalButton : MonoBehaviour
{
    public int buttonPressCooldown = 1;
    public int buttonPressTime = 1;
    public bool isBeingPressed = false;
    Animator anim;
    public AudioSource positiveSound;
    public AudioSource negativeSound;

    public UnityEvent OnButtonPressed;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ButtonDown()
    {
        if (!isBeingPressed)
        {
			// Animate the button and call the event tied to it (e.g. open a door)
            isBeingPressed = true;
            anim.SetTrigger("button");
            OnButtonPressed.Invoke();
        }
    }

}
