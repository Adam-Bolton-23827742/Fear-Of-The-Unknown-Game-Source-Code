using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private Image Prompt;
    [SerializeField] private Sprite PromptC, PromptKM;
    [SerializeField] private Transform TargetPosition;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Speed * Time.deltaTime);

        if (XInputController.current != null)
        {
            Prompt.sprite = PromptC;
        }

        else
        {
            Prompt.sprite = PromptKM;
        }

        float Distance = TargetPosition.position.y - transform.position.y;

        if (Input.GetKeyDown(KeyCode.Escape) || XInputController.current != null && XInputController.current.bButton.isPressed || Distance <= 0.1f)
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
