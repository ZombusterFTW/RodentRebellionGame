using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiationCanister : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [Tooltip("The upgrade that the raditation canister will grant")][SerializeField] private UpgradeType upgradeType;
    [Tooltip("Wether or not picking up the upgrade can activate items that implement the R4Activatable interface.")][SerializeField] private bool activatesItems = false;
    [Tooltip("The list of doors to open or traps to trigger")][SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
    [Tooltip("Color of health upgrade.")][SerializeField] private Color healthUpgradeColor = Color.green;
    [Tooltip("Color of attack upgrade.")][SerializeField] private Color attackUpgradeColor = Color.red;
    [Tooltip("Color of ability upgrade.")][SerializeField] private Color abilityUpgradeColor = Color.yellow;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        HandleUpgradeColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController controller = collision.gameObject.GetComponent<PlayerController>();

        if(controller != null) 
        {
            Debug.Log(upgradeType.ToString() + " picked up");
            if(activatesItems) ActivateItems();
            Destroy(gameObject);
        }
    }


    void ActivateItems()
    {
        //Ensure that the list doesn't contain a null item.
        objectsToActivate.RemoveAll(x => x == null);
        foreach (var item in objectsToActivate)
        {
            //Check if item has the activatable interface and if so, we activate.
            if(item.GetComponent<R4Activatable>() != null)
            {
                item.GetComponent<R4Activatable>().Activate();
            }
        }
    }


    void HandleUpgradeColor()
    {
        //Set color of canister based on the upgrade type enum.
        switch(upgradeType) 
        {
            case UpgradeType.Health_Upgrade:
                spriteRenderer.color = healthUpgradeColor;
                break;
            case UpgradeType.Attack_Upgrade: 
                spriteRenderer.color = attackUpgradeColor;
                break;
            case UpgradeType.GroundPound_Ability:
                spriteRenderer.color = abilityUpgradeColor;
                break;
            case UpgradeType.WallClimb_Ability:
                spriteRenderer.color = abilityUpgradeColor;
                break;
            case UpgradeType.WallJump_Ability:
                spriteRenderer.color = abilityUpgradeColor;
                break;
            case UpgradeType.DoubleJump_Ability:
                spriteRenderer.color = abilityUpgradeColor;
                break;
        }
    }



}

public interface R4Activatable
{
    void Activate();
    void Deactivate();
}


public enum UpgradeType
{
    Health_Upgrade,
    Attack_Upgrade,
    GroundPound_Ability,
    WallClimb_Ability,
    WallJump_Ability,
    DoubleJump_Ability
}
