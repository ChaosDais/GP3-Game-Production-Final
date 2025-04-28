using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAfterDelay : MonoBehaviour
{
    public float delay;

    private void Update()
    {
        delay -= Time.deltaTime;

        if(delay<0) gameObject.SetActive(false);
    }
}
