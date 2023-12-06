using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActivater : MonoBehaviour
{

    [SerializeField] bool activateItems;
    [SerializeField] GameObject[] itemsToActivate;

   public void Trigger()
    {
        if(activateItems) 
        {
            foreach(GameObject item in itemsToActivate) 
            {
                item.GetComponent<R4Activatable>()?.Activate();
            }
        }
        else
        {
            foreach (GameObject item in itemsToActivate)
            {
                item.GetComponent<R4Activatable>().Deactivate();
            }
        }
        Destroy(gameObject);
    }
}
