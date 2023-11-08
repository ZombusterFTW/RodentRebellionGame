using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroya : MonoBehaviour
{
    public float timeBeforeDestroy = 3.5f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeBeforeDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
