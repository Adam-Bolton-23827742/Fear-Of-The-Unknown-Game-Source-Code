using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Passcode : MonoBehaviour
{
    [SerializeField] private char Number;
    [SerializeField] private TMP_InputField InputField;
    [SerializeField] private string Password;
    [SerializeField] private float Timer;
    [SerializeField] private GameObject KeycodePanel;
    [SerializeField] private Doors KeycodeDoor;
    [SerializeField] private BoxCollider KeycodeTrigger;
    [SerializeField] private PasscodeSystem PasscodeSystem;

    public void OnPressed()
    {
        // The passcode only needs 4 characters
        if (InputField.text.Length < 4)
        {
            // Add the number clicked into the passcode
            InputField.text += Number.ToString();
        }
    }

    public void Backspace()
    {
        // Only remove the last character if it is greater than zero to prevent index error
        if (InputField.text.Length > 0)
        {
            // Remove the last character of the input fields text
            InputField.text = InputField.text.Remove(InputField.text.Length - 1);
        }
    }

    public void OnEnter()
    {
        StartCoroutine(Enter());
    }

    private IEnumerator Enter()
    {
        if (InputField.text == Password)
        {
            InputField.text = "Password correct";
            yield return new WaitForSecondsRealtime(Timer);
            KeycodeTrigger.enabled = false;
            PasscodeSystem.IsColliding = false;
            KeycodePanel.SetActive(false);
            KeycodeDoor.IsLocked = false;
            Time.timeScale = 1.0f;
        }

        else if (InputField.text == "")
        {
            InputField.text = "Please enter a password";
            yield return new WaitForSecondsRealtime(Timer);
            InputField.text = null;
        }

        else
        {
            InputField.text = "Password incorrect";
            yield return new WaitForSecondsRealtime(Timer);
            InputField.text = null;
        }
    }

    public void CloseWindow()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (InputField.text == "Password correct")
        {
            KeycodeTrigger.enabled = false;
            PasscodeSystem.IsColliding = false;
            KeycodeDoor.IsLocked = false;
        }

        KeycodePanel.SetActive(false);
        InputField.text = null;
        Time.timeScale = 1.0f;
    }
}