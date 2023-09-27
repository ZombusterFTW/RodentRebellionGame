using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricTrap : MonoBehaviour, R4Activatable, R4ActivatableTrap
{
    private PlayerController controller;
    [SerializeField] private bool isActive = true;
    [SerializeField] private bool selfActivating = true;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        if (!selfActivating) isActive = true;
    }

    public void Deactivate()
    {
        if (!selfActivating) isActive = false;
    }

    public void TriggerTrap()
    {
        if (selfActivating)
        {
            //Trigger trap by player hitting prox trigger. 
            Debug.Log("Trap triggered");
        }
        else Debug.Log("Trap triggered by button or other activator.");
    }
}
