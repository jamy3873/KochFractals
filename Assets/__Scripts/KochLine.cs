using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class KochLine : KochGenerator
{
    LineRenderer _lineRenderer;
    /*[Range(0,1)]
    public float _lerpAmount;*/
    Vector3[] _lerpPosition;
    public float _generatorMultiplier;
    private float[] _lerpAudio;

    [Header("AudioPeer")]
    public AudioPeer _audioPeer;
    public int[] _audioBand;
    public bool _useBuffer;
    public Material _material;
    public Color _color;
    public Gradient _colorGrad;
    private Material _matInstance;
    public int _audioBandMaterial;
    public float _emissionMultiplier;

    void Start()
    {
        _lerpAudio = new float[_initiatorPointAmount];
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = true;
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.loop = true;
        _lineRenderer.positionCount = _position.Length;
        _lineRenderer.SetPositions(_position);
        _lerpPosition = new Vector3[_position.Length];
        //Apply Material
        _matInstance = new Material(_material);
        if (_matInstance)
        {
            _lineRenderer.material = _matInstance;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_matInstance)
        {
            _color = _colorGrad.Evaluate(AudioPeer._audioBandBuffer[_audioBand[0]]);
            _matInstance.SetColor("_EmissionColor", _color * AudioPeer._amplitudeBuffer * _emissionMultiplier);
        }
        

        if (_generationCount != 0)
        {
            int count = 0;
            for (int i = 0; i < _initiatorPointAmount; i++)
            {
                if (_useBuffer)
                {
                    _lerpAudio[i] = AudioPeer._audioBandBuffer[_audioBand[i]];
                }
                else
                {
                    _lerpAudio[i] = AudioPeer._audioBand[_audioBand[i]];
                }
                for (int j = 0; j < (_position.Length - 1) / _initiatorPointAmount; j++)
                {
                    _lerpPosition[count] = Vector3.Lerp(_position[count], _targetPosition[count], _lerpAudio[i]);
                    count++;
                }
            }
            _lerpPosition[count] = Vector3.Lerp(_position[count], _targetPosition[count], _lerpAudio[_initiatorPointAmount - 1]);

            if (_useBezierCurves)
            {
                _bezierPosition = BezierCurve(_lerpPosition, _bezierVertexCount);
                _lineRenderer.positionCount = _bezierPosition.Length;
                _lineRenderer.SetPositions(_bezierPosition);
            }
            else
            {
                _lineRenderer.positionCount = _lerpPosition.Length;
                _lineRenderer.SetPositions(_lerpPosition);
            }
            
        }

        /*if (Input.GetKeyUp(KeyCode.O))
        {
            KochGenerate(_targetPosition, true, _generatorMultiplier);
            _lerpPosition = new Vector3[_position.Length];
            _lineRenderer.positionCount = _position.Length;
            _lineRenderer.SetPositions(_position);
            _lerpAmount = 0;
        }
        else if (Input.GetKeyUp(KeyCode.I))
        {
            KochGenerate(_targetPosition, false, _generatorMultiplier);
            _lerpPosition = new Vector3[_position.Length];
            _lineRenderer.positionCount = _position.Length;
            _lineRenderer.SetPositions(_position);
            _lerpAmount = 0;
        }*/
    }
}
