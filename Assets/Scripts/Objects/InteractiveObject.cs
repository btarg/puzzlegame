using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour
{
    public UnityEvent onPlayerInteract;

    public void PlayerInteract()
    {
        onPlayerInteract.Invoke();
    }
}
