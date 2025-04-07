using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KnownFish : MonoBehaviour
{
    public List<FishData> allFish;
    public List<FishData> currentlyKnownFish;
    public List<FishData> initiallyKnownFish;
    public UnityEvent<FishData> onFishDataKnown;

    public int KnownFishCount => currentlyKnownFish.Count;
    public int AllFishCount => allFish.Count;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (FishData data in initiallyKnownFish)
        {
            RegisterFish(data);
        }

        if (onFishDataKnown==null)
        {
            onFishDataKnown=new UnityEvent<FishData>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RegisterFish(FishData data)
    {
        if (IsKnown(data))
        {
            Debug.Log("Fish already known: " + data.displayName);
            return;
        }

        currentlyKnownFish.Add(data);

        Debug.Log("New fish added to album: " + data.displayName);
        Debug.Log("Fish known now: " + KnownFishCount + "/" + AllFishCount);

        onFishDataKnown.Invoke(data);
    }

    public bool IsKnown(FishData data)
    {
        return currentlyKnownFish.Contains(data);
    }
}