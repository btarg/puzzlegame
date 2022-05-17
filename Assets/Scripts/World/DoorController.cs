using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorController : MonoBehaviour
{
    Animator animator;
    public bool startOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        if (startOpen) {
            OpenDoor();
        }
    }

    public void OpenDoor() {
        animator.SetBool("isOpen", true);
        Debug.Log("open");
    }
    public void CloseDoor() {
        animator.SetBool("isOpen", false);
        Debug.Log("close");
    }
}
