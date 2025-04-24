using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.UI;

public class FastDash : MonoBehaviour
{
    public float dashSpeed = 80f;
    public float dashTime = 1f;
    public float dashCooldown = 0.1f;
    public AudioSource dashSound;
    public GameObject cooldownMeter;

    private float dashCDCurrent;
    vThirdPersonInput input;
    Transform playerTransform;
    Rigidbody body;
    bool canDash = true;
    Slider dashSlider;

    void Start()
    {
        input = gameObject.GetComponent<vThirdPersonInput>();
        body = gameObject.GetComponent<Rigidbody>();
        playerTransform = gameObject.transform;
        dashCDCurrent = dashCooldown;
        dashSlider = cooldownMeter.GetComponent<Slider>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(input.abilityInput) && canDash)
            StartCoroutine(Dash());
        else if (Input.GetKeyDown(input.abilityInput))
            print("Can't dash yet!");
    }

    void FixedUpdate()
    {
        if (!canDash) dashCDCurrent += Time.deltaTime;
     
        dashSlider.value = dashCDCurrent / dashCooldown;
        cooldownMeter.SetActive(dashCDCurrent < dashCooldown);
    }

    IEnumerator Dash()
    {
        if(dashSound != null) dashSound.Play();
        Quaternion yaw = Quaternion.Euler(0, playerTransform.eulerAngles.y, 0);
        Vector3 movement = yaw * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        body.AddForce(new Vector3(movement.x * dashSpeed, 0f, movement.z * dashSpeed), ForceMode.Impulse);

        yield return new WaitForSeconds(dashTime);

        dashCDCurrent = 0;
        canDash = false;

        yield return new WaitForSeconds(dashCooldown);

        dashCDCurrent = dashCooldown;
        canDash = true;
    }
}
