using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SteamPump : MonoBehaviour
{
    private CinemachineCamera SteamCamera;
    [SerializeField] private CinemachineCamera CurrentCamera;
    [SerializeField] private float Timer;
    private ToggleInventoryMenu ToggleInventoryMenu;
    private ObjectInteraction PumpInteraction;
    [HideInInspector] public bool TurnValve = false;
    [SerializeField] private ParticleSystem Steam;
    [SerializeField] private PlayerMovement PlayerMovement;
    private AudioSource AudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ToggleInventoryMenu = GameObject.Find("Player").GetComponent<ToggleInventoryMenu>();
        PumpInteraction = GetComponent<ObjectInteraction>();
        SteamCamera = Steam.GetComponentInChildren<CinemachineCamera>();
        AudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (TurnValve)
        {
            StartCoroutine(ToggleSteam());
        }
    }

    private IEnumerator ToggleSteam()
    {
        TurnValve = false;

        // Unlock the foor if the player is near the fireplace and uses the lighter
        if (PumpInteraction.Colliding)
        {
            ToggleInventoryMenu.LoadInventory(false);
            Time.timeScale = 0.0f;
            PlayerMovement.enabled = false;
            AudioSource.Play();
            yield return new WaitForSecondsRealtime(Timer);
            SteamCamera.Prioritize();
            yield return new WaitForSecondsRealtime(Timer);
            Steam.Stop();
            Steam.GetComponent<BoxCollider>().enabled = false;
            yield return new WaitUntil(() => Steam.isStopped);
            Steam.GetComponent<AudioSource>().Stop();
            CurrentCamera.Prioritize();
            PlayerMovement.enabled = true;
            Time.timeScale = 1.0f;
        }

        else
        {
            print("Cannot use at this time");
        }
    }
}