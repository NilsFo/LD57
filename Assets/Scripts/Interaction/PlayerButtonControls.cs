using UnityEngine;

public class PlayerButtonControls : MonoBehaviour
{
    private PlayerInteraction _interaction;

    private void Start()
    {
        _interaction = FindFirstObjectByType<PlayerInteraction>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var inFocus = _interaction.GetInteractableInFocus();
            if (inFocus != null)
            {
                inFocus.Interact();
            }
            else
            {
                print("nichts im focus");
            }
        }
    }
}