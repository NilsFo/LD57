using UnityEngine;

public class ViewmodelSway : MonoBehaviour
{
    public CharacterMovement playerMovement;
    public MouseLook mouseLook;
    private float _swayMultiplier = 0.0005f;
    
    void Start()
    {
        if(playerMovement == null)
            playerMovement = FindFirstObjectByType<CharacterMovement>();
        if (mouseLook == null)
            mouseLook = FindFirstObjectByType<MouseLook>();
    }

    private Vector3 _sway_vec;
    private float walkSine = 0f;
    public float step_size = 0.8f;
    public float walk_sway = 85f;
    public float walk_recover = 5f;
    public float view_rotation = 5f;


    public float breath_period = 3f;
    public Vector3 breath_transform = Vector3.zero;

    private float _breath = 0f;
    // Update is called once per frame
    void Update()
    {
        // Looking
        //var mouseMov = mouseLook.SmoothMouse;
        // TODO Move by rotation
        
        // Swaying
        _sway_vec.x = Mathf.Lerp(_sway_vec.x, 0f, Time.deltaTime*walk_recover);
        _sway_vec.y = Mathf.Lerp(_sway_vec.y, 0f, Time.deltaTime*walk_recover);
        _sway_vec.z = Mathf.Lerp(_sway_vec.z, 0f, Time.deltaTime*walk_recover);

        var breath_intensity = 1f;
        _breath += breath_intensity * Time.deltaTime;
        var breath_p = Mathf.Sin(2*Mathf.PI/breath_period * _breath);
        var _breath_pos = Vector3.Lerp(Vector3.zero, breath_transform, breath_p);
            
        // sway for jumping and flying
        var v = playerMovement.velocity;
        Sway(new Vector3(0, v.y * Time.deltaTime * 10, 0));
        var v_xz = new Vector2(v.x, v.z);
        walkSine += Time.deltaTime * v_xz.magnitude / step_size;
        if (walkSine > Mathf.PI * 2)
            walkSine -= Mathf.PI * 2;
        if(v_xz.magnitude < 0.01)
            walkSine = 0;
        var left_foot = Mathf.Max(0, Mathf.Sin(walkSine + Mathf.PI));
        var right_foot = Mathf.Max(0, Mathf.Sin(walkSine));
        var hip = new Vector3(-1, -1, 0) * left_foot + new Vector3(1, -1, 0) * right_foot;
        Sway(hip * (Time.deltaTime * walk_sway));

        transform.localPosition = _sway_vec + _breath_pos;
    }

    public void Sway(Vector3 swayAmount)
    {
        _sway_vec.x -= swayAmount.x * _swayMultiplier;
        _sway_vec.y += swayAmount.y * _swayMultiplier;
        _sway_vec.z += swayAmount.z * _swayMultiplier;
    }
}
