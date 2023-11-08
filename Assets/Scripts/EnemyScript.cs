using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour, ControlledCharacter
{
    [SerializeField] GameObject playerUI;
    private GameObject UIClone;
    private Health health;


    public PlayerUI GetPlayerUI()
    {
        return UIClone.GetComponent<PlayerUI>();
    }

    public void RespawnPlayer()
    {
        health.HealthToMax();
    }

    // Start is called before the first frame update
    void Awake()
    {
        UIClone = Instantiate(playerUI, transform.position + new Vector3(0,2,0), Quaternion.identity);
        UIClone.transform.parent = transform;
        //UIClone.GetComponent<Canvas>().worldCamera = Camera.main;
        health = GetComponent<Health>();    
    }


    public Health GetHealth() { return health; }

    // Update is called once per frame
    void Update()
    {
        
    }
}
