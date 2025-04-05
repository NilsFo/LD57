using TMPro;
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    private PlayerInteraction _interaction;
    public TMP_Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _interaction = FindFirstObjectByType<PlayerInteraction>();
    }

    // Update is called once per frame
    void Update()
    {
        var focus = _interaction.GetInteractableInFocus();
        if (focus == null)
        {
            text.text = "";
        }
        else
        {
            text.text = "[E]: " + focus.interactionPrompt;
        }
    }
}