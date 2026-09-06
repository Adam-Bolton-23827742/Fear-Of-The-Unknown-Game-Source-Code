using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class SetPrompt : MonoBehaviour
{
    [SerializeField] private Image PromptImage1, PromptImage2, PromptImage3;
    [SerializeField] private Sprite Sprite1KM, Sprite2KM, Sprite3KM, Sprite1C, Sprite2C, Sprite3C;
    private TextMeshProUGUI Text1, Text2, Text3;
    [SerializeField] string Text1String, Text2String, Text3String;
    [SerializeField] private GameObject PromptPanel;
    private ControlPrompts ControlPrompts;

    private void Start()
    {
        Text1 = PromptImage1.GetComponentInChildren<TextMeshProUGUI>();
        Text2 = PromptImage2.GetComponentInChildren<TextMeshProUGUI>();
        Text3 = PromptImage3.GetComponentInChildren<TextMeshProUGUI>();
        ControlPrompts = GetComponent<ControlPrompts>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // if the player collides with the trigger box
        if (other.tag == "Player")
        {
            ControlPrompts.SetPrompt(PromptImage1, PromptImage2, PromptImage3, Sprite1KM, Sprite2KM, Sprite3KM, Sprite1C, Sprite2C, Sprite3C, Text1, Text2, Text3, Text1String, Text2String, Text3String, PromptPanel);
            Destroy(gameObject);
        }
    }
}