using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [ExecuteInEditMode]
    //This script is responcible for the collectible "coins" that can be found in each level. These coins can be "deactivated" in some cases which disallows their pickup. 
    //This script will require a boxcollider or circlecollider 2d that adds to a persistent integer when grabbed. The amount of collectible items is shown at the top right of the screen with the amount of coins in a level being populated.


    [Tooltip("The type of the collectible. Health, Frenzy, or Coin")][SerializeField] private CollectibleType collectibleType;
    //[Tooltip("The sprites the collectible can be.")][SerializeField] private Sprite[] collectibleSprites;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;

    [SerializeField] private AudioSource audioSource;
    public CollectibleType GetCollectibleType() { return collectibleType; }
    
    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();  
    }

   
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        switch(collectibleType) 
        {
            default:
                {
                    Debug.Log("Something broke in the Collectible script");
                    break;
                }
            case CollectibleType.Coin:
                {
                    spriteRenderer.color = Color.yellow; 
                    break;
                }
            case CollectibleType.Health: 
                {
                    spriteRenderer.color = Color.green;
                    break;
                }
            case CollectibleType.Frenzy:
                {
                    spriteRenderer.color = Color.red;
                    break;
                }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController controller = collision.gameObject.GetComponent<PlayerController>();
            switch (collectibleType)
            {
                default:
                    {
                        Debug.Log("Something broke in the Collectible script");
                        break;
                    }
                case CollectibleType.Coin:
                    {
                        //Add to the coin collector.
                        if (GameObject.FindObjectOfType<CoinCollectibleManager>() != null)
                        {
                            GameObject.FindObjectOfType<CoinCollectibleManager>().CollectCoin();
                        }
                        break;
                    }
                case CollectibleType.Health:
                    {
                        //Health collectible gives player 15% health
                        controller.GetHealthComponent().AddToHealth(controller.GetHealthComponent().GetCurrentHealth() * 0.15f);
                        break;
                    }
                case CollectibleType.Frenzy:
                    {
                        //Frenzy collectible gives player 15% of their meter. Unhandled null exception but one shouldn't be possible here.
                        GameObject.FindObjectOfType<FrenzyManager>()?.AddToFrenzyMeter(0.15f);
                        break;
                    }
            }
        }
        audioSource.Play();
        circleCollider.enabled = false;
        spriteRenderer.enabled = false;
        Destroy(gameObject, 3.5f);
    }
}



public enum CollectibleType
{
    Coin,
    Health,
    Frenzy
}
