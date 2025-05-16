using UnityEngine;
using UnityEngine.Events;

public class PhotoListener : MonoBehaviour
{
    public UnityEvent onPhotoTaken;
    public UnityEvent onBlurryPhotoTaken;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (onPhotoTaken == null)
        {
            onPhotoTaken = new UnityEvent();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPhotoTaken()
    {
        onPhotoTaken.Invoke();
    }

    public void OnPhotoOutOfFocus()
    {
        Debug.Log("Photo taken but im fuken blurry lmao");
        onBlurryPhotoTaken.Invoke();
    }
}