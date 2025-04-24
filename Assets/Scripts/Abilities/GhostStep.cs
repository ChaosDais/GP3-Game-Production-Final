using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GhostStep : MonoBehaviour
{
    public GameObject cooldownMeter;
    public AudioSource activateSound;
    public AudioSource deactivateSound;
    public float cooldown = 5f;
    public float duration = 5f;

    bool usable = true;
    [HideInInspector] public bool hidden = false;
    bool onCD = false;
    vThirdPersonInput input;
    Slider cooldownSlider;
    float cdCurrent;

    void Start()
    {
        input = gameObject.GetComponent<vThirdPersonInput>();
        cooldownSlider = cooldownMeter.GetComponent<Slider>();
        cdCurrent = cooldownSlider.value;
    }

    private void Update()
    {
        if (Input.GetKeyDown(input.abilityInput) && usable)
        {
            print("Cast Ghost Step!");
            StartCoroutine(Hide());
        }
    }

    private void FixedUpdate()
    {
        if (onCD) cdCurrent += Time.deltaTime;

        cooldownSlider.value = cdCurrent / cooldown;
        cooldownMeter.SetActive(cdCurrent < cooldown && onCD);
    }

    IEnumerator Hide()
    {
        if(activateSound != null) activateSound.Play();
        hidden = true;
        usable = false;

        yield return new WaitForSeconds(duration);

        if(deactivateSound != null) deactivateSound.Play();
        hidden = false;
        onCD = true;
        cdCurrent = 0;

        yield return new WaitForSeconds(cooldown);

        onCD = false;
        cdCurrent = cooldown;
        usable = true;

    }

}
