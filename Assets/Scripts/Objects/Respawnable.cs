using UnityEngine;

public class Respawnable : MonoBehaviour
{
    Vector3 originalPosition;
    Quaternion originalRotation;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

    }

    public void Respawn() {
        gameObject.transform.SetPositionAndRotation(originalPosition, originalRotation);
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero; // cancel velocity
        gameObject.GetComponent<TimeBody>().DeleteHistory(); // remove rewind history
    }
}
