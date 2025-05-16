using TMPro;
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    public GameObject iconHolder;
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
        PlayerInteractable focus = _interaction.GetInteractableInFocus();
        if (focus == null)
        {
            text.text = "";
            iconHolder.SetActive(false);
        }
        else
        {
            if (focus.isInteractable)
            {
                text.text = focus.interactionPrompt;
                iconHolder.SetActive(true);
            }
            else
            {
                text.text = "";
                iconHolder.SetActive(false);
            }
        }
    }
}