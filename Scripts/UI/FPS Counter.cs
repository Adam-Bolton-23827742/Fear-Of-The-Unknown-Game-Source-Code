using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private TextMeshProUGUI TextMeshProUGUI;

    private void Start()
    {
        TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        //fps is found by dividing 1 by delta time. It is rounded to the nearest whole number for readability
        float FPS = Mathf.Round(1f / Time.deltaTime);

        //The FPS is displayed by converting to a string and set as the textmesh pro text
        TextMeshProUGUI.text = FPS.ToString();
    }
}
