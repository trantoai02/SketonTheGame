using System.Collections.Generic;
using UnityEngine;

    public class ThrowObject : FakeHeightObject
    {

    [SerializeField] float lifeTime = 5f;
    bool hasHit = false;
    private void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Indestructible>() != null)
        {
            hasHit = true;
            StopMoving();
            return;
        }
    }

    void StopMoving()
    {
        FakeHeightObject f = GetComponent<FakeHeightObject>();
        if (f != null)
            f.Stick();
    }
}


