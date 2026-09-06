using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleInventoryMenu : MonoBehaviour
{
    [SerializeField] private GameObject InventoryMenu, PauseMenu, Slot1, Map;
    private SelectedUI SelectedUI;
    [SerializeField] private TextMeshProUGUI InfoText;
    [SerializeField] private Button EquipButton, CombineButton, DropButton;

    public void OnInventory()
    {
        LoadInventory(false);
    }

    public void LoadInventory(bool UseButtonPressed)
    {
        if (GetComponent<PlayerStats>().CurrentHealth > 0)
        {
            if (!PauseMenu.activeSelf)
            {
                if (InventoryMenu.activeSelf)
                {
                    if (EventSystem.current.currentSelectedGameObject != null)
                    {
                        SelectedUI = EventSystem.current.currentSelectedGameObject.GetComponent<SelectedUI>();
                        SelectedUI.GetComponent<Button>().colors = SelectedUI.UIColor;
                    }

                    InfoText.text = null;
                    EventSystem.current.SetSelectedGameObject(null);
                    InventoryMenu.SetActive(false);
                    EquipButton.interactable = false;
                    CombineButton.interactable = false;
                    DropButton.interactable = false;

                    if (CombineButton.GetComponent<ButtonController>().IsCombining)
                    {
                        CombineButton.GetComponent<ButtonController>().CombiningSlot.interactable = true;
                        CombineButton.GetComponent<ButtonController>().IsCombining = false;
                    }

                    if (!UseButtonPressed)
                    {
                        Time.timeScale = 1.0f;
                    }
                }

                else if (Map.activeSelf)
                {
                    Map.SetActive(false);
                    Time.timeScale = 1.0f;
                }

                else if(!InventoryMenu.activeSelf)
                {
                    if (Time.timeScale == 1.0f)
                    {
                        Time.timeScale = 0.0f;
                        EventSystem.current.SetSelectedGameObject(Slot1);
                        Slot1.GetComponent<ButtonController>().OnSelect(new BaseEventData(EventSystem.current));
                        InventoryMenu.SetActive(true);
                    }
                }

                else if (!Map.activeSelf)
                {
                    if (Time.timeScale == 1.0f)
                    {
                        Time.timeScale = 0.0f;
                        Map.SetActive(true);
                    }
                }
            }
        }
    }
}
