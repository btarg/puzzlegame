using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void SceneLoad(string sceneToLoad)
    {
        Debug.Log("Loading Scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoadFromFile()
    {
        string sceneName = PlayerPrefs.GetString("SavedScene", "SampleScene");

        Debug.Log("Loading Saved Scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

}

