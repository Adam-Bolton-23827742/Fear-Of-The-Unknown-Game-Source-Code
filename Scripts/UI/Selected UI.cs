using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedUI : MonoBehaviour, IDeselectHandler
{
    [HideInInspector] public bool Deselected;
    [HideInInspector] public ColorBlock UIColor;
    [SerializeField] public GameObject InventoryMenu;

    private void Start()
    {
        UIColor = GetComponent<Button>().colors;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Deselected)
        {
            // Prevent Slot from being deselected if the player clicks on something that is not a slot
            if (InventoryMenu.activeSelf && EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.tag != "Slot")
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }

            // Rest the color when it is deselected so it does not look like its still selected
            else
            {
                GetComponent<Button>().colors = UIColor;
            }

            Deselected = false;
        }
    }

    public void OnDeselect(BaseEventData Data)
    {
        Deselected = true;
    }
}
