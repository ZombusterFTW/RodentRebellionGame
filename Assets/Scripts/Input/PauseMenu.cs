using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Canvas canvas;
    private bool pauseMenuActive = false;
    [SerializeField] SceneTransitionerManager sceneTransitionerManager;
    [SerializeField] PlayerController playerController;
    [SerializeField] Health health;
    public void GoToLastCheckpoint()
    {
        canvas.enabled = false;
        Time.timeScale = 1.0f;
        //Debug.Log("Un-Paused Game");
        if(!GameObject.ReferenceEquals(playerController.movingPlatform, null)) playerController.UnlinkPlatform(playerController.movingPlatform);
        playerController.BigJoeRespawn(true);
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
            canvas.enabled = false;
            Time.timeScale = 1.0f;
            Debug.Log("Un-Paused Game");
            pauseMenuActive = false;
            health.isInvincible = false;
            playerController.DisableControls(false);
        }
    }





    void Start()
    {
        canvas.enabled = false;
        sceneTransitionerManager = SceneTransitionerManager.instance;
    }

    
}
