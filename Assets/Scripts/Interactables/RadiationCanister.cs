using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
[ExecuteInEditMode]
public class RadiationCanister : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [Tooltip("The upgrade that the raditation canister will grant")][SerializeField] private UpgradeType upgradeType;
    [Tooltip("Wether or not picking up the upgrade can activate items that implement the R4Activatable interface.")][SerializeField] private bool activatesItems = false;
    [Tooltip("The list of doors to open or traps to trigger")][SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
    //[Tooltip("Color of health upgrade.")][SerializeField] private Color healthUpgradeColor = Color.green;
   // [Tooltip("Color of attack upgrade.")][SerializeField] private Color attackUpgradeColor = Color.red;
   // [Tooltip("Color of ability upgrade.")][SerializeField] private Color abilityUpgradeColor = Color.yellow;
    public GameObject upgradeHintObject;
    public AudioSource pickupSound;
    public Sprite attackCanisterImage;
    public Sprite abilityCanisterImage;
    public Sprite healthCanisterImage;
    public Sprite laserGunWeaponImage;
    public Sprite daggerWeaponImage;
    [SerializeField] private Collider2D pickupCollider;

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



    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController controller = collision.gameObject.GetComponent<PlayerController>();

        if (controller != null)
        {
            GameObject hintText = Instantiate(upgradeHintObject);
            hintText.GetComponentInChildren<TextMeshProUGUI>().text = HandleUpgradeHint(upgradeType) + " Acquired!";
            Debug.Log(upgradeType.ToString() + " picked up");
            if (activatesItems) ActivateItems();
            controller.RadiationCanisterPickup(upgradeType);
            pickupSound.Play();
            spriteRenderer.enabled = false;
            pickupCollider.enabled = false;
            Destroy(gameObject, 3.5f);
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
        //Set sprite of canister based on the upgrade type enum.
        spriteRenderer.color = Color.white;
        switch (upgradeType) 
        {
            case UpgradeType.Health_Upgrade:
                spriteRenderer.sprite = healthCanisterImage;
                break;
            case UpgradeType.Attack_Upgrade:
                spriteRenderer.sprite = attackCanisterImage;
                break;
            case UpgradeType.GroundPound_Ability:
                spriteRenderer.sprite = abilityCanisterImage;
                break;
            case UpgradeType.Dash_Ability:
                spriteRenderer.sprite = abilityCanisterImage;
                break;
            case UpgradeType.WallClimb_Ability:
                spriteRenderer.sprite = abilityCanisterImage;
                break;
            case UpgradeType.WallJump_Ability:
                spriteRenderer.sprite = abilityCanisterImage;
                break;
            case UpgradeType.DoubleJump_Ability:
                spriteRenderer.sprite = abilityCanisterImage;
                break;
            case UpgradeType.PhaseShift_Ability:
                spriteRenderer.sprite = abilityCanisterImage;
                break;
            case UpgradeType.RageMode_Ability:
                spriteRenderer.sprite = abilityCanisterImage;
                break;
            case UpgradeType.Dagger_Weapon:
                spriteRenderer.sprite = daggerWeaponImage;
                spriteRenderer.color = Color.white;
                break;
            case UpgradeType.LaserGun_Weapon:
                spriteRenderer.sprite = laserGunWeaponImage;
                spriteRenderer.color = Color.white;
                break;
        }

}

    private string HandleUpgradeHint(UpgradeType upgradeType)
    {
        //Return string based on what the upgrade is
        switch (upgradeType)
        {
            default: return "Error";
            case UpgradeType.Health_Upgrade:
                return "<color=green>15% Health Upgrade";
            case UpgradeType.Attack_Upgrade:
                //Just for Vertical Slice so the player will have Frenzy when prompted.
                GameObject.FindObjectOfType<FrenzyManager>()?.AddToFrenzyMeter(1f);
                return "<color=red>15% Attack Upgrade";
            case UpgradeType.GroundPound_Ability:
                return "<color=yellow>Ground Pound Ability";
            case UpgradeType.WallClimb_Ability:
                return "<color=yellow>Wall Climb Ability";
            case UpgradeType.WallJump_Ability:
                return "<color=yellow>Wall Jump Ability";
            case UpgradeType.DoubleJump_Ability:
                return "<color=yellow>Double Jump Ability";
            case UpgradeType.Dash_Ability:
                return "<color=yellow>Dash Ability";
            case UpgradeType.Dagger_Weapon:
                return "<color=white>Dagger";
            case UpgradeType.LaserGun_Weapon:
                return "<color=white>Laser Gun";
            case UpgradeType.PhaseShift_Ability:
                return "<color=yellow>Phase Shift Ability";
            case UpgradeType.RageMode_Ability:
                return "<color=yellow>Frenzy Mode Ability";
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
    DoubleJump_Ability,
    Dash_Ability,
    Dagger_Weapon,
    LaserGun_Weapon,
    PhaseShift_Ability,
    RageMode_Ability
}
