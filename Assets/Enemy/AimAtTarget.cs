using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class AimAtTarget : MonoBehaviour
{
    public Transform _target;
    [Tooltip("degrees per second that this enemy can rotate towards target")]
    public float _agility;

    private Rigidbody _rigid;
    private LaserGun _gun;
    private LaserBeam _projectile;
    private float _laserSpeed;
    private Vector3 _requiredTorque = Vector3.zero;

    private void Start()
    {
        _gun = GetComponentInChildren<LaserGun>();
        _projectile = _gun.LaserBeamPrefab;
        _laserSpeed = _projectile.speed;
        _rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 targetVelocity = _target.GetComponent<Rigidbody>().velocity;
        Vector3 Aimpoint = FirstOrderIntercept(transform.position, Vector3.zero, _laserSpeed, _target.position, targetVelocity);
        DebugText.ShowVector("aimpoint ", Aimpoint);
        //for "up" vector, I'm using where the target is aiming for. This assumes that, like fighter planes in atmosphere,
        //this enemy can make the tightest turns by changing pitch
        Quaternion lookat = Quaternion.LookRotation(Aimpoint - transform.position, (_target.position + 10 * targetVelocity) - transform.position);
        //tested OK, enemy aims correctly for all positions of target, but this is too quick
        //transform.rotation = lookat;

        Quaternion reqEuler;
        //rotatetowards generates a new absolute rotation, somewhere between from and to, with a max degrees rotation... so whats the difference with LERP ?
        reqEuler = Quaternion.RotateTowards(transform.rotation, lookat, _agility * Time.deltaTime);
        //tested OK: instead of turning immediately, the enemy slowly rotates towards desired aimpoint (but not using physics)
        transform.rotation = reqEuler;

        Debug.DrawLine(transform.position, Aimpoint, Color.blue);
        //tested NOK: FromToRotation creates a "relative" rotation. To rotate enemy correctly, apply transform.rotation = transform.rotation * reqEuler
        reqEuler = Quaternion.FromToRotation(transform.forward, Aimpoint-transform.position);
        //if we apply the reqEuler here, the enemy will always look exactly where it needs to (no delay)
        //transform.rotation = transform.rotation * reqEuler;

        //tested OK: now clamp the rotation speed:
        //reqEuler = Quaternion.RotateTowards(Quaternion.identity, reqEuler, _agility * Time.deltaTime);

        float reqx = reqEuler.x > 180 ? reqEuler.x - 360 : reqEuler.x;
        float reqy = reqEuler.y > 180 ? reqEuler.y - 360 : reqEuler.y;
        float reqz = reqEuler.z > 180 ? reqEuler.z - 360 : reqEuler.z;
        reqEuler = Quaternion.Euler(reqx, reqy, reqz);

        //DebugText.ShowVector("euler rotation ", reqEuler.eulerAngles);
        //transform.Rotate(reqx, reqy, 0);

        //_requiredTorque = reqEuler;
    }

    private void FixedUpdate()
    {
        //_rigid.angularVelocity = _requiredTorque;

    }

    //first-order intercept using absolute target position
    private static Vector3 FirstOrderIntercept
    (
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity
    )
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        float t = FirstOrderInterceptTime
        (
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );
        return targetPosition + t * (targetRelativeVelocity);
    }

    //first-order intercept using relative target position
    private static float FirstOrderInterceptTime
    (
        float shotSpeed,
        Vector3 targetRelativePosition,
        Vector3 targetRelativeVelocity
    )
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude /
            (
                2f * Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            }
            else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        }
        else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
    }

}
