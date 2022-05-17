using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject prefab;

    public void SpawnPrefab() {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
