using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmplitudeRotate : MonoBehaviour
{
    public AudioPeer _audioPeer;
    public float _rotationSpeed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.up, (AudioPeer._amplitudeBuffer * Time.deltaTime * _rotationSpeed));
    }
}
