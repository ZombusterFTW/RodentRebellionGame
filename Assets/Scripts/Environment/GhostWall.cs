using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWall : MonoBehaviour
{
    //When in "rubbermode" these walls become passthroughable and glow a distinct color or dissapear alltogether.
    private FrenzyManager frenzyManager;
    private BoxCollider2D wallCollider;
    [Tooltip("If set to true the wall will have no collision rat mode")][SerializeField] private bool invertWall = false;
    
    void Start()
    {
        wallCollider = GetComponent<BoxCollider2D>();
        frenzyManager = GameObject.FindObjectOfType<FrenzyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!invertWall)
        {
            if (frenzyManager.inRubberMode)
            {
                wallCollider.enabled = false;
            }
            else
            {
                wallCollider.enabled = true;
            }
        }
        else
        {
            if (!frenzyManager.inRubberMode)
            {
                wallCollider.enabled = false;
            }
            else
            {
                wallCollider.enabled = true;
            }
        }
    }
}
