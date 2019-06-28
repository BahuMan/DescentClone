using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FPSDescentControls : MonoBehaviour
{

    public Vector3 _maxSpeed; //max speed on 3 different axises, so not a single vector (directional) really
    public Vector3 _keyboardSensitivity;
    public Vector3 _mouseSensitivity;

    [Tooltip("0 means no drag, 1 means negate current angular velocity")]
    [Range(0, 5)]
    public float _rotationDrag;

    [Tooltip("0 means no drag, 1 means negate current angular velocity")]
    [Range(0, 5)]
    public float _inertiaDrag;

    private Rigidbody _rigid;
    private Vector3 _requestedRelativeThrust;
    private Vector3 _requestedRelativeTorque;

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponentInChildren<Rigidbody>();
    }

    private void Update()
    {
        float torquePitch = Input.GetAxis("Mouse Y");
        float torqueYaw = Input.GetAxis("Mouse X");
        float torqueRoll = Input.GetAxis("Roll");

        //rotation damping
        Vector3 curRotation = this.transform.InverseTransformDirection(_rigid.angularVelocity);
        if (Mathf.Abs(torquePitch) < 0.1) torquePitch = -curRotation.x * _rotationDrag; //break if no thrust applied
        if (Mathf.Abs(torqueYaw) < 0.1) torqueYaw = -curRotation.y * _rotationDrag; //break if no thrust applied
        if (Mathf.Abs(torqueRoll) < 0.1) torqueRoll = -curRotation.z * _rotationDrag; //break if no thrust applied

        float thrustRight = Input.GetAxis("Strafe") * _keyboardSensitivity.x;
        float thrustUp = Input.GetAxis("StrafeUp") * _keyboardSensitivity.y;
        float thrustForward = Input.GetAxis("Forward") * _keyboardSensitivity.z;

        //dampen linear motion
        Vector3 relVel = transform.InverseTransformDirection(_rigid.velocity);
        if (Mathf.Abs(thrustRight) < 0.1) thrustRight = -relVel.x * _inertiaDrag;
        if (Mathf.Abs(thrustUp) < 0.1) thrustUp = -relVel.y * _inertiaDrag;
        if (Mathf.Abs(thrustForward) < 0.1) thrustForward = -relVel.z * _inertiaDrag;

        //these 2 variables will be used in fixedUpdate:
        _requestedRelativeTorque = new Vector3(torquePitch, torqueYaw, torqueRoll);
        _requestedRelativeThrust = new Vector3(thrustRight, thrustUp, thrustForward);

    }

    private void FixedUpdate()
    {
        _rigid.AddRelativeTorque(_requestedRelativeTorque);
        _rigid.AddRelativeForce(_requestedRelativeThrust);

        LimitSpeed();
    }

    private void LimitSpeed()
    {
        Vector3 vel = _rigid.velocity;
        float relativeRight = Vector3.Dot(vel, transform.right);
        relativeRight = Mathf.Sign(relativeRight) * Mathf.Min(Mathf.Abs(relativeRight), _maxSpeed.x);
        float relativeUp = Vector3.Dot(vel, transform.up);
        relativeUp = Mathf.Sign(relativeUp) * Mathf.Min(Mathf.Abs(relativeUp), _maxSpeed.y);
        float relativeForward = Vector3.Dot(vel, transform.forward);
        relativeForward = Mathf.Sign(relativeForward) * Mathf.Min(Mathf.Abs(relativeForward), _maxSpeed.z);

        vel = new Vector3(relativeRight, relativeUp, relativeForward);
        DebugText.ShowVector("relative velocity", vel);
        vel = transform.TransformVector(vel);

        _rigid.velocity = vel;
    }
}
