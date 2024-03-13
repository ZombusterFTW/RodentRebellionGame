using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinCollectibleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCounter;
    private int totalCoinCount;
    private int currentCoinCount;

    void Start()
    {
        //Init the total coin counter with the amount of collectible coins in the level.
        Collectible[] collectibles =  GameObject.FindObjectsOfType<Collectible>();
        foreach (Collectible collectible in collectibles)
        {
            if(collectible.GetCollectibleType() == CollectibleType.Coin)
            {
                totalCoinCount++;
            }
        }
        coinCounter.text = currentCoinCount.ToString() + "/" + totalCoinCount.ToString() + " Collectibles";
    }

    public void CollectCoin()
    {
        currentCoinCount++;
        coinCounter.text = currentCoinCount.ToString() + "/" + totalCoinCount.ToString() + " Collectibles";

    }
}
