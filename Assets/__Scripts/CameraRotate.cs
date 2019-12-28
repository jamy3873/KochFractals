using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float _speed;
    public Vector3 _axis;

    [Header("AudioPeer")]
    public AudioPeer _audioPeer;
    public int _audioBand;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_axis, (_speed * Time.deltaTime % 360));
    }
}
