using System.Collections;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] private float MusicDelay;
    private AudioSource AudioSource;
    private bool IsPlaying, PlayBossMusic;
    [SerializeField] private GameObject BossHealth;
    [SerializeField] private AudioClip BossMusic;

    private void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!BossHealth.activeSelf)
        {
            if (!IsPlaying)
            {
                StartCoroutine(PlayMusic());
            }
        }

        else
        {
            if (!PlayBossMusic)
            {
                PlayBossMusic = true;
                StopCoroutine(PlayMusic());
                AudioSource.clip = BossMusic;
                AudioSource.loop = true;
                AudioSource.Play();
            }
        }
    }

    private IEnumerator PlayMusic()
    {
        IsPlaying = true;
        yield return new WaitForSecondsRealtime(MusicDelay);
        AudioSource.Play();
        yield return new WaitUntil(() => !AudioSource.isPlaying);
        IsPlaying = false;
    }
}
