using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{

    public List<FishDatabaseEntry> fishPoser;
    private GameState _gameState;
    private MouseLook mouseLook;
    private CharacterController controller;
    public TMP_Text descriptionTF;
    private KnownFish knownFish;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameState = FindFirstObjectByType<GameState>();
        knownFish = FindFirstObjectByType<KnownFish>();
        mouseLook = FindFirstObjectByType<MouseLook>();
        controller = FindFirstObjectByType<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        FishData selectedData = null;
        foreach (var poster in fishPoser)
        {
            if (poster.isHovered)
            {
                selectedData=poster.fishData;
            }
        }

        var description = "Mouse over species to learn more.";
        if (selectedData!=null)
        {
            if (knownFish.IsKnown(selectedData))
            {
                description=selectedData.displayName;
            }else{
                description=selectedData.whereFindText;
            }
        }

        descriptionTF.text = description;
    }

    public void OnContinuePressed()
    {
        _gameState.gameState = GameState.GAME_STATE.PLAYING;
    }

    public void OnRestartPressed()
    {
        _gameState.BackToMenu();
    }

}
