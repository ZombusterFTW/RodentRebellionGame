using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiBouncePad : MonoBehaviour, R4Activatable
{
    //This script will require a boxcollider 2d with 2 2d physics materials. One of these materials will be the normal rubber bounce material and the other one will be have no bounce
    //When activated the non bounce material will be active on the boxcollider and the inverse is true when deactivated.


    [Tooltip("If set to true the antibounce pad will be active without a button or switch")][SerializeField] private bool startOn = false;
    [Tooltip("Set this value to the default bounciness of the wall physics material 2d")][SerializeField] private float wallBounciness = 0.3f;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D bounceCollider;
    private bool isActive = false;

    PhysicsMaterial2D defaultMaterial;
    PhysicsMaterial2D noBouncyMaterial;



    public void Activate()
    {
        isActive = true;
        spriteRenderer.color = Color.red;
        bounceCollider.sharedMaterial = noBouncyMaterial;
        
    }

    public void Deactivate()
    {
        isActive = false;
        spriteRenderer.color = Color.green;
        bounceCollider.sharedMaterial = defaultMaterial;
    }

  
    // Start is called before the first frame update
    void Start()
    {
        defaultMaterial = new PhysicsMaterial2D("NormalBounciness");
        defaultMaterial.bounciness = wallBounciness;
        noBouncyMaterial = new PhysicsMaterial2D("NoBounceMaterial");
        noBouncyMaterial.bounciness = 0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        bounceCollider = GetComponent<BoxCollider2D>();
        if (startOn) 
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

}
