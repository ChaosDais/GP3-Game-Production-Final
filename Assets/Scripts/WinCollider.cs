using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCollider : MonoBehaviour
{
    public BoxCollider boxTriggerCollider;
    void OnTriggerEnter(Collider COllider)
    {
        if (gameObject.CompareTag("Player")) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("WinScreen");
        }
    }
}
