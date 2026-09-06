using UnityEngine;

public class Ambient : MonoBehaviour
{
    private AudioSource AudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AudioSource.volume > 0.3f)
        {
            AudioSource.volume = 0.3f;
        }
    }
}
