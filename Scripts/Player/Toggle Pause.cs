using UnityEngine;
using UnityEngine.EventSystems;

public class TogglePause : MonoBehaviour
{
    [SerializeField] private ButtonController ButtonController;
    [SerializeField] private GameObject Panels, InventoryMenu, ResumeButton, PauseMenu;

    private void OnPause()
    {
        if (GetComponent<PlayerStats>().CurrentHealth > 0)
        {
            if (!InventoryMenu.activeSelf)
            {
                // If the game is not paused
                if (Time.timeScale == 1.0f)
                {
                    // Pause time
                    Time.timeScale = 0.0f;

                    // Sets the panel to load to the pause panel and loads it
                    foreach (Transform Panel in Panels.transform)
                    {
                        if (Panel.name == "Pause Menu")
                        {
                            ButtonController.PanelToLoad = Panel.gameObject;
                        }
                    }

                    ButtonController.LoadPanel();
                    EventSystem.current.SetSelectedGameObject(ResumeButton);
                }

                // Else if the game is paused
                else if (PauseMenu.activeSelf)
                {
                    // Resume time
                    Time.timeScale = 1.0f;

                    // There is no panel to load so sets it to null
                    ButtonController.PanelToLoad = null;

                    // Sets the current panel and loads and then sets the current panel to null as it is not active
                    foreach (Transform Panel in Panels.transform)
                    {
                        if (Panel.gameObject.activeSelf)
                        {
                            ButtonController.CurrentPanel = Panel.gameObject;
                        }
                    }

                    EventSystem.current.SetSelectedGameObject(null);

                    ButtonController.LoadPanel();
                    ButtonController.CurrentPanel = null;
                }
            }
        }
    }
}
