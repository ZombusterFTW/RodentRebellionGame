using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashWall : MonoBehaviour, R4Activatable
{
    //This trap requires a boxcollider 2d that is impassable unless big joe is dashing. During a dash these walls become passable as the collider is disabled. These walls can also be turned on and off via switches and other items.
    //When a wall is turned off it is permamently in the impassable state. 

    [Tooltip("If the wall will begin in an active state. When deactivated a dash wall is inpassable even when dashing.")][SerializeField] private bool startOn = false;
    private Collider2D dashWallCollider;
    private SpriteRenderer dashWallRenderer;
    private bool isActive = false;
    //For debug
    [SerializeField] private bool isPassable = false;



    // Start is called before the first frame update
    void Start()
    {
        dashWallCollider = GetComponent<Collider2D>();
        dashWallRenderer = GetComponent<SpriteRenderer>();
        dashWallCollider.enabled = true;
        if (startOn) 
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }


    public void Activate()
    {
        isActive = true;
        dashWallRenderer.color = Color.red;
        dashWallCollider.enabled = true;
    }

    public void Deactivate()
    {
        isActive = false;
        dashWallRenderer.color = Color.black;
        dashWallCollider.enabled = true;
    }


    public void TogglePassableState(bool passable)
    {
        if(isActive) 
        {
            if (passable)
            {
                isPassable = true;
                dashWallCollider.enabled = false;
                dashWallRenderer.color = Color.green;
            }
            else
            {
                isPassable = false;
                dashWallCollider.enabled = true;
                dashWallRenderer.color = Color.red;
            }
        }
    }
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
