using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image healthBarFill;
    public Image frenzyBarFill;
    public TextMeshProUGUI weaponIndentifier;
    private PlayerController playerController;
    [SerializeField] GameObject frenzyBar;

    // Start is called before the first frame update
    void Start()
    {
        playerController = PlayerController.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.canPhaseShift || playerController.canEnterRageMode) frenzyBar.SetActive(true);
        else frenzyBar.SetActive(false);
    }

    public void UpdateFrenzyBar(float currentFrenzyLevel, float maxFrenzyLevel)
    {
        float frenzyPercentage = currentFrenzyLevel / maxFrenzyLevel;
        frenzyBarFill.fillAmount = frenzyPercentage;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float healthPercentage = currentHealth / maxHealth;
        healthBarFill.fillAmount = healthPercentage;
    }







}
