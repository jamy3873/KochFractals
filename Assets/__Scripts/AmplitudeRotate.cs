using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmplitudeRotate : MonoBehaviour
{
    public AudioPeer _audioPeer;
    public float _rotationSpeed;
    public int _direction;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.up, (_audioPeer._amplitudeBuffer * Time.deltaTime * _rotationSpeed * _direction));
    }
}
