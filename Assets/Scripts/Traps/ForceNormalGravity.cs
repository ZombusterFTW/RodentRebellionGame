using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceNormalGravity : MonoBehaviour
{
    //This trigger serves to forcibly remove a player from being gravity flipped if they don't have normal gravity.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerController>().isFlipped) collision.gameObject.GetComponent<PlayerController>().ToggleGravityFlip();
        }
    }
}
