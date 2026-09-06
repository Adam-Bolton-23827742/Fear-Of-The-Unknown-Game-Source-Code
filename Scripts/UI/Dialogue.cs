using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    private TextMeshProUGUI TextMeshProUGUI;
    [HideInInspector] public bool IsRunning = false, Set = false;
    [SerializeField] private ButtonController UseButton;
    [SerializeField] private Enemies Boss;
    [SerializeField] private Image ControlPrompt;
    [SerializeField] private Sprite SkipC, SkipKM;
    [SerializeField] private GameObject[] Menus;
    [SerializeField] private CinemachineCamera StartCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCamera.Prioritize();
        TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
        StartCoroutine(SetDialogue("I have been tasked to investigate screams coming from inside of this mansion. Maybe I can find those recently missing people. I should go pick up my gun."));
    }

    private void Update()
    {
        if (Set)
        {
            Set = false;
            StartCoroutine(SetDialogue(UseButton.DialogueText));
        }
    }

    public IEnumerator SetDialogue(string Text)
    {
        // Pause the game and set the dialogue text
        IsRunning = true;

        foreach (GameObject Menu in Menus)
        {
            if (Menu.activeSelf)
            {
                yield break;
            }
        }

        TextMeshProUGUI.text = Text;

        if (XInputController.current != null)
        {
            ControlPrompt.sprite = SkipC;
        }

        else
        {
            ControlPrompt.sprite = SkipKM;
        }

        ControlPrompt.transform.parent.gameObject.SetActive(true);

        if (Boss.CurrentHealth > 0)
        {
            Time.timeScale = 0.0f;
        }

        // Wait for all past inputs to be gone
        yield return new WaitForSecondsRealtime(0.5f);

        // Wait for the player to press left mouse or A
        yield return new WaitUntil(() => Input.GetButtonDown("Shoot"));

        // remove the text and unpause time
        TextMeshProUGUI.text = null;
        ControlPrompt.transform.parent.gameObject.SetActive(false);
        Time.timeScale = 1.0f;

        if (Boss.CurrentHealth <= 0)
        {
            Boss.gameObject.GetComponent<EndGame>().LoadCredits = true;
        }

        IsRunning = false;
    }
}
