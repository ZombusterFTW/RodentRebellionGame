using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Ink.Parsed;


public class FinalBoss : MonoBehaviour, R4Activatable, OneHitHealthEnemy, NewDialougeActivatable
{
    /// <summary>
    /// ////Temp final boss flow. Joe speaks to whiskers and they have an argument with whiskers begging joe to reconsider. This dialouge activates this script, 
    /// on activation the shield appears over whiskers and players must find three switches that disable it.
    /// upon the activation of 3 switches the player can hit whiskers once resulting in his death. This script is an activator that can activate dialouge on its death.
    /// NEED TO ADD REINFORCEMENT CALLS
    /// </summary>
    [Tooltip("The array of switches that must be activated to deactivate the shield.")][SerializeField] private GameObject[] switchesToActivate;
    [Tooltip("This is the dialouge object or objects that will be activated on boss death")][SerializeField] private GameObject[] itemsToActivateOnDeath;
    [Tooltip("This the sprite renderer that will represent Dr Whiskers in rat mode")][SerializeField] private SpriteRenderer drWhiskersSpriteRenderer;
    [SerializeField] private GameObject shield;
    private bool bossActivated = false;
    private int currentSwitchesActivatedCount = 0;
    private bool allSwitchesActivated = false;
    private CapsuleCollider2D whiskersCapsuleCollider;
    [SerializeField] Animator animator;
    [SerializeField] Animator animator2;

    [SerializeField] private TriggerCheckerSupervisor triggerSpawnArea;
    [SerializeField] private Collider2D enemySpawnArea;
    [SerializeField] private float minSpawnTime = 10f;
    [SerializeField] private float maxSpawnTime = 30f;
    [SerializeField] private float minInactiveTime = 25f;
    [SerializeField] private float maxInactiveTime = 50f;
    [SerializeField] private int maxEnemiesSpawned = 5;
    [SerializeField] private GameObject[] enemiesToSpawn;
    private bool isAlive = true;
    public List<GameObject> enemiesSpawned;
    float timeInTrigger = 0;
    float coolDownTime = 0;
    float timeToSpawnEnemy = 0;

    // Start is called before the first frame update
    void Start()
    {
        shield.GetComponent<SpriteRenderer>().DOFade(0, 0);
        shield.SetActive(false);
        whiskersCapsuleCollider = GetComponent<CapsuleCollider2D>();
        foreach (GameObject item in switchesToActivate)
        {
            if (item.GetComponent<Switch>() != null)
            {
                item.GetComponent<Collider2D>().enabled = false;
            }
        }
        timeToSpawnEnemy = Random.Range(minSpawnTime, maxSpawnTime);
    }



    private void Update()
    {
        if (isAlive && bossActivated)
        {
            if (coolDownTime <= 0)
            {
                if (triggerSpawnArea.playerIsInsideOfTrigger)
                {
                    timeInTrigger += Time.deltaTime;
                    if (timeInTrigger >= timeToSpawnEnemy && enemiesSpawned.Count < maxEnemiesSpawned)
                    {
                        SpawnEnemy();
                        animator.SetTrigger("GuardSummon");
                        animator2.SetTrigger("GuardSummon");
                        timeInTrigger = 0;
                        timeToSpawnEnemy = Random.Range(minSpawnTime, maxSpawnTime);
                        coolDownTime = Random.Range(minInactiveTime, maxInactiveTime);
                    }
                }
                else
                {
                    if (timeInTrigger > 0) timeInTrigger -= Time.deltaTime;
                }
            }
            else
            {
                coolDownTime -= Time.deltaTime;
            }
            //Wipe enemies that may have become null references
            enemiesSpawned.RemoveAll(x => !x);
        }
    }

    private void SpawnEnemy()
    {
        Debug.Log("SPAWN ENEMY");
        enemiesSpawned.Add(Instantiate(enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)], RandomPointInBounds(enemySpawnArea.bounds), Quaternion.identity));
    }


    private Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            0
        );
    }


    public void Activate()
    {
        if(!allSwitchesActivated) 
        {
            //This case is when the boss is activated for the first time. Any other activations are treated as the switches needed to deactivate the shield. 
            if (!bossActivated)
            {
                Debug.Log("Activated the whiskers boss for the first time");
                shield.SetActive(true);
                
                //shield.GetComponent<SpriteRenderer>().DOFade(0.25f, 0.5f).SetUpdate(true);
                bossActivated = true;
                //animator.SetTrigger("ShieldUp");
                //animator2.SetTrigger("ShieldUp");
                //Debug.Log("SHIELD");
            }
            else
            {
                Debug.Log("Activated a switch");
                currentSwitchesActivatedCount++;
                allSwitchesActivated = currentSwitchesActivatedCount > switchesToActivate.Length;
                if(allSwitchesActivated)
                {
                    Debug.Log("All switches activated");
                    shield.SetActive(false);
                }
            }
        }
    }

    public void Deactivate()
    {
        Debug.Log("Cannot deactivate the final boss");
    }

    public void OnOneHitEnemyDeath()
    {
        if(allSwitchesActivated)
        {
            isAlive = false;
            Debug.Log("Boss officially dead. Play dialouge now");
            animator.SetTrigger("Death");
            animator2.SetTrigger("Death");
            drWhiskersSpriteRenderer.DOFade(0, 1.5f);
            whiskersCapsuleCollider.enabled = false;
            ActivateItemsOnDeath();
            Destroy(gameObject, 1.5f);
        }
    }

    private void ActivateItemsOnDeath()
    {
        foreach(GameObject item in itemsToActivateOnDeath) 
        {
            if(item.GetComponent<R4Activatable>() != null)
            {
                item.GetComponent<R4Activatable>().Activate();
            }
        }
    }

    public void DialougeActivate()
    {
        shield.SetActive(true);
        //This is here so the shield activation can occur during dialoge.
        shield.GetComponent<SpriteRenderer>().DOFade(0.25f, 0.75f).SetUpdate(UpdateType.Normal, true);
        bossActivated = true;
        animator.SetTrigger("ShieldUp");
        animator2.SetTrigger("ShieldUp");
        foreach (GameObject item in switchesToActivate)
        {
            if (item.GetComponent<Switch>() != null)
            {
                item.GetComponent<Collider2D>().enabled = true;
            }
        }
    }
}

public interface NewDialougeActivatable
{
    public void DialougeActivate();
}