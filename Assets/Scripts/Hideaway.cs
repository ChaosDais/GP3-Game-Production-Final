using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

public class Hideaway : MonoBehaviour
{
    vThirdPersonController player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<vThirdPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.hidden = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.hidden = false;
        }
    }
}
