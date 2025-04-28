using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAfterDelay : MonoBehaviour
{
    public float delay;
    public GameObject obj;

    private void Start()
    {
        obj.SetActive(false);
    }

    private void Update()
    {
        delay -= Time.deltaTime;

        if(delay<0) obj.SetActive(true);
    }
}
