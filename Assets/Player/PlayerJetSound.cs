using UnityEngine;
using System.Collections;

public class PlayerJetSound : MonoBehaviour
{
    public FPSDescentControls _player;
    public AudioSource _jet;
    [Range(0, 1)]
    public float maxVolume = 1f;
    private float velocity = 0;
    

    private void Start()
    {
        _jet.volume = 0;
    }
    private void Update()
    {
        float vol = Mathf.Max(_player.requestedRelativeThrust.magnitude, _player.requestedRelativeTorque.magnitude);
        _jet.volume = Mathf.SmoothDamp(_jet.volume, Mathf.Clamp01(vol), ref velocity, .2f) * maxVolume;
    }
}
