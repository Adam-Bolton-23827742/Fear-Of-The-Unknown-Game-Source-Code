using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PasscodeSystem : MonoBehaviour
{
    private bool IsRunning = false;
    [HideInInspector] public bool IsColliding = false;
    [SerializeField] private GameObject KeycodePanel;
    [SerializeField] private GameObject Key;

    private void Update()
    {
        if (!IsRunning && IsColliding && Time.timeScale == 1.0f)
        {
            StartCoroutine(OpenWindow());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            IsColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            IsColliding = false;
        }
    }

    private IEnumerator OpenWindow()
    {
        IsRunning = true;

        yield return new WaitUntil(() => !Input.GetButtonDown("Shoot"));

        // Open the passcode window when interacted with
        if (Input.GetButtonDown("Shoot"))
        {
            EventSystem.current.SetSelectedGameObject(Key);
            KeycodePanel.SetActive(true);
            Time.timeScale = 0.0f;
        }

        IsRunning = false;
    }
}
