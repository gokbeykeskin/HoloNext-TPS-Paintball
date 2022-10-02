using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyMe());
    }

    IEnumerator DestroyMe(){
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
