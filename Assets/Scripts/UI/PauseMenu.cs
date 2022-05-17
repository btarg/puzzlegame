using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    LoadScene loader;

    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    GameObject playerObject;

    public List<GameObject> objects;
    public GameObject cameraHolder;

    public GameObject savePopup;

    public UnityEvent onSaved;
    PlayerControls controls;

    // Make sure the game isn't paused on startup
    void Start()
    {
        // Use new input system
        controls = new PlayerControls();
        controls.Enable();
        controls.Gameplay.PauseMenu.performed += HandlePause;

        loader = gameObject.AddComponent(typeof(LoadScene)) as LoadScene;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        Resume();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameIsPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void HandlePause(InputAction.CallbackContext value)
    {
        if (GameIsPaused)
        {
            Debug.Log("Game Resumed");
            Resume();
        }
        else
        {
            Debug.Log("Game Paused");
            Pause();
        }
    }

    public void Resume()
    {
        // Resume Game
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        // Enable all objects in list
        EnableAll();

        // Re-attach camera to player
        cameraHolder.transform.parent = playerObject.transform;

    }

    public void Pause()
    {
        // Pause Game
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        // Detach camera from player
        cameraHolder.transform.parent = null;

        // Disable all objects in list
        DisableAll();
    }

    public void EnableAll()
    {
        foreach (var obj in objects)
            obj.SetActive(true);
    }

    public void DisableAll()
    {
        foreach (var obj in objects)
            obj.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void MenuButton()
    {
        playerObject.GetComponent<SelectObject>().SavePlayer();
        loader.SceneLoad("MainMenu");
    }

    public void LoadButton()
    {
        loader.LoadFromFile();
    }

    public void SaveButton()
    {
        // Save game
        Resume();
        onSaved.Invoke();
        playerObject.GetComponent<SelectObject>().SavePlayer();
        savePopup.GetComponent<Animator>().SetTrigger("fade");
    }

}