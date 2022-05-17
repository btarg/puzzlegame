using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
			// Kill the player
            other.gameObject.GetComponent<SelectObject>().Kill();
        } else if (other.GetComponent<Respawnable>()) {
			// Respawn important objects (e.g. cubes)
            other.GetComponent<Respawnable>().Respawn();
        } else {
            GameObject effect = GameObject.Find("DeathEffect");
            effect.transform.position = other.transform.position;
            effect.GetComponent<ParticleSystem>().Play();
            Destroy(other.gameObject);
        }
    }
}
