using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheckerSupervisor : MonoBehaviour
{
    public bool playerIsInsideOfTrigger = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerIsInsideOfTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsInsideOfTrigger = false;
        }
    }
}
