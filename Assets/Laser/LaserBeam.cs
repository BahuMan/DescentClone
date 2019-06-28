using UnityEngine;

public class LaserBeam : MonoBehaviour
{

    public float speed;
    public float growTime; //since a laser shoots out of a gun and into an object, the z-axis needs to be stretched and squashed (unlike solid objects like rockts and bullets)
    public float maxLifeTime;
    public GameObject particlePrefab;

    public Transform _model; //gets squashed in the z-axis
    public Rigidbody _rigidBody;

    private bool dying = false;
    private float _startLifeTime;
    private float _startDieTime;


    // Use this for initialization
    void Start()
    {
        _startLifeTime = Time.time;
        _rigidBody.velocity = transform.forward * speed;
        _model.localScale = new Vector3(1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (dying)
        {
            float dieTime = Time.time - _startDieTime;
            if (dieTime < growTime)
            {
                _model.localScale = new Vector3(1f, 1f, 1f - dieTime / growTime);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            float lived = Time.time - _startLifeTime;
            if (lived < growTime) {
                _model.localScale = new Vector3(1f, 1f, lived / growTime);
            }
            else if (lived < maxLifeTime)
            {
                _model.localScale = Vector3.one;
            }
            else
            {
                dying = true;
                _startDieTime = Time.time;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Laser hit " + collision.gameObject.name);
        if (!dying)
        {
            dying = true;
            _startDieTime = Time.time;
            Destroy(GameObject.Instantiate(particlePrefab, transform.position, Quaternion.identity), 4);
        }
    }
}
