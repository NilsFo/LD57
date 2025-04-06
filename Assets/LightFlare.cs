using UnityEngine;

public class LightFlare : MonoBehaviour
{
    public AnimationCurve lightCurve;

    public float interval = 3f;
    public float offset = 0f;
    
    public Renderer renderer;
    

    private float _timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.position.magnitude / 10f;
        _timer = offset;
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > interval)
        {
            _timer -= interval;
        }

        var alpha = lightCurve.Evaluate(_timer / interval);
        renderer.material.color = new Color(1, 1, 1, alpha);
    }
}
