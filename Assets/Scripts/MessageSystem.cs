using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MessageSystem : MonoBehaviour
{
    private PhotoCamera _photoCamera;

    private KnownFish _knownFish;

    public UIPhotoMessage messagePrefab;

    public Queue<string> msgQueue = new Queue<string>();

    public Sequence currentSeq;

    private GamepadInputDetector _gamepadInputDetector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _knownFish = FindFirstObjectByType<KnownFish>();
        _photoCamera = FindFirstObjectByType<PhotoCamera>();
        _gamepadInputDetector = FindFirstObjectByType<GamepadInputDetector>();

        _knownFish.onFishDataKnown.AddListener(FishCaptured);
        _photoCamera.onBlurryPhoto.AddListener(BlurryPhoto);
    }

    // Update is called once per frame
    void Update()
    {
        if (msgQueue.Count == 0)
            return;

        if (currentSeq != null)
            return;

        string msg = msgQueue.Dequeue();
        ShowMessage(msg);
    }

    public void FishCaptured(FishData data)
    {
        Debug.Log("Message received: New Fish added");

        string keyPrompt = "[TAB]";
        if (_gamepadInputDetector.isGamePad)
        {
            keyPrompt = "[START]";
        }

        EnqueueMessage("NEW SPECIES ADDED\n" + data.displayName.ToUpper() + "\n" + keyPrompt + " TO VIEW");
    }

    public void EnqueueMessage(string text)
    {
        msgQueue.Enqueue(text);
    }

    public void BlurryPhoto()
    {
        Debug.Log("Message received: Blurry");

        string keyPrompt = "[MWHEEL +/-]";
        if (_gamepadInputDetector.isGamePad)
        {
            keyPrompt = "[D-Pad]";
        }

        string msg = "OUT OF FOCUS\nADJUST FOCAL DEPTH\n" + keyPrompt;
        if (!msgQueue.Contains(msg))
            msgQueue.Enqueue(msg);
    }

    public void ShowMessage(string msg)
    {
        Debug.Log("Showing Message: " + msg);
        var msgObj = Instantiate(messagePrefab, transform);
        msgObj.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        var seq = DOTween.Sequence();
        seq.Append(msgObj.transform.DOMove(transform.position + Vector3.down * 200, 0.4f).From());
        seq.AppendInterval(3f);
        seq.Append(msgObj.transform.DOMove(transform.position + Vector3.down * 200, 0.3f));
        seq.onComplete = () =>
        {
            Destroy(msgObj.gameObject);
            currentSeq = null;
        };
        seq.Play();
        currentSeq = seq;
    }
}