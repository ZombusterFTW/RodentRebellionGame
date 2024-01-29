using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperBouncePad : MonoBehaviour, R4Activatable, R4ActivatableTrap
{
    //This script will require a boxcollider 2d with 2 physics materials. One material will be double as bouncy as the normal bouncy material and the other material will be the normal one.
    //When activated the 2x bouncy material will be activated on the the collider with the inverse being true when the trap is deactivated.




    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public void Deactivate()
    {
        throw new System.NotImplementedException();
    }

    public void DealPlayerDamage(bool damage)
    {
        throw new System.NotImplementedException();
    }

    public void TriggerTrap()
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
