using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyandDoor : MonoBehaviour, R4Activatable
{
    [Tooltip("The color the door and key will be. Changed at runtime")][SerializeField] private Color doorColor = Color.white;
    [Tooltip("The name of the door. If left undefined it will be determined by the color set")][SerializeField] private string doorColorString = null;
    [Tooltip("If the door is active. If it is inactive, the key will be invisible until activated.")][SerializeField] private bool isDoorActive = true;
    [Tooltip("The key connected to the door")][SerializeField] private GameObject doorKey;
    private bool keyPickedUp = false;
    private Collider2D keyCollider;
    private BoxCollider2D doorCollider;
    private PlayerController playerController;
    private Collider2D playerCollider;
    [SerializeField] AudioSource keyAudio;
    [SerializeField] AudioSource doorAudio;
    [SerializeField] GameObject doorKeyHint;
    public void Activate()
    {
        doorKey.gameObject.SetActive(true);
        isDoorActive = true;
    }
    public void Deactivate() 
    {
        if(doorKey != null) doorKey.gameObject.SetActive(false);
        isDoorActive=false;
    }


    // Start is called before the first frame update
    void Start()
    {
        doorCollider = GetComponent<BoxCollider2D>();   
        if (doorColorString == null) doorColorString = doorColor.ToString();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerCollider = playerController.GetPlayerCollider();
        GetComponent<SpriteRenderer>().color = doorColor;
        doorKey.GetComponent<SpriteRenderer>().color = doorColor;
        keyCollider = doorKey.GetComponent<Collider2D>();




        if (!isDoorActive)
        {
            doorKey.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!keyPickedUp)
        {
            if (keyCollider != null && keyCollider.IsTouching(playerCollider))
            {
                GameObject hintText = Instantiate(doorKeyHint);
                hintText.GetComponentInChildren<TextMeshProUGUI>().text = "Picked up " + doorColorString + " door key";
                hintText.GetComponentInChildren<TextMeshProUGUI>().color = doorColor;
                Debug.Log("Picked up " + doorColorString + " door key");
                keyPickedUp = true;
                keyAudio.Play();
                doorKey.GetComponent<SpriteRenderer>().enabled = false;
                doorKey.GetComponent<CircleCollider2D>().enabled = false;
                Destroy(doorKey.gameObject, 3.5f);
                
            }
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == playerCollider && keyPickedUp && isDoorActive)
        {
            //Door opened.
            Debug.Log("Unlocked " + doorColorString + " door");
            GameObject hintText = Instantiate(doorKeyHint);
            hintText.GetComponentInChildren<TextMeshProUGUI>().text = "Unlocked " + doorColorString + " door";
            hintText.GetComponentInChildren<TextMeshProUGUI>().color = doorColor;
            doorAudio.Play();
            doorCollider.enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject, 3.5f);
        }
    }
}
