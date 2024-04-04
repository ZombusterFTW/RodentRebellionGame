using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceReturnToLobby : MonoBehaviour, R4Activatable
{
    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public void Deactivate()
    {
        Debug.Log("It is not possible to deactivate this script");
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
