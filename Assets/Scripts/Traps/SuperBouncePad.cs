using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperBouncePad : MonoBehaviour, R4Activatable
{
    //This script will require a boxcollider 2d with 2 physics materials. One material will be double as bouncy as the normal bouncy material and the other material will be the normal one.
    //When activated the 2x bouncy material will be activated on the the collider with the inverse being true when the trap is deactivated.
    [Tooltip("If set to true the antibounce pad will be active without a button or switch")][SerializeField] private bool startOn = false;
    [Tooltip("Set this value to the default bounciness of the wall physics material 2d")][SerializeField] private float wallBounciness = 0.3f;
    [Tooltip("Wall bounciness will be multipled by this value when the platform is active")][SerializeField] private float bouncinessMultipiler = 2f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D bounceCollider;
    private bool isActive = false;

    PhysicsMaterial2D defaultMaterial;
    PhysicsMaterial2D extraBouncyMaterial;



    public void Activate()
    {
        isActive = true;
        spriteRenderer.color = Color.blue;
        bounceCollider.sharedMaterial = extraBouncyMaterial;

    }

    public void Deactivate()
    {
        isActive = false;
        spriteRenderer.color = Color.gray;
        bounceCollider.sharedMaterial = defaultMaterial;
    }


    // Start is called before the first frame update
    void Start()
    {
        defaultMaterial = new PhysicsMaterial2D("NormalBounciness");
        defaultMaterial.bounciness = wallBounciness;
        extraBouncyMaterial = new PhysicsMaterial2D("NoBounceMaterial");
        extraBouncyMaterial.bounciness = wallBounciness*bouncinessMultipiler;
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