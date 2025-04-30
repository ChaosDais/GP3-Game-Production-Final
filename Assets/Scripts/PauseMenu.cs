using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // Reference to the pause menu UI panel
    public GameObject pauseMenuUI;
    public GameObject gameUI;

    // Bool to track if the game is paused
    private bool isPaused = false;

    void Start()
    {
        // Make sure the pause menu is hidden on start
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // Ensure the cursor is locked and hidden when the game starts
        SetMouseActive(false);
    }

    void Update()
    {
        // Check for the Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        // Disable the pause menu UI
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            gameUI.SetActive(true);
        }

        // Resume the game by setting timeScale to 1
        Time.timeScale = 1f;

        // Update the pause state
        isPaused = false;

        // Lock and hide the cursor when resuming the game
        SetMouseActive(false);
    }

    private void Pause()
    {
        // Enable the pause menu UI
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            gameUI.SetActive(false);
        }

        // Pause the game by setting timeScale to 0
        Time.timeScale = 0f;

        // Update the pause state
        isPaused = true;

        // Unlock and show the cursor when the game is paused
        SetMouseActive(true);
    }

    // Helper method to set the mouse active or inactive
    private void SetMouseActive(bool isActive)
    {
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isActive;
    }
}
