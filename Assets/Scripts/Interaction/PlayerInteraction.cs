using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInteraction : MonoBehaviour
{
    private MouseLook playerLook;

    [FormerlySerializedAs("interactionDistance")]
    public float interactionCylinderRadius = 2.5f;

    public float interactionCylinderHeight = 2.5f;

    private PlayerInteractable _interactableCache;
    public bool HasInteractableInFocus => GetInteractableInFocus() != null;

    private void Start()
    {
        playerLook = FindFirstObjectByType<MouseLook>();
        Debug.Assert(playerLook != null, "parentPlayer is missing!", gameObject);
    }

    private void LateUpdate()
    {
        // invalidating cache
        _interactableCache = null;
    }

    public (bool, RaycastHit) GetInFocus(float distance, int layerMask = -1)
    {
        if (layerMask == -1)
            layerMask = LayerMask.GetMask("Default");

        // MouseLook mainCamera = parentPlayer.mouseLook;
        bool isHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward,
            out RaycastHit hit, distance, layerMask);
        return (isHit, hit);
    }

    public (bool, RaycastHit) GetInFocusCylinder(int layerMask = -1)
    {
        return GetInFocusCylinder(interactionCylinderRadius, interactionCylinderHeight, layerMask);
    }

    public (bool, RaycastHit) GetInFocusCylinder(float radius, float height, int layerMask = -1)
    {
        if (layerMask == -1)
            layerMask = LayerMask.GetMask("Default", "Entities", "Slot");

        //MouseLook mainCamera = parentPlayer.mouseLook;
        bool isHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward,
            out RaycastHit hit, Mathf.Sqrt(radius * radius + height * height), layerMask);
        if (isHit)
        {
            if (Vector3.ProjectOnPlane(hit.point - playerLook.transform.position, Vector3.up).magnitude <= radius)
            {
                print("hit");
                return (true, hit);
            }
        }

        return (false, hit);
    }

    public PlayerInteractable GetInteractableInFocus()
    {
        if (_interactableCache != null)
        {
            return _interactableCache;
        }

        var (isHit, hit) = GetInFocusCylinder(interactionCylinderRadius, interactionCylinderHeight);
        if (isHit)
        {
            PlayerInteractable hitInteractable = hit.collider.GetComponentInParent<PlayerInteractable>();
            if (hitInteractable != null)
            {
                _interactableCache = hitInteractable;
            }
            
            print("hit");

            return hitInteractable;
        }

        return null;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        
        PlayerInteractable interactableInFocus = GetInteractableInFocus();
        if (interactableInFocus.IsDestroyed() || interactableInFocus == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Collider c = interactableInFocus.GetComponent<Collider>();
        if (c != null)
            Gizmos.DrawWireCube(c.bounds.center, c.bounds.size);

        Gizmos.DrawLine(playerLook.transform.position,
            interactableInFocus.transform.position);
    }
#endif
}