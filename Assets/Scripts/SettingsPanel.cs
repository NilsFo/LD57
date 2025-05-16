using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    private MouseLook mouseLook;
    private GameState gameState;
    private MusicManager musicManager;

    public Slider volumeSlider;
    public Slider sensitivitySlider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicManager = FindFirstObjectByType<MusicManager>();
        mouseLook = FindFirstObjectByType<MouseLook>(FindObjectsInactive.Include);

        volumeSlider.value = MusicManager.userDesiredMasterVolume;
        sensitivitySlider.value = mouseLook.sensitivitySettings;
    }

    // Update is called once per frame
    void Update()
    {
        MusicManager.userDesiredMasterVolume = volumeSlider.value;
        mouseLook.sensitivitySettings = sensitivitySlider.value;
    }
}