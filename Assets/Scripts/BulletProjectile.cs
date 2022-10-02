using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigidbody;
    void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        StartCoroutine(DestroyMe());

    }
    void Start()
    {
        float speed=50f;
        bulletRigidbody.velocity = transform.forward*speed;
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    IEnumerator DestroyMe(){
        yield return new WaitForSeconds(6f);
        Destroy(gameObject);
    }
}
