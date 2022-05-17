using UnityEngine;

public class FaceUp : MonoBehaviour
{ 
    void Update()
    {
		// Keep facing upwards
        gameObject.transform.rotation = Quaternion.identity;
    }
}
