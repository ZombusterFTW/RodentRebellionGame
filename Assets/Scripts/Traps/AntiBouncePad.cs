using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiBouncePad : MonoBehaviour, R4Activatable, R4ActivatableTrap
{
    //This script will require a boxcollider 2d with 2 2d physics materials. One of these materials will be the normal rubber bounce material and the other one will be have no bounce
    //When activated the non bounce material will be active on the boxcollider and the inverse is true when deactivated.
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
