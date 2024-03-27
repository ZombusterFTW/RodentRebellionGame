using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWall : MonoBehaviour
{
    
    //When in "rubbermode" these walls become passthroughable and glow a distinct color or dissapear alltogether.
    private FrenzyManager frenzyManager;
    private BoxCollider2D wallCollider;
    [SerializeField][Tooltip("The wall image used.")] private WallType wallType = WallType.RubberWall;
    [Tooltip("If set to true the wall will have no collision rat mode")][SerializeField] private bool invertWall = false;
    [SerializeField] private GameObject rubberWalls;
    [SerializeField] private GameObject caveWalls;
    [SerializeField] private GameObject surfaceWalls;

    [ExecuteInEditMode]
    void Start()
    {
        wallCollider = GetComponent<BoxCollider2D>();
        frenzyManager = GameObject.FindObjectOfType<FrenzyManager>();
        switch (wallType)
        {
            case WallType.RubberWall:
                rubberWalls.SetActive(true);
                caveWalls.SetActive(false);
                surfaceWalls.SetActive(false);
                break;
            case WallType.CaveWall:
                rubberWalls.SetActive(false);
                caveWalls.SetActive(true);
                surfaceWalls.SetActive(false);
                break;
            case WallType.SurfaceWall:
                rubberWalls.SetActive(false);
                caveWalls.SetActive(false);
                surfaceWalls.SetActive(true);
                break;
        }
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

public enum WallType
{
    RubberWall,
    CaveWall,
    SurfaceWall
}
