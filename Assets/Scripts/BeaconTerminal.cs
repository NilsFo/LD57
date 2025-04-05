using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BeaconTerminal : MonoBehaviour
{

    public enum BeaconState : UInt16
    {
        WAITING_FOR_PLAYER,
        FISH_COUNT
    }

    private MouseLook mouseLook;
    public float activationDistance = 20f;

    public BeaconState beaconState = BeaconState.WAITING_FOR_PLAYER;
    public TMP_Text displayText;
    public GameObject canvasHolder;

    [Header("Hose timer")]
    public float hose_wait_time = 10;
    public float _hose_wait_time_current = 0;
    public bool hose_available = false;
    private bool hose_event_fired = false;
    public UnityEvent onHoseAvailable;

    [Header("Fish")]
    public List<FishData> nearbyFish;
    public List<FishData> nearbyFishForce;
    private KnownFish _knownFish;
    public float fishDiscoveryRange;

    [Header("Debug ranges")]
    public bool showFishDiscoverRange = false;
    public bool showActivationDistanceRange = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _knownFish = FindFirstObjectByType<KnownFish>();
        mouseLook = FindAnyObjectByType<MouseLook>();
        hose_event_fired=false;

        if (onHoseAvailable == null) onHoseAvailable = new UnityEvent();

        foreach (var fish in nearbyFishForce) AddNearbyFish(fish);
    }

    // Update is called once per frame
    void Update()
    {
        float player_distance = Vector3.Distance(transform.position, mouseLook.transform.position);
        if (player_distance > activationDistance)
        {
            canvasHolder.SetActive(false);
            return;
        }

        canvasHolder.SetActive(true);

        switch (beaconState)
        {
            case BeaconState.WAITING_FOR_PLAYER:
                displayText.text = "AWAITING\n\nUSER\n\nINPUT";
                break;
            case BeaconState.FISH_COUNT:
                _hose_wait_time_current += Time.deltaTime;

                int hose_progress = (int)(_hose_wait_time_current / hose_wait_time * 100);
                hose_progress = Math.Min(hose_progress, 100);
                string hose_progress_s = hose_progress + "";
                if (hose_progress < 10)
                {
                    hose_progress_s = "00" + hose_progress;
                }
                else
                if (hose_progress < 100)
                {
                    hose_progress_s = "0" + hose_progress;
                }

                if (hose_progress == 100)
                {
                    hose_available = true;
                    if (!hose_event_fired)
                    {
                        hose_event_fired = true;
                        OnHoseAvailable();
                    }
                }

                var progress_text = "COMPRESSING\n" +
                                    "OXYGEN:\n" +
                                    hose_progress_s + "%\n" +
                                    "\n" +
                                    "UNKNOWN\n" +
                                    "SPECIES\n" +
                                    "DETECTED: " + GetUnknownFishCountNearby();
                displayText.text = progress_text;
                break;
        }
    }

    public void AddNearbyFish(FishData data)
    {
        if (!nearbyFish.Contains(data))
        {
            nearbyFish.Add(data);
        }
    }

    public int GetUnknownFishCountNearby()
    {
        int count = 0;

        foreach (var fish in nearbyFish)
        {
            if (!_knownFish.IsKnown(fish))
            {
                count++;
            }
        }

        return count;
    }

    public void ActivateByPlayer()
    {
        beaconState = BeaconState.FISH_COUNT;
    }

    private void OnHoseAvailable()
    {
        onHoseAvailable.Invoke();
        Debug.Log("Beacon hose is now available:" + name);
    }


#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        if (showFishDiscoverRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, fishDiscoveryRange);
        }

        if (showActivationDistanceRange)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, activationDistance);
        }
    }

#endif

}
