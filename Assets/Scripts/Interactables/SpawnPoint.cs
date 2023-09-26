using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class SpawnPoint : MonoBehaviour, R4Activatable
{
    [Tooltip("Wether or not the spawn point will start active.")][SerializeField]private bool isActive = true;
    [Tooltip("Wether or not this spawn is the starting spawn")][SerializeField] private bool isStartingSpawn = false;
    [Tooltip("The color of the spawn point when it is activated")][SerializeField] private Color activatedColor = Color.green;
    [Tooltip("The color of the spawn point when it is not active")][SerializeField] private Color deactivatedColor = Color.white;
    [Tooltip("The color of the spawn point when it is disabled")][SerializeField] private Color disabledColor = Color.red;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private CircleCollider2D circleCollider;    



    private bool isSelected = false;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private SpawnPoint spawnPoint;




    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindObjectOfType<PlayerController>();
        spawnPoint = GetComponent<SpawnPoint>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();

        spriteRenderer.color = deactivatedColor;


        if (isStartingSpawn) isActive = true;


        if (playerController != null && playerController.GetSpawn() == null && isStartingSpawn && isActive) 
        {
            playerController.SetSpawn(spawnPoint);
        }
        if(!isActive) spriteRenderer.color = disabledColor;
    }

    public void UnlinkSpawn()
    {
        isSelected = false;
        if (isActive) spriteRenderer.color = deactivatedColor;
        else spriteRenderer.color = disabledColor;
    }


    public void LinkSpawn()
    {
        if(isActive)
        {
            spriteRenderer.color = activatedColor;
            isSelected = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        spriteRenderer.color = deactivatedColor;
        isSelected = false;
        isActive = true;
    }

    public void Deactivate()
    {
        Debug.Log("Spawn points cannot be deactivated");
    }

    public bool GetIsActive()
    {
        return isActive;
    }

}
