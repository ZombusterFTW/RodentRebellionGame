using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHazard : MonoBehaviour, R4Activatable
{
    [Tooltip("This hazard can be activated by a button or other activator. Set to true for the hazard to work immeadiatley.")][SerializeField] private bool isActive = true;
    [Tooltip("The color the hazard will be when active")][SerializeField] private Color activeColor = Color.green;
    [Tooltip("The color the hazard will be when inactive")][SerializeField] private Color disabledColor = Color.gray;

    SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
        if (isActive) spriteRenderer.color = activeColor;
        else spriteRenderer.color = disabledColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null && isActive) 
        {
            //Subtract all the player has to eliminate them.
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            Health playerHealth = playerController.GetHealthComponent();
            playerHealth.SubtractFromHealth(playerHealth.GetCurrentHealth());
        }
    }

    public void Activate()
    {
       spriteRenderer.color = activeColor;
       isActive = true;
    }

    public void Deactivate() 
    {
        spriteRenderer.color = disabledColor;
        isActive = false;   
    }
}
