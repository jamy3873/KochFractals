using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochTrail : KochGenerator
{

    public class TrailObject
    {
        public GameObject go { get; set; }
        public TrailRenderer trail { get; set; }
        public int currentTargetNum { get; set; }
        public Vector3 targetPosition { get; set; }
        public Color emissionColor { get; set; }
    }

    [HideInInspector]
    public List<TrailObject> _trail;

    [Header("Trail Properties")]
    public GameObject _trailPrefab;
    public AnimationCurve _trailWidthCurve;
    [Range(0,8)]
    public int _trailEndCapVertices;
    public Material _trailMaterial;
    public Gradient _trailColor;

    [Header("Audio")]
    public AudioPeer _audioPeer;
    public int[] _audioBand;
    public Vector2 _speedMinMax, _widthMinMax, _trailTimeMinMax;
    public float _colorMultiplier;

    //Private Vars
    private float _lerpPosSpeed;
    private float _distanceSnap;
    private Color _startColor, _endColor;

    void Start()
    {
        _startColor = new Color(0, 0, 0, 0);
        _endColor = new Color(0, 0, 0, 1);

        _trail = new List<TrailObject>();
        for (int i = 0; i < _initiatorPointAmount; i++)
        {
            GameObject trailInstance = Instantiate(_trailPrefab, transform.position, Quaternion.identity, this.transform);
            TrailObject trailObjectInstance = new TrailObject();
            trailObjectInstance.go = trailInstance;
            trailObjectInstance.trail = trailInstance.GetComponent<TrailRenderer>();
            trailObjectInstance.trail.material = new Material(_trailMaterial);
            trailObjectInstance.emissionColor = _trailColor.Evaluate(i * (1.0f / _initiatorPointAmount));
            trailObjectInstance.trail.numCapVertices = _trailEndCapVertices;
            trailObjectInstance.trail.widthCurve = _trailWidthCurve;

            Vector3 instantiatePosition;
            if(_generationCount > 0)
            {
                int step;
                if (_useBezierCurves)
                {
                    step = _bezierPosition.Length / _initiatorPointAmount;
                    instantiatePosition = _bezierPosition[i * step];
                    trailObjectInstance.currentTargetNum = (i * step) + 1;
                    trailObjectInstance.targetPosition = _bezierPosition[trailObjectInstance.currentTargetNum];
                }
                else
                {
                    step = _position.Length / _initiatorPointAmount;
                    instantiatePosition = _position[i * step];
                    trailObjectInstance.currentTargetNum = (i * step) + 1;
                    trailObjectInstance.targetPosition = _position[trailObjectInstance.currentTargetNum];
                }
            }
            else
            {
                instantiatePosition = _position[i];
                trailObjectInstance.currentTargetNum = i + 1;
                trailObjectInstance.targetPosition = _position[trailObjectInstance.currentTargetNum];
            }

            trailObjectInstance.go.transform.localPosition = instantiatePosition;
            _trail.Add(trailObjectInstance);

        }
    }

    void Movement()
    {
        _lerpPosSpeed = Mathf.Lerp(_speedMinMax.x, _speedMinMax.y, _audioPeer._amplitude);
        for (int i = 0; i < _trail.Count; i++)
        {
            _distanceSnap = Vector3.Distance(_trail[i].go.transform.localPosition, _trail[i].targetPosition);
            if (_distanceSnap < 0.05)
            {
                _trail[i].go.transform.localPosition = _trail[i].targetPosition;
                if (_useBezierCurves && _generationCount > 0)
                {
                    if (_trail[i].currentTargetNum < _bezierPosition.Length - 1)
                    {
                        _trail[i].currentTargetNum += 1;
                    }
                    else
                    {
                        _trail[i].currentTargetNum = 1;
                    }
                    _trail[i].targetPosition = _bezierPosition[_trail[i].currentTargetNum];
                }
                else
                {
                    if(_trail[i].currentTargetNum < _position.Length - 1)
                    {
                        _trail[i].currentTargetNum += 1;
                    }
                    else
                    {
                        _trail[i].currentTargetNum = 1;
                    }
                    _trail[i].targetPosition = _targetPosition[_trail[i].currentTargetNum];
                }
            }

            if (_trail[i].go != null)
            {
                _trail[i].go.transform.localPosition = Vector3.MoveTowards(_trail[i].go.transform.localPosition, _trail[i].targetPosition, Time.deltaTime * _lerpPosSpeed);
            }
        }
    }

    void AudioBehavior()
    {
        for (int i = 0; i < _initiatorPointAmount; i++)
        {
            Color colorLerp = Color.Lerp(_startColor, _trail[i].emissionColor * _colorMultiplier, _audioPeer._audioBand[_audioBand[i]]);
            _trail[i].trail.material.SetColor("_EmissionColor", colorLerp);
            colorLerp = Color.Lerp(_startColor, _endColor, _audioPeer._audioBand[_audioBand[i]]);
            _trail[i].trail.material.SetColor("Color", colorLerp);

            float widthLerp = Mathf.Lerp(_widthMinMax.x, _widthMinMax.y, _audioPeer._audioBandBuffer[_audioBand[i]]);
            _trail[i].trail.widthMultiplier = widthLerp;

            float timeLerp = Mathf.Lerp(_trailTimeMinMax.x, _trailTimeMinMax.y, _audioPeer._audioBandBuffer[_audioBand[i]]);
            _trail[i].trail.time = timeLerp;
        }
    }

    void Update()
    {
        Movement();
        AudioBehavior();
    }
}
