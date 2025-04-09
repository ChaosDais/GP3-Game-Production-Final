using UnityEngine;
using UnityEngine.UI; // if using UI Image for the reticle

public class ReticleController : MonoBehaviour
{
    public LayerMask enemyLayer;
    public LayerMask interactableLayer;
    public LayerMask grappleLayer;

    public Image reticle; 
    public Color enemyColor = Color.red;
    public Color interactableColor = Color.green;
    public Color neutralColor = Color.white;
    public Color defaultColor = Color.gray; //Changed to transparent in inspector

    public float rayDistance = 100f;
    public Camera cam;

    void Update()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            int hitLayer = 1 << hit.collider.gameObject.layer;

            if ((enemyLayer.value & hitLayer) != 0)
            {
                reticle.color = enemyColor;
            }
            else if ((interactableLayer.value & hitLayer) != 0)
            {
                reticle.color = interactableColor;
            }
            else if ((grappleLayer.value & hitLayer) != 0)
            {
                reticle.color = neutralColor;
            }
            else
            {
                reticle.color = defaultColor;
            }
        }
        else
        {
            reticle.color = defaultColor;
        }
    }
}
