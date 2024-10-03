using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public DestructMethods destructOptions = DestructMethods.OnTime;
    public float timeToDestruct = 3f;
    public string collisionObjectTag;

    public GameObject destructEffect;
    void Update()
    {
        if ((destructOptions & DestructMethods.OnTime) == DestructMethods.OnTime)
        {
            timeToDestruct -= Time.deltaTime;
            if (timeToDestruct <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Destruction()
    {
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {
        if ((destructOptions & DestructMethods.OnCollision) == DestructMethods.OnCollision)
        {
            // Your collision logic here
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        if (destructEffect != null)
        {
            Instantiate(destructEffect, transform.position, transform.rotation);
        }
    }
}
[System.Flags]
public enum DestructMethods
{
    None = 0,
    OnTime = 1 << 0,  // 1
    OnCollision = 1 << 1,  // 2
    // OptionThree = 1 << 2,  // 4
    // OptionFour = 1 << 3   // 8
}