using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ClangSounds : MonoBehaviour
{
    public AudioClip[] clangs;
    private AudioSource[] _src;

    private void Start()
    {
        _src = new AudioSource[clangs.Length];
        for (int i=0; i<clangs.Length; ++i)
        {
            _src[i] = this.gameObject.AddComponent<AudioSource>();
            _src[i].playOnAwake = false;
            _src[i].loop = false;
            _src[i].clip = clangs[i];
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        _src[Random.Range(0, _src.Length)].Play();
    }
}
