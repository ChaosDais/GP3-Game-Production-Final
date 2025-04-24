using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

public class LoadWin : MonoBehaviour
{
    public BoxCollider boxTriggerCollider;

    void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("Player"))
        {
            Debug.Log("Player touched the win trigger and collected a tome!");
            string currentScene = SceneManager.GetActiveScene().name;


            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("WinScreen");

        }
    }
}
