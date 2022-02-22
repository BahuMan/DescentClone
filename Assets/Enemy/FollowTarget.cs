using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    //set this for the enemy to try and get closer to
    public Transform target;

    public Vector3 _maxSpeed; //max speed on 3 different axises, so not a single vector (directional) really
    [Tooltip("0 means no drag, 1 means negate current angular velocity")]
    [Range(0, 5)]
    public float _rotationDrag;

    [Tooltip("0 means no drag, 1 means negate current angular velocity")]
    [Range(0, 5)]
    public float _inertiaDrag;

    [Tooltip("power of the thrusters in 3 directions")]
    public Vector3 power;

    private Rigidbody _rigid;

    public Vector3 requestedRelativeThrust { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponentInChildren<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        float thrustRight = 0f;
        float thrustUp = 0f;
        float thrustForward = 0f;

        Vector3 localCoordinates = transform.InverseTransformPoint(target.position);
        if (localCoordinates.z > 20f) thrustForward = 1f;
        else if (localCoordinates.z < 0) thrustForward = -1f;

        thrustUp = Mathf.Sign(localCoordinates.y);
        thrustRight = Mathf.Sign(localCoordinates.x);
        requestedRelativeThrust = new Vector3(thrustRight, thrustUp, thrustForward);
    }

    private void FixedUpdate()
    {
        _rigid.AddRelativeForce(requestedRelativeThrust);

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
        //DebugText.ShowVector("relative velocity", vel);
        vel = transform.TransformVector(vel);

        _rigid.velocity = vel;
    }
}
