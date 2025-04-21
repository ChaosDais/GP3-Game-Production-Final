using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

public class FastDash : MonoBehaviour
{
    public float dashSpeed = 80f;
    public float dashTime = 1f;
    public float dashCooldown = 0.1f;
    public AudioSource dashSound;

    private float dashCDCurrent;
    vThirdPersonInput input;
    Transform playerTransform;
    Rigidbody body;
    bool canDash = true;

    void Start()
    {
        input = gameObject.GetComponent<vThirdPersonInput>();
        body = gameObject.GetComponent<Rigidbody>();
        playerTransform = gameObject.transform;
        dashCDCurrent = dashCooldown;
    }

    private void OnEnable()
    {
        print("Fast Dash active!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(input.abilityInput) && canDash)
        {
            print("Dash!");
            StartCoroutine(Dash());
        }
        else if (Input.GetKeyDown(input.abilityInput)){
            print("Can't dash yet!");
        }
    }

    IEnumerator Dash()
    {
        if(dashSound != null)
            dashSound.Play();

        Quaternion yaw = Quaternion.Euler(0, playerTransform.eulerAngles.y, 0);
        Vector3 movement = yaw * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        body.AddForce(new Vector3(movement.x * dashSpeed, 0f, movement.z * dashSpeed), ForceMode.Impulse);
        Vector3 flatVel = new Vector3(body.velocity.x, 0f, body.velocity.z);

        yield return new WaitForSeconds(dashTime);

        dashCDCurrent = 0;
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);

        dashCDCurrent = dashCooldown;
        canDash = true;
        print("Cooldown refreshed.");
    }
}
