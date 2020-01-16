using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmplitudeFlash : MonoBehaviour
{
    public AudioPeer _audioPeer;
    public Gradient _colorGrad;
    public float _colorMultiplier;
    public Vector3 _targetScale;

    private Color _startColor, _endColor;
    private Color _emissionColor;
    private Vector3 _scale;
    private Renderer rend;
    void Start()
    {
        _startColor = new Color(0, 0, 0, 0);
        _endColor = new Color(0, 0, 0, 1);
        _scale = transform.localScale;
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _emissionColor = _colorGrad.Evaluate(_audioPeer._amplitude);

        Color colorLerp = Color.Lerp(_startColor, _emissionColor * _colorMultiplier, _audioPeer._amplitudeBuffer);
        rend.material.SetColor("_EmissionColor", colorLerp);
        colorLerp = Color.Lerp(_startColor, _endColor, _audioPeer._amplitudeBuffer);
        rend.material.SetColor("_Color", colorLerp);

        transform.localScale = Vector3.Lerp(_scale, _targetScale, _audioPeer._amplitudeBuffer);
    }
}
