using System;
using DG.Tweening;
using UnityEngine;

public class FinaleTrigger : MonoBehaviour
{
    public Renderer bossRenderer;
    private FlashlightControl _flashlightControl;
    private GameState _gameState;

    public AudioClip photoReverb;

    public FishData bossData;


    private void Start()
    {
        _gameState = FindFirstObjectByType<GameState>();
        _flashlightControl = FindFirstObjectByType<FlashlightControl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<CharacterController>();
        if (player == null)
            return;
        StartFinale();
    }

    public void StartFinale()
    {
        bossRenderer.material.color = Color.black;
        var pos = bossRenderer.transform.parent.position;
        bossRenderer.transform.parent.position += Vector3.down * 15;
        bossRenderer.transform.parent.DOMove(pos, 15f);

        _flashlightControl.flashlight.DOIntensity(0, 2f);

        FindFirstObjectByType<MusicScheduler>().gameObject.SetActive(false);
        MusicManager musicManager = FindFirstObjectByType<MusicManager>();
        musicManager.Stop();

        _gameState.movement.maximumSpeed = 0f;
        _gameState.movement.tethered = false;

        _gameState.isCarryingHose = false;

        Invoke(nameof(StartMusic), 5f);
    }

    private void StartMusic()
    {
        bossRenderer.material.DOColor(Color.white, 10f);
        MusicManager musicManager = FindFirstObjectByType<MusicManager>();
        musicManager.Play(6);
    }

    public void FinalePhoto()
    {
        var musicmanager = FindFirstObjectByType<MusicManager>();
        musicmanager.Stop();

        musicmanager.CreateAudioClip(photoReverb, Vector3.zero);

        _gameState.gameState = GameState.GAME_STATE.CREDITS;

        FindFirstObjectByType<KnownFish>().RegisterFish(bossData);

        Invoke(nameof(PostFinale), 10f);
    }

    public void PostFinale()
    {
        _gameState.movement.transform.position = new Vector3(80.800827f, 15.4800005f, -59.6245499f);
        MusicManager musicManager = FindFirstObjectByType<MusicManager>();
        musicManager.Play(0, true);
    }
}