using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour, ISelectHandler
{
    public GameObject CurrentPanel, PanelToLoad, SelectedButton;
    [SerializeField] private TextMeshProUGUI EquipOrUseText, InfoText;
    public Button[] SlotButtons;
    [SerializeField] private Inventory Inventory;
    [SerializeField] private Shooting Pistol, Shotgun;
    private Button Button, CurrentSelectedButton;
    [SerializeField] private Button EquipButton, DropButton;
    public Button CombineButton;
    [HideInInspector] public Button CombiningSlot;
    private int ItemToCombine;
    [HideInInspector] public bool IsCombining;
    [SerializeField] private Transform RightHand;
    [SerializeField] private ItemScriptableObjects GreenKey, YellowHeal;
    private GameObject SelectedItem;
    [SerializeField] private Fireplace Fireplace;
    [SerializeField] private SteamPump Pump;
    [SerializeField] private UseHammer UseHammer;
    [SerializeField] private ObjectInteraction PumpInteraction, FirePlaceInteraction;
    [SerializeField] private ToggleInventoryMenu ToggleInventoryMenu;
    [SerializeField] private Dialogue Dialogue;
    public string DialogueText;
    [SerializeField] private GameObject[] AllItems;
    private AudioSource AudioSource;
    [SerializeField] private AudioClip Equip, Heal;

    private void Start()
    {
        Button = GetComponent<Button>();
        AudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (transform.parent != null && transform.parent.GetComponent<Image>() != null)
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
            {
                transform.parent.GetComponent<Image>().color = Color.red;
            }

            else if (EventSystem.current.currentSelectedGameObject != null)
            {
                transform.parent.GetComponent<Image>().color = Color.white;
            }
        }
    }

    public void NewGame()
    {
        //Loads the game scene
        SceneManager.LoadScene("Game");
    }

    public void LoadPanel()
    {
        if (PanelToLoad != null)
        {
            //Activates the next panel
            PanelToLoad.SetActive(true);

            EventSystem.current.SetSelectedGameObject(SelectedButton);
        }
        
        if (CurrentPanel != null)
        {
            //Deactivates the current panel to prevent overlap
            CurrentPanel.SetActive(false);
        }
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void Quit()
    {
        //Closes the game
        Application.Quit();
    }

    public void Resume()
    {
        // Deactivates the pause menu and resumes time
        CurrentPanel.SetActive(false);
        Time.timeScale = 1.0f;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void MainMenu()
    {
        // Loads the main menu
        SceneManager.LoadScene("Main Menu");
    }

    public void EquipOrUse()
    {
        foreach (Transform Equipable in RightHand)
        {
            if (Equipable.GetComponent<Shooting>() != null)
            {
                Equipable.GetComponent<Shooting>().CanShoot = true;
                Equipable.GetComponent<Shooting>().IsReloading = false;
            }
        }

        for (int i = 0; i < SlotButtons.Length; i++)
        {
            if (SlotButtons[i].gameObject == SelectedItem)
            {
                // Equip the item
                if (Inventory.InventorySlots[i].Equipable)
                {
                    AudioSource.clip = Equip;
                    AudioSource.Play();

                    foreach (Transform Equipable in RightHand)
                    {
                        if (Equipable.gameObject.name == Inventory.InventorySlots[i].ItemName)
                        {
                            if (!Equipable.gameObject.activeSelf)
                            {
                                Equipable.gameObject.SetActive(true);
                            }

                            else
                            {
                                Equipable.gameObject.SetActive(false);
                            }

                            SetEquipText(CurrentSelectedButton.gameObject);
                        }

                        else if (Equipable.name != "police" && Equipable.name != "metarig")
                        {
                            Equipable.gameObject.SetActive(false);
                            SetEquipText(CurrentSelectedButton.gameObject);
                        }
                    }
                }

                // Use the item
                else
                {
                    // Use Health item
                    if (Inventory.InventorySlots[i].ItemType == "Heal")
                    {
                        if (Inventory.gameObject.GetComponent<PlayerStats>().CurrentHealth < Inventory.gameObject.GetComponent<PlayerStats>().MaxHealth)
                        {
                            AudioSource.clip = Heal;
                            AudioSource.Play();
                            Inventory.gameObject.GetComponent<PlayerStats>().CurrentHealth += Inventory.InventorySlots[i].HealthIncrease;
                            Inventory.RemoveFromInventory(i);
                        }

                        else
                        {
                            Dialogue.Set = true;
                            ToggleInventoryMenu.LoadInventory(true);
                        }
                    }

                    // Use key item
                    else if (Inventory.InventorySlots[i].ItemType == "Key")
                    {
                        Doors Door = GameObject.Find(Inventory.InventorySlots[i].Door).GetComponentInChildren<Doors>();

                        if (Door.NearDoor)
                        {
                            if (Door.IsLocked)
                            {
                                Door.IsLocked = false;
                                Door.AudioSource.clip = Door.UnlockSound;
                                Door.AudioSource.Play();
                                Door.LockedDoorImage.SetActive(false);
                                Inventory.RemoveFromInventory(i);
                            }
                        }

                        else
                        {
                            Dialogue.Set = true;
                            ToggleInventoryMenu.LoadInventory(true);
                        }
                    }

                    // Use lighter item
                    else if (Inventory.InventorySlots[i].ItemName == "Lighter")
                    {
                        if (FirePlaceInteraction.Colliding)
                        {
                            Fireplace.Light = true;
                            Inventory.RemoveFromInventory(i);
                        }

                        else
                        {
                            Dialogue.Set = true;
                            ToggleInventoryMenu.LoadInventory(true);
                        }
                    }

                    // Use valve item on the steam pump
                    else if (Inventory.InventorySlots[i].ItemName == "Valve")
                    {
                        if (PumpInteraction.Colliding)
                        {
                            Pump.TurnValve = true;
                            Inventory.RemoveFromInventory(i);
                        }

                        else
                        {
                            Dialogue.Set = true;
                            ToggleInventoryMenu.LoadInventory(true);
                        }
                    }

                    // Use hammer item on the planks
                    else if (Inventory.InventorySlots[i].ItemName == "Hammer")
                    {
                        if (UseHammer.IsColliding)
                        {
                            UseHammer.BreakPlank = true;
                            Inventory.RemoveFromInventory(i);
                        }

                        else
                        {
                            Dialogue.Set = true;
                            ToggleInventoryMenu.LoadInventory(true);
                        }
                    }

                    else if (Inventory.InventorySlots[i].ItemName == "Red Heal")
                    {
                        InfoText.text = "Select green health item to combine the " + Inventory.InventorySlots[i].ItemName.ToString() + " with";
                    }
                }
                break;
            }
        }
    }

    public void Combine()
    {
        Button.interactable = false;
        EquipButton.interactable = false;

        for (int i = 0; i < SlotButtons.Length; i++)
        {
            if (SlotButtons[i].gameObject == SelectedItem)
            {
                CombiningSlot = SlotButtons[i];
                CombiningSlot.interactable = false;
                IsCombining = true;
                ItemToCombine = i;
                InfoText.text = "Select another item to combine the " + Inventory.InventorySlots[i].ItemName.ToString() + " with";
                EventSystem.current.SetSelectedGameObject(SelectedButton);
            }
        }
    }

    // Drop the item and spawn it at the players position
    public void Drop()
    {
        for (int i = 0; i < SlotButtons.Length; i++)
        {
            if (SlotButtons[i].gameObject == SelectedItem)
            {
                foreach (GameObject Item in AllItems)
                {
                    if (Inventory.InventorySlots[i].ItemName == Item.name)
                    {
                        AudioSource.Play();

                        GameObject ItemClone = Instantiate(Item, Inventory.transform.position, Item.transform.rotation);

                        ItemClone.GetComponent<Items>().Amount = Inventory.InventorySlots[i].Amount;

                        if (Inventory.InventorySlots[i].ItemName == "Pistol")
                        {
                            Pistol.gameObject.SetActive(false);
                        }

                        else if (Inventory.InventorySlots[i].ItemName == "Shotgun")
                        {
                            Shotgun.gameObject.SetActive(false);
                        }
                        break;
                    }
                }

                Inventory.RemoveFromInventory(i);
                break;
            }
        }
    }

    public void OnSelect(BaseEventData Data)
    {
        Button = GetComponent<Button>();

        for (int j = 0; j < SlotButtons.Length; j++)
        {
            if (name == SlotButtons[j].name)
            {
                CombineButton.GetComponent<ButtonController>().SelectedItem = EventSystem.current.currentSelectedGameObject;
                EquipButton.GetComponent<ButtonController>().SelectedItem = EventSystem.current.currentSelectedGameObject;
                DropButton.GetComponent<ButtonController>().SelectedItem = EventSystem.current.currentSelectedGameObject;

                // Set the normal color to prevent the slot's color from flickering if it is reselected
                ColorBlock CB = Button.colors;
                CB.normalColor = Button.colors.selectedColor;
                Button.colors = CB;

                for (int i = 0; i < SlotButtons.Length; i++)
                {
                    if (SlotButtons[i] == Button)
                    {
                        if (!CombineButton.GetComponent<ButtonController>().IsCombining)
                        {
                            if (Inventory.InventorySlots[i].ItemName != null)
                            {
                                InfoText.text = "Item: " + Inventory.InventorySlots[i].ItemName.ToString();
                                DropButton.interactable = true;
                            }

                            else
                            {
                                InfoText.text = null;
                                DropButton.interactable = false;
                            }
                        }
                    }
                }

                EquipButton.GetComponent<ButtonController>().CurrentSelectedButton = Button;
                SetEquipText(Button.gameObject);
                SetCombineInteractivity(Button.gameObject);
            }
        }
    }

    public void SetEquipText(GameObject Button)
    {
        for (int i = 0; i < SlotButtons.Length; i++)
        {
            if (SlotButtons[i].gameObject == Button)
            {
                if (Inventory.InventorySlots[i].Equipable)
                {
                    foreach (Transform Equipable in RightHand)
                    {
                        if (Equipable.gameObject.name == Inventory.InventorySlots[i].ItemName)
                        {
                            if (!Equipable.gameObject.activeSelf)
                            {
                                EquipOrUseText.text = "Equip";
                            }
                            
                            else
                            {
                                EquipOrUseText.text = "Unequip";
                            }

                            EquipButton.interactable = true;
                        }
                    }
                }

                else if (Inventory.InventorySlots[i].ItemType == "Heal" || Inventory.InventorySlots[i].ItemType == "Key" || Inventory.InventorySlots[i].ItemType == "Use")
                {
                    EquipOrUseText.text = "Use";
                    EquipButton.interactable = true;
                }

                else
                {
                    EquipButton.interactable = false;
                }

                break;
            }
        }
    }

    public void SetCombineInteractivity(GameObject Button)
    {
        for (int i = 0; i < SlotButtons.Length; i++)
        {
            if (SlotButtons[i].gameObject == Button)
            {
                if (Inventory.InventorySlots[i].Combinable)
                {
                    CombineButton.interactable = true;
                }

                else
                {
                    CombineButton.interactable = false;
                }

                break;
            }
        }
    }

    public void SlotPressed()
    {
        for (int i = 0; i < SlotButtons.Length; i++)
        {
            if (SlotButtons[i] == Button)
            {
                if (!CombineButton.GetComponent<ButtonController>().IsCombining)
                {
                    EventSystem.current.SetSelectedGameObject(EquipButton.gameObject);
                    break;
                }

                else
                {
                    EquipButton.interactable = true;
                    ItemToCombine = CombineButton.GetComponent<ButtonController>().ItemToCombine;

                    if (Inventory.InventorySlots[i].ItemName != null)
                    {
                        if (Inventory.InventorySlots[ItemToCombine].ItemName == "Pistol" && Inventory.InventorySlots[i].ItemName == "Pistol Ammo")
                        {
                            AudioSource.Play();
                            Pistol.Reload(i, InfoText);

                            if (Inventory.InventorySlots[i].Amount == 0)
                            {
                                Inventory.RemoveFromInventory(i);
                            }
                        }

                        else if (Inventory.InventorySlots[ItemToCombine].ItemName == "Pistol Ammo" && Inventory.InventorySlots[i].ItemName == "Pistol")
                        {
                            AudioSource.Play();
                            Pistol.Reload(ItemToCombine, InfoText);

                            if (Inventory.InventorySlots[ItemToCombine].Amount == 0)
                            {
                                Inventory.RemoveFromInventory(ItemToCombine);
                            }
                        }

                        else if (Inventory.InventorySlots[ItemToCombine].ItemName == "Shotgun" && Inventory.InventorySlots[i].ItemName == "Shotgun Ammo")
                        {
                            AudioSource.Play();
                            Shotgun.Reload(i, InfoText);

                            if (Inventory.InventorySlots[i].Amount == 0)
                            {
                                Inventory.RemoveFromInventory(i);
                            }
                        }

                        else if (Inventory.InventorySlots[ItemToCombine].ItemName == "Shotgun Ammo" && Inventory.InventorySlots[i].ItemName == "Shotgun")
                        {
                            AudioSource.Play();
                            Shotgun.Reload(ItemToCombine, InfoText);

                            if (Inventory.InventorySlots[ItemToCombine].Amount == 0)
                            {
                                Inventory.RemoveFromInventory(ItemToCombine);
                            }
                        }

                        else if (Inventory.InventorySlots[ItemToCombine].ItemName == "First Half Of A Green Key" && Inventory.InventorySlots[i].ItemName == "Second Half Of A Green Key" ||
                            Inventory.InventorySlots[ItemToCombine].ItemName == "Second Half Of A Green Key" && Inventory.InventorySlots[i].ItemName == "First Half Of A Green Key")
                        {
                            AudioSource.Play();
                            Inventory.RemoveFromInventory(ItemToCombine);
                            Inventory.InventorySlots[i].ItemName = GreenKey.ItemName;
                            Inventory.InventorySlots[i].AmmoType = GreenKey.AmmoType;
                            Inventory.InventorySlots[i].ItemType = GreenKey.ItemType;
                            Inventory.InventorySlots[i].Equipable = GreenKey.Equipable;
                            Inventory.InventorySlots[i].Combinable = GreenKey.Combinable;
                            Inventory.InventorySlots[i].Stackable = GreenKey.IsStackable;
                            Inventory.InventorySlots[i].MaxStack = GreenKey.MaxStack;
                            Inventory.InventorySlots[i].HealthIncrease = GreenKey.HealthIncrease;
                            Inventory.InventorySlots[i].TakeMultipleSlots = GreenKey.TakeMultipleSlots;
                            Inventory.InventorySlots[i].Door = GreenKey.Door;
                            Inventory.ItemIcons[i].sprite = GreenKey.ItemIcon;
                            InfoText.text = "Item: " + Inventory.InventorySlots[i].ItemName.ToString();
                        }

                        else if (Inventory.InventorySlots[ItemToCombine].ItemName == "Red Heal" && Inventory.InventorySlots[i].ItemName == "Green Heal" ||
                            Inventory.InventorySlots[ItemToCombine].ItemName == "Green Heal" && Inventory.InventorySlots[i].ItemName == "Red Heal")
                        {
                            AudioSource.Play();
                            Inventory.RemoveFromInventory(ItemToCombine);
                            Inventory.InventorySlots[i].ItemName = YellowHeal.ItemName;
                            Inventory.InventorySlots[i].AmmoType = YellowHeal.AmmoType;
                            Inventory.InventorySlots[i].ItemType = YellowHeal.ItemType;
                            Inventory.InventorySlots[i].Equipable = YellowHeal.Equipable;
                            Inventory.InventorySlots[i].Combinable = YellowHeal.Combinable;
                            Inventory.InventorySlots[i].Stackable = YellowHeal.IsStackable;
                            Inventory.InventorySlots[i].MaxStack = YellowHeal.MaxStack;
                            Inventory.InventorySlots[i].HealthIncrease = YellowHeal.HealthIncrease;
                            Inventory.InventorySlots[i].TakeMultipleSlots = YellowHeal.TakeMultipleSlots;
                            Inventory.InventorySlots[i].Door = YellowHeal.Door;
                            Inventory.ItemIcons[i].sprite = YellowHeal.ItemIcon;
                            InfoText.text = "Item: " + Inventory.InventorySlots[i].ItemName.ToString();
                        }

                        else if (Inventory.InventorySlots[ItemToCombine].ItemName == Inventory.InventorySlots[i].ItemName && Inventory.InventorySlots[ItemToCombine].Stackable)
                        {
                            if (Inventory.InventorySlots[i].Amount < Inventory.InventorySlots[i].MaxStack)
                            {
                                if (Inventory.InventorySlots[ItemToCombine].Amount + Inventory.InventorySlots[i].Amount <= Inventory.InventorySlots[i].MaxStack)
                                {
                                    AudioSource.Play();
                                    Inventory.InventorySlots[i].Amount += Inventory.InventorySlots[ItemToCombine].Amount;
                                    Inventory.RemoveFromInventory(ItemToCombine);
                                }

                                else
                                {
                                    AudioSource.Play();
                                    int Difference = Inventory.InventorySlots[i].MaxStack - Inventory.InventorySlots[i].Amount;
                                    Inventory.InventorySlots[i].Amount += Difference;
                                    Inventory.InventorySlots[ItemToCombine].Amount -= Difference;

                                    if (Inventory.InventorySlots[ItemToCombine].Amount == 0)
                                    {
                                        Inventory.RemoveFromInventory(i);
                                    }
                                }

                                InfoText.text = "Item: " + Inventory.InventorySlots[i].ItemName.ToString();
                            }

                            else
                            {
                                InfoText.text = "Slot already full!";
                            }
                        }

                        else
                        {
                            if (Inventory.InventorySlots[ItemToCombine].ItemName != null)
                            {
                                if (Inventory.InventorySlots[i].ItemName != null)
                                {
                                    InfoText.text = "Cannot combine the " + Inventory.InventorySlots[ItemToCombine].ItemName.ToString() + " and the " + Inventory.InventorySlots[i].ItemName.ToString() + " together!";
                                }

                                else
                                {
                                    InfoText.text = null;
                                }
                            }
                        }

                        CombineButton.GetComponent<ButtonController>().CombiningSlot.interactable = true;
                        CombineButton.GetComponent<ButtonController>().IsCombining = false;
                    }
                }
            }
        }
    }
}