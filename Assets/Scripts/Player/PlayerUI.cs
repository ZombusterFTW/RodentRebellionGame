using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    private Health playerHealth;
    public Image healthBarFill;
    public Image frenzyBarFill;
    public TextMeshProUGUI weaponIndentifier;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
