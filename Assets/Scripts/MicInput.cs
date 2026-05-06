using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicInput : MonoBehaviour
{
    public AudioSource audioSource;
    public string micDevice;
    public int sampleRate = 44100;
    public int sampleWindow = 128;

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            audioSource = GetComponent<AudioSource>();

            audioSource.clip = Microphone.Start(micDevice, true, 5, sampleRate);
            audioSource.loop = true;

            // Esperar a que el micrófono empiece
            while (!(Microphone.GetPosition(micDevice) > 0)) { }

            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No microphone detected!");
        }
    }

    public float GetMicLevel()
    {
        if (audioSource == null || audioSource.clip == null)
            return 0f;

        int pos = Microphone.GetPosition(micDevice) - sampleWindow;
        if (pos < 0) return 0f;

        float[] samples = new float[sampleWindow];
        audioSource.clip.GetData(samples, pos);

        float max = 0f;
        foreach (float s in samples)
            max = Mathf.Max(max, Mathf.Abs(s));

        return max;
    }
}
