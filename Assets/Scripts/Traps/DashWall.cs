using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashWall : MonoBehaviour, R4Activatable
{
    //This trap requires a boxcollider 2d that is impassable unless big joe is dashing. During a dash these walls become passable as the collider is disabled. These walls can also be turned on and off via switches and other items.
    //When a wall is turned off it is permamently in the impassable state. 
    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public void Deactivate()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
