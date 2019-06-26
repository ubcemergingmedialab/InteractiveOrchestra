using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// retrieved from https://docs.unity3d.com/2018.1/Documentation/ScriptReference/AudioSettings.html
[RequireComponent(typeof(AudioSource))]

public class InHouseMetronome : MonoBehaviour {

    public double bpm = 140.0F;
    public float gain = 0.5F;
    public int signatureHi = 4;
    public int signatureLo = 4;
    private double nextTick = 0.0F;
    private float amp = 0.0F;
    private float phase = 0.0F;
    private double sampleRate = 0.0F;
    private int accent;
    private bool running = false;

    private AudioSource metronome;
    private bool playing = false;

    // Use this for initialization
    void Start () {
        accent = signatureHi;
        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;
        nextTick = startTick * sampleRate;
        running = true;
        Debug.Log(AudioSettings.GetConfiguration());

        metronome = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (playing)
        {
            metronome.Play();
        }
        else
        {
            metronome.Stop();
        }
	}

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running)
            return;

        double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        double sample = AudioSettings.dspTime * sampleRate;
        int dataLen = data.Length / channels;
        int n = 0;
        while (n < dataLen)
        {
            float x = gain * amp * Mathf.Sin(phase);
            int i = 0;
            while (i < channels)
            {
                data[n * channels + i] += x;
                i++;
            }
            while (sample + n >= nextTick)
            {
                nextTick += samplesPerTick;
                amp = 1.0F;
                if (++accent > signatureHi)
                {
                    accent = 1;
                    amp *= 2.0F;
                }
                //Debug.Log("Tick: " + accent + "/" + signatureHi);
            }
            phase += amp * 0.3F;
            amp *= 0.993F;
            n++;
        }
    }

    public void TogglePlay()
    {
        playing = !playing;
    }

    public void SetNewBPM(double newBPM)
    {
        bpm = newBPM;
    }
}

