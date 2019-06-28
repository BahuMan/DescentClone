using UnityEngine;
using System.Collections;

public class PlayerJetSound : MonoBehaviour
{
    public AudioSource _jet;
    private float velocity = 0;

    private void Update()
    {
        float vol = Mathf.Max(Mathf.Abs(Input.GetAxis("Mouse X")),
            Mathf.Max(Mathf.Abs(Input.GetAxis("Mouse Y")),
            Mathf.Max(Mathf.Abs(Input.GetAxis("Roll")),
            Mathf.Max(Mathf.Abs(Input.GetAxis("Forward")),
            Mathf.Abs(Input.GetAxis("Strafe")),
            Mathf.Abs(Input.GetAxis("StrafeUp"))
            ))));
        _jet.volume = Mathf.SmoothDamp(_jet.volume, Mathf.Clamp01(vol), ref velocity, .2f);
    }
}
