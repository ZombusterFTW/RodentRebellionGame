using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoorCompanionKeypad : MonoBehaviour
{
    //This class is just a door unlock keypad changer.

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite keypadRed;
    [SerializeField] private Sprite keypadGreen;


    public void ToggleDoorKeypad(bool unlocked)
    {
        if(unlocked) 
        {
            spriteRenderer.sprite = keypadGreen; 
        }
        else
        {
            spriteRenderer.sprite = keypadRed;
        }
    }
}
