using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class SwitchMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] Menus;
    [SerializeField] private Button[] Slots;
    private int Index;
    [SerializeField] private ButtonController CombineButon;
    [SerializeField] private TextMeshProUGUI InfoText;
    [SerializeField] private Image[] Next, Previous;
    [SerializeField] private Sprite NextKM, NextC, PreviousKM, PreviousC;

    // Update is called once per frame
    void Update()
    {
        // Set the control rompts for switching panels depending on if a controller or keyboard and mouse is being used
        if (XInputController.current == null)
        {
            foreach (Image Image in Next)
            {
                Image.sprite = NextKM;
                
            }

            foreach (Image Image in Previous)
            {
                Image.sprite = PreviousKM;
            }
        }

        else
        {
            foreach (Image Image in Next)
            {
                Image.sprite = NextC;

            }

            foreach (Image Image in Previous)
            {
                Image.sprite = PreviousC;
            }
        }

        for (int i = 0; i < Menus.Length; i++)
        {
            if (Menus[i].activeSelf)
            {
                // Deactivate the current menu, and then activate the next menu
                if (Input.GetButtonDown("Next Menu"))
                {
                    Menus[i].SetActive(false);

                    if (i < Menus.Length - 1)
                    {
                        Index = i + 1;
                    }

                    else
                    {
                        Index = 0;
                    }

                    Switch();
                    break;
                }

                // Deactivate the current menu, and then activate the previous menu
                else if (Input.GetButtonDown("Back Menu"))
                {
                    Menus[i].SetActive(false);

                    if (i > 0)
                    {
                        Index = i - 1;
                    }

                    else
                    {
                        Index = Menus.Length - 1;
                    }

                    Switch();
                    break;
                }
            }
        }
    }

    private void Switch()
    {
        if (Menus[Index].name != "Inventory Menu")
        {
            EventSystem.current.SetSelectedGameObject(null);
            CombineButon.IsCombining = false;
            InfoText.text = null;

            foreach (Button Slot in Slots)
            {
                Slot.interactable = true;
            }
        }

        else
        {
            EventSystem.current.SetSelectedGameObject(Slots[0].gameObject);
        }

        Menus[Index].SetActive(true);
    }
}