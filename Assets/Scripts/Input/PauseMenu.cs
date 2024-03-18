using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Canvas canvas;
    private bool pauseMenuActive = false;
    [SerializeField] SceneTransitionerManager sceneTransitionerManager;
    [SerializeField] PlayerController playerController;
    [SerializeField] Health health;
    [SerializeField] Button returnToLastCheckpointButton;
    public void GoToLastCheckpoint()
    {
        if (!DialogueManager.GetInstance().dialogueIsPlaying)
        {
            canvas.enabled = false;
            Time.timeScale = 1.0f;
            //Debug.Log("Un-Paused Game");
            if (!GameObject.ReferenceEquals(playerController.movingPlatform, null)) playerController.UnlinkPlatform(playerController.movingPlatform);
            playerController.BigJoeRespawn(true);
        }
    }

    public void RestartCurrentLevel()
    {
        //Reload the current scene. This will properly reset the timer and take the player to the correct point
        if (!GameObject.ReferenceEquals(playerController.movingPlatform, null)) playerController.UnlinkPlatform(playerController.movingPlatform);
        canvas.enabled = false;
        Time.timeScale = 1.0f;
        Debug.Log("Un-Paused Game");
        pauseMenuActive = false;
        health.isInvincible = false;
        playerController.DisableControls(false);
        sceneTransitionerManager.StartTransition(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        sceneTransitionerManager.StartTransition();
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    public void ToggleMenu()
    {
        if (!pauseMenuActive) 
        {
            canvas.enabled = true;
            Time.timeScale = 0.0f;
            Debug.Log("Paused Game");
            pauseMenuActive = true;
            health.isInvincible = true;
            playerController.DisableControls(true);
        }
        else
        {

            if(DialogueManager.GetInstance().dialogueIsPlaying)
            {
                canvas.enabled = false;
                pauseMenuActive = false;
                health.isInvincible = false;
            }
            else
            {
                canvas.enabled = false;
                Time.timeScale = 1.0f;
                Debug.Log("Un-Paused Game");
                pauseMenuActive = false;
                health.isInvincible = false;
                playerController.DisableControls(false);
            }
        }
    }



    private void Update()
    {
        if(DialogueManager.GetInstance().dialogueIsPlaying)
        {
            returnToLastCheckpointButton.interactable = false;
        }
        else
        {
            returnToLastCheckpointButton.interactable = true;
        }
    }

    void Start()
    {
        canvas.enabled = false;
        sceneTransitionerManager = SceneTransitionerManager.instance;
    }

    
}
