using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinCollectibleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCounter;
    private int totalCoinCount;
    private int currentCoinCount;

    void Start()
    {
        totalCoinCount = 0;
        currentCoinCount = 0;
        SceneManager.sceneLoaded += OnSceneChange;
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

    private void OnSceneChange(Scene arg0, LoadSceneMode arg1)
    {
        totalCoinCount = 0;
        currentCoinCount = 0;
        //Init the total coin counter with the amount of collectible coins in the level.
        Collectible[] collectibles = GameObject.FindObjectsOfType<Collectible>();
        foreach (Collectible collectible in collectibles)
        {
            if (collectible.GetCollectibleType() == CollectibleType.Coin)
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

    public int GetCurrentCoinCount()
    {
        return currentCoinCount;
    }
    public int GetTotalCoinCount() 
    {
        return totalCoinCount;
    }
}
