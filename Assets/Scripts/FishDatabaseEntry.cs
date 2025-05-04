using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FishDatabaseEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public FishData fishData;
    public Image wantedSprite;
    public Image fishSprite;
    private KnownFish knownFish;

    public bool isHovered = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        knownFish = FindFirstObjectByType<KnownFish>();
        isHovered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (knownFish.IsKnown(fishData))
        {
            wantedSprite.gameObject.SetActive(false);
            fishSprite.gameObject.SetActive(true);
        }
        else
        {
            wantedSprite.gameObject.SetActive(true);
            fishSprite.gameObject.SetActive(false);
        }
    }

    void OnMouseEnter()
    {
        isHovered = true;
    }

    void OnMouseExit()
    {
        isHovered = false;
    }

    void OnEnable()
    {
        isHovered = false;
    }

    void OnDisable()
    {
        isHovered = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}