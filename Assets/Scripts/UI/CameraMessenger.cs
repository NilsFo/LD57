using UnityEngine;

public class CameraMessenger: MonoBehaviour
{
    private KnownFish _knownFish;
    private PhotoCamera _photoCamera;

    public MessageSystem messageSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _knownFish = FindFirstObjectByType<KnownFish>();
        _photoCamera = FindFirstObjectByType<PhotoCamera>();
        
        _knownFish.onFishDataKnown.AddListener(FishCaptured);
        _photoCamera.onBlurryPhoto.AddListener(BlurryPhoto);
    }
    
    public void FishCaptured(FishData data)
    {
        Debug.Log("Message received: New Fish added");
        messageSystem.EnqueueMessage(
            "NEW SPECIES ADDED\n" + data.displayName.ToUpper() + "\n[TAB] TO VIEW", 
            important: true
        );
    }

    public void BlurryPhoto()
    {
        Debug.Log("Message received: Blurry");
        var msg = "OUT OF FOCUS\nADJUST FOCAL DEPTH\n[MWHEEL +/-]";
        messageSystem.EnqueueMessage(msg);
    }
}
