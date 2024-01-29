using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour, R4Activatable
{
    //This script is responcible for the collectible "coins" that can be found in each level. These coins can be "deactivated" in some cases which disallows their pickup. 
    //This script will require a boxcollider or circlecollider 2d that adds to a persistent integer when grabbed. The amount of collectible items is shown at the top right of the screen with the amount of coins in a level being populated.


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
