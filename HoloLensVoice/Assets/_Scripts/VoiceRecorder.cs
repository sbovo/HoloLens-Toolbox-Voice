using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class VoiceRecorder : MonoBehaviour {


    private AudioSource audioSource;

    // Use this for initialization
    async void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start("Built-in Microphone", true, 10, 16000);
        audioSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        audioSource.Play();
        InvokeRepeating("CheckSilence", 2.0f, 1f);
    }

    int silenceCount = 0;
    float[] spectrum = new float[256];

    void CheckSilence()
    {
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        float currentAverageVolume = spectrum.Average();


        if (currentAverageVolume < 0.00001f)
        {
            silenceCount++;
        }
        else
        {
            silenceCount = 0;
        }

        if (silenceCount > 3)
        {
            Debug.Log("Silence");
            MicrophoneHelper.Save(@"c:\_temp\AudioCapture", audioSource.clip);
            silenceCount = 0;
        }
    }

   


    // Update is called once per frame
    void Update ()
    {
        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        }
    }
}
