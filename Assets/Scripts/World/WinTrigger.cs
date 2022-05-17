using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
			// The player wins the game
            other.gameObject.GetComponent<SelectObject>().CompleteLevel();
        } else if (other.GetComponent<Respawnable>()) {
			// Destroy object and create particles
            Destroy(other);
        }
    }
}
