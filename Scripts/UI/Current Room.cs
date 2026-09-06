using System.Collections;
using UnityEngine;

public class CurrentRoom : MonoBehaviour
{
    public GameObject CurrentRoomImage;
    [HideInInspector] public GameObject PreviousRoomImage;
    [SerializeField] private float FlashTimer;
    private bool IsRunning = false;

    private void Update()
    {
        if (!IsRunning)
        {
            StartCoroutine(Flash());
        }

        if (PreviousRoomImage != null && CurrentRoomImage != PreviousRoomImage && !PreviousRoomImage.activeSelf)
        {
            PreviousRoomImage.SetActive(true);
        }
    }

    private IEnumerator Flash()
    {
        IsRunning = true;

        yield return new WaitForSecondsRealtime(FlashTimer);

        if (CurrentRoomImage.activeSelf)
        {
            CurrentRoomImage.SetActive(false);
        }

        else
        {
            CurrentRoomImage.SetActive(true);
        }

        IsRunning = false;
    }
}
