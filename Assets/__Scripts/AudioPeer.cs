using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource _audioSource;
    public static float[] _samples = new float[512];
    float[] _freqBands = new float[8];
    float[] _bandBuffer = new float[8];
    float[] _bufferDecrease = new float[8];

    float[] _freqBandHighest = new float[8];
    public float[] _audioBand = new float[8];
    public float[] _audioBandBuffer = new float[8];

    public float _amplitude, _amplitudeBuffer;
    float _amplitudeHighest = 0;
    public float _audioProfile;

    public AmplitudeRotate rotator;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioProfile(_audioProfile);
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource(); //Step 1: Get audio spectrum sample using FFTWindow
        MakeFrequencyBands(); //Step 2: Parse samples into 8 different frequency averages
        BandBuffer(); //Step 3: Create "buffer" bands which gradually drop while the frequency band is below it
        CreateAudioBands(); //Step 4: Create the audio bands which store a value between 0 and 1 based on the freq band's high value
        GetAmplitude(); //Step 5: Get the amplitude modifier by dividing the current amplitude (sum of all the audio bands) by the highest recorded amplitude
    }

    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < 8; i++)
        {
            _freqBandHighest[i] = audioProfile;
        }
    }

    void GetAmplitude()
    {
        float currentAmplitude = 0;
        float currentAmplitudeBuff = 0;
        for (int i = 0; i < 8; i++)
        {
            currentAmplitude += _audioBand[i];
            currentAmplitudeBuff += _audioBandBuffer[i];
        }
        if (currentAmplitude > _amplitudeHighest)
        {
            _amplitudeHighest = currentAmplitude;
            //rotator._direction *= -1;
        }
        _amplitude = currentAmplitude / _amplitudeHighest;
        _amplitudeBuffer = currentAmplitude / _amplitudeHighest;
    }

    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBands[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBands[i];
            }
            _audioBand[i] = (_freqBands[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    void BandBuffer()
    {
        for (int k = 0; k < 8; k++)
        {
            if (_freqBands[k] > _bandBuffer[k])
            {
                _bandBuffer[k] = _freqBands[k];
                _bufferDecrease[k] = .005f;
            }
            if (_freqBands[k] < _bandBuffer[k])
            {
                _bandBuffer[k] -= _bufferDecrease[k];
                _bufferDecrease[k] *= 1.2f;
            }
        }
    }

    void GetSpectrumAudioSource() //Where the magic happens
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        /*
         * 22050 Hz / 512 samples = 43 Hz per sample
         * 
         * 0: 2 samples -> 86 Hz (0-86)
         * 1: 4 samples -> 172 Hz (87-258)
         * 2: 8 samples -> 344 Hz (259-602)
         * 3: 16 samples -> 688 Hz (603-1290)
         * 4: 32 samples -> 1376 Hz (1291-2666)
         * 5: 64 samples -> 2752 Hz (2667-5418)
         * 6: 128 samples -> 55504 Hz (5419-10922)
         * 7: 256 samples -> 11008 Hz (10923-21930)
         * Total: 510
         */

        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i + 1);
            if (i == 7) //For last band, add 2 to cover all 512 samples
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }
            average /= count;
            _freqBands[i] = average * 10;

        }

    }
}
