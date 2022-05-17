using UnityEngine;

public class PlatformAttach : MonoBehaviour
{
    Transform tempParent;
    private void OnTriggerEnter(Collider other) {
        tempParent = other.transform.parent;
        other.transform.parent = transform;
    }
    private void OnTriggerExit(Collider other) {
        other.transform.parent = tempParent;
        tempParent = null;
    }
}
