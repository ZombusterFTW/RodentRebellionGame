using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //This is the projectile that will go towards players.
    public Vector2 targetPosition = Vector2.zero;
    public float projectileSpeed = 50f;
    public float damageByProjectile = 15f;
    private Vector2 playerDirection;
    private Vector2 startingPos;
    private FrenzyManager frenzyManager;
    private CircleCollider2D circleCollider;
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        frenzyManager = FrenzyManager.instance;
        playerDirection = targetPosition - (Vector2)gameObject.transform.position;
        playerDirection.Normalize();
        startingPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!frenzyManager.inRubberMode) 
        {
            circleCollider.enabled = true;
            gameObject.transform.position += (Vector3)(playerDirection * projectileSpeed * Time.deltaTime);
            //gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, targetPosition, projectileSpeed * Time.deltaTime);
        }
        else
        {
            circleCollider.enabled = false;
        }
        if(Vector2.Distance(startingPos, (Vector2)gameObject.transform.position) >= 25f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!GameObject.ReferenceEquals(collision.gameObject.GetComponent<PlayerController>(), null))
        {
            collision.gameObject.GetComponent<Health>().SubtractFromHealth(damageByProjectile);
        }
        Destroy(gameObject);
    }
}
