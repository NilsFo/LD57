using Unity.Mathematics;
using UnityEngine;

public class ViewmodelSway : MonoBehaviour
{
    public CharacterMovement playerMovement;
    public MouseLook mouseLook;
    [Range(0, 10000)] public float swayStrength = 5f;
    private float _swayMultiplier; // defaults to 0.0005f
    public AudioSource stepSound;

    void Start()
    {
        _swayMultiplier = swayStrength * 0.0001f;

        if (playerMovement == null)
            playerMovement = FindFirstObjectByType<CharacterMovement>();
        if (mouseLook == null)
            mouseLook = FindFirstObjectByType<MouseLook>();
    }

    private Vector3 _swayVec;
    private float _walkSine = 0f;
    public float step_size = 0.8f;
    public float walk_sway = 85f;
    public float walk_recover = 5f;
    public float view_rotation_recover = 5f;
    public Vector2 view_rotation_intensity;


    public float breath_period = 3f;
    private Vector3 breath_transform = Vector3.zero;

    private float _breath = 0f;

    private Vector2 _mouseMovVec;

    // Update is called once per frame
    void Update()
    {
        // Looking
        _mouseMovVec += mouseLook.GetMouseDelta();
        _mouseMovVec = Vector2.Lerp(_mouseMovVec, Vector2.zero, Time.deltaTime * view_rotation_recover);

        // Swaying
        _swayVec.x = Mathf.Lerp(_swayVec.x, 0f, Time.deltaTime * walk_recover);
        _swayVec.y = Mathf.Lerp(_swayVec.y, 0f, Time.deltaTime * walk_recover);
        _swayVec.z = Mathf.Lerp(_swayVec.z, 0f, Time.deltaTime * walk_recover);

        float breathIntensity = 1f;
        _breath += breathIntensity * Time.deltaTime;
        float breathP = Mathf.Sin(2 * Mathf.PI / breath_period * _breath);
        Vector3 breathPos = Vector3.Lerp(Vector3.zero, breath_transform, breathP);

        // sway for jumping and flying
        Vector3 v = playerMovement.velocity;
        Sway(new Vector3(0, Mathf.Max(v.y, -1) * Time.deltaTime * 10, 0));
        Vector2 v_xz = new Vector2(v.x, v.z);
        _walkSine += Time.deltaTime * v_xz.magnitude / step_size;

        if (_walkSine > Mathf.PI * 2)
        {
            _walkSine -= Mathf.PI * 2;
        }

        var walkSineVal = Mathf.Sin(_walkSine);
        if ((walkSineVal < -0.95 || walkSineVal > 0.95) && !stepSound.isPlaying)
        {
            stepSound.Play();
        }

        if (v_xz.magnitude < 0.33)
            _walkSine = 0;
        float leftFoot = Mathf.Max(0, Mathf.Sin(_walkSine + Mathf.PI));
        float rightFoot = Mathf.Max(0, Mathf.Sin(_walkSine));
        float walkLr = Mathf.Sin(_walkSine);
        Vector3 hip = new Vector3(-1, -1, 0) * leftFoot + new Vector3(1, -1, 0) * rightFoot;
        Sway(hip * (Time.deltaTime * walk_sway));

        transform.localPosition = _swayVec + breathPos;
        transform.localRotation =
            quaternion.LookRotation(
                new Vector3(walkLr * 0.01f - _mouseMovVec.x * view_rotation_intensity.x,
                    -_mouseMovVec.y * view_rotation_intensity.y, 1f),
                Vector3.up);
    }

    public void Sway(Vector3 swayAmount)
    {
        _swayVec.x -= swayAmount.x * _swayMultiplier;
        _swayVec.y += swayAmount.y * _swayMultiplier;
        _swayVec.z += swayAmount.z * _swayMultiplier;
    }
}