using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundsSwitcher : MonoBehaviour
{
    //This class serves as a container for the polygon 2d collider that acts as the camera bounds. This variable is referenced in the player controller script whenever a player enters a new area. 
    [SerializeField] public PolygonCollider2D cameraBounds { get; private set; }



    // Start is called before the first frame update
    void Start()
    {
        if(cameraBounds == null) 
        {
            cameraBounds = GetComponent<PolygonCollider2D>();   
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
