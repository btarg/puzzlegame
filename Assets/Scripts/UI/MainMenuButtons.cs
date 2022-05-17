using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    LoadScene loader = new LoadScene();

    public void SceneLoad(string sceneToLoad)
    {
        loader.SceneLoad(sceneToLoad);
    }

    public void LoadFromFile()
    {
        loader.LoadFromFile();
    }

    public void QuitButton()
    {
        Application.Quit();
    }

}
