using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class ControlPrompts : MonoBehaviour
{
    public void SetPrompt(Image PromptImage1, Image PromptImage2, Image PromptImage3, Sprite Sprite1KM, Sprite Sprite2KM, Sprite Sprite3KM, Sprite Sprite1C, Sprite Sprite2C, Sprite Sprite3C,
        TextMeshProUGUI Text1, TextMeshProUGUI Text2, TextMeshProUGUI Text3, string Text1String, string Text2String, string Text3String, GameObject PromptPanel)
    {
        // Set the control prompts
        if (XInputController.current == null)// Use keyboard and mouse prompts if there is no controller connected
        {
            PromptImage1.sprite = Sprite1KM;

            if (Sprite2KM != null)
            {
                PromptImage2.sprite = Sprite2KM;
                PromptImage2.gameObject.SetActive(true);
            }

            else
            {
                PromptImage2.gameObject.SetActive(false);
            }

            if (Sprite3KM != null)
            {
                PromptImage3.sprite = Sprite3KM;
                PromptImage3.gameObject.SetActive(true);
            }

            else
            {
                PromptImage3.gameObject.SetActive(false);
            }
        }

        // Use controller prompts if there is a controller connected
        else
        {
            PromptImage1.sprite = Sprite1C;

            if (Sprite2C != null)
            {
                PromptImage2.sprite = Sprite2C;
                PromptImage2.gameObject.SetActive(true);
            }

            else
            {
                PromptImage2.gameObject.SetActive(false);
            }

            if (Sprite3C != null)
            {
                PromptImage3.sprite = Sprite3C;
                PromptImage3.gameObject.SetActive(true);
            }

            else
            {
                PromptImage3.gameObject.SetActive(false);
            }
        }

        // Set the prompt text
        Text1.text = Text1String;

        if (Text2String != null)
        {
            Text2.text = Text2String;
            Text2.gameObject.SetActive(true);
        }

        else
        {
            Text2.gameObject.SetActive(false);
        }

        if (Text3String != null)
        {
            Text3.text = Text3String;
            Text3.gameObject.SetActive(true);
        }

        else
        {
            Text3.gameObject.SetActive(false);
        }

        PromptPanel.SetActive(true);
    }
}