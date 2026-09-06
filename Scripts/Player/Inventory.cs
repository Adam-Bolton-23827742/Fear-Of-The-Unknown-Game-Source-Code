using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public string ItemName, AmmoType, ItemType;// Item types include: Weapon, heal, key item
    public int Amount, HealthIncrease, MaxStack;
    public Sprite ItemIcon;
    public bool Equipable, Combinable, Stackable, TakeMultipleSlots;
    public string Door;
}

public class Inventory : MonoBehaviour
{
    // Array of the icons used to display which items are currently in the player's inventory
    public Slot[] InventorySlots;

    // Array of the icons used to display which items are currently in the player's inventory
    public Image[] ItemIcons;

    // Array of the text displaying how much is in each slot
    [SerializeField] private TextMeshProUGUI[] ItemAmountText;

    // Array for how much free space is in each slot, apart from the first slot, when trying to split a stack of items between slots later on
    private int[] FreeSpace = { 0, 0, 0, 0, 0, 0, 0, 0 };

    [SerializeField] private ButtonController ButtonController;

    private int TransparentSlot = -1, AddedAmount;

    private void Start()
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            InventorySlots[i] = new Slot();
        }
    }

    private void Update()
    {
        // if the player has no items left in one of the slots then remove it from the inventory and icon arrays
        for (int Item = 0; Item < InventorySlots.Length; Item++)
        {
            if (InventorySlots[Item].Amount == 0)
            {
                InventorySlots[Item].ItemName = null;
                ItemIcons[Item].sprite = null;
                ItemIcons[Item].color = new Color(ItemIcons[Item].color.r, ItemIcons[Item].color.g, ItemIcons[Item].color.b, 0);
            }

            else
            {
                if (InventorySlots[Item].TakeMultipleSlots)
                {
                    if (Item == 1 ||  Item == 3 || Item == 5)
                    {
                        TransparentSlot = Item;
                        ItemIcons[TransparentSlot].color = new Color(ItemIcons[TransparentSlot].color.r, ItemIcons[TransparentSlot].color.g, ItemIcons[TransparentSlot].color.b, 0);
                    }
                }

                if (Item != TransparentSlot)
                {
                    ItemIcons[Item].color = new Color(ItemIcons[Item].color.r, ItemIcons[Item].color.g, ItemIcons[Item].color.b, 255);
                }
            }
        }
    }

    public void FindStackableSlot(ItemScriptableObjects NewItem, GameObject Pickup)
    {
        // Find another potential instance of the same object type to stack with
        for (int Item = 0; Item < InventorySlots.Length; Item++)
        {
            if (InventorySlots[Item].ItemName == NewItem.ItemName)
            {
                // If the stack has not reached its max value call the add inventory function
                if (InventorySlots[Item].Amount < NewItem.MaxStack)
                {
                    AddToInventory(Item, -1, NewItem, Pickup);

                    // End execution of the function as a slot has been found
                    return;
                }
            }
        }

        // Looks for the first empty slot if a slot of the same item cannot be stacked with, or if there are no slots containing the same item
        FindEmptySlot(NewItem, Pickup);
    }

    public void FindEmptySlot(ItemScriptableObjects NewItem, GameObject Pickup)
    {
        int SlotsFound = 0;
        int Index1 = 0;

        // Looks for the first empty slot if there is no other objects of the same type to stack with, or if the object is not stackable
        for (int Item = 0; Item < InventorySlots.Length; Item++)
        {
            // If the current index in the array is empty or null
            if (InventorySlots[Item].ItemName == null)
            {
                // Counts how many empty slots are found
                SlotsFound++;


                // if the item only takes up 1 slot
                if (!NewItem.TakeMultipleSlots)
                {
                    // Pass through the item to the add to inventory function
                    AddToInventory(Item, -1, NewItem, Pickup);
                    break;
                }

                // else if the item takes up 2 slots
                else
                {
                    // If 1 free slot is found
                    if (SlotsFound == 1)
                    {
                        // Stores the first slot that this item will take
                        Index1 = Item;
                    }

                    // if 2 slots are found
                    else
                    {
                        // Pass through both these slots found into the add inventory function
                        AddToInventory(Index1, Item, NewItem, Pickup);
                        break;
                    }
                }
            }
        }
    }

    private void AddToInventory(int Index, int Index2, ItemScriptableObjects NewItem, GameObject Pickup)
    {
        // Add the item name, amount, and icon to the inventory
        if (!NewItem.IsStackable)
        {
            InventorySlots[Index].Amount++;
        }

        InventorySlots[Index].ItemName = NewItem.ItemName;
        InventorySlots[Index].AmmoType = NewItem.AmmoType;
        InventorySlots[Index].ItemType = NewItem.ItemType;
        InventorySlots[Index].Equipable = NewItem.Equipable;
        InventorySlots[Index].Combinable = NewItem.Combinable;
        InventorySlots[Index].Stackable = NewItem.IsStackable;
        InventorySlots[Index].MaxStack = NewItem.MaxStack;
        InventorySlots[Index].HealthIncrease = NewItem.HealthIncrease;
        InventorySlots[Index].TakeMultipleSlots = NewItem.TakeMultipleSlots;
        InventorySlots[Index].Door = NewItem.Door;
        ItemIcons[Index].sprite = NewItem.ItemIcon;

        // If the item takes up 2 slots
        if (Index2 >= 0)
        {
            // If the item is in either slots 2, 4, or 6 then the it needs to be moved to the left by 1 due to the layout of the UI slots being in rows of 2, which would cause a visual issue where the item is split in half between 2 different rows.
            if (Index == 1 || Index == 3 || Index == 5)
            {
                // Store what item is currently inside the slot before this item
                string PreviousItem = InventorySlots[Index - 1].ItemName;

                // Store how many items are inside the slot before this item
                int PreviousAmount = InventorySlots[Index - 1].Amount;

                // Store how ammo type inside the slot before this item
                string PreviousType = InventorySlots[Index - 1].AmmoType;

                // Store the item type inside the slot before this item
                string PreviousItemType = InventorySlots[Index - 1].ItemType;

                // Store if the previous item is equipable
                bool PreviousEquipable = InventorySlots[Index - 1].Equipable;

                // Store if the previous item is combinable
                bool PreviousCombinable = InventorySlots[Index - 1].Combinable;

                // Store if the previous item is stackable
                bool PreviousStackable = InventorySlots[Index - 1].Stackable;

                // Store the previous item's health increase
                int PreviousHealthIncrease = InventorySlots[Index - 1].HealthIncrease;

                // Store the previous item's max stack
                int PreviousMaxStack = InventorySlots[Index - 1].MaxStack;

                // Store if the previous item takes up multiple slots or not
                bool PreviousTakesMultipleSlots = InventorySlots[Index - 1].TakeMultipleSlots;

                // Store how the icon for the item inside the slot before this item
                Sprite PreviousIcon = ItemIcons[Index - 1].sprite;

                // Swaps the the first part of this item with the item to its left
                InventorySlots[Index - 1].ItemName = NewItem.ItemName;
                InventorySlots[Index - 1].Amount = 1;
                InventorySlots[Index - 1].AmmoType = NewItem.AmmoType;
                InventorySlots[Index - 1].ItemType = NewItem.ItemType;
                InventorySlots[Index - 1].Equipable = NewItem.Equipable;
                InventorySlots[Index - 1].Combinable = NewItem.Combinable;
                InventorySlots[Index - 1].Stackable = NewItem.IsStackable;
                InventorySlots[Index - 1].HealthIncrease = NewItem.HealthIncrease;
                InventorySlots[Index - 1].MaxStack = NewItem.MaxStack;
                InventorySlots[Index - 1].TakeMultipleSlots = NewItem.TakeMultipleSlots;
                ItemIcons[Index - 1].sprite = NewItem.ItemIcon;

                InventorySlots[Index].ItemName = PreviousItem;
                InventorySlots[Index].Amount = PreviousAmount;
                InventorySlots[Index].AmmoType = PreviousType;
                InventorySlots[Index].ItemType = PreviousItemType;
                InventorySlots[Index].Equipable = PreviousEquipable;
                InventorySlots[Index].Combinable = PreviousCombinable;
                InventorySlots[Index].Stackable = PreviousStackable;
                InventorySlots[Index].HealthIncrease = PreviousHealthIncrease;
                InventorySlots[Index].MaxStack = PreviousMaxStack;
                InventorySlots[Index].TakeMultipleSlots = PreviousTakesMultipleSlots;
                ItemIcons[Index].sprite = PreviousIcon;

                // Store the new position of the first part of this item
                Index -= 1;
            }

            // Store what item is currently inside the slot after this item
            string NextItem = InventorySlots[Index + 1].ItemName;

            // Store how many items are inside the slot after this item
            int NextAmount = InventorySlots[Index + 1].Amount;

            // Store how ammo type inside the slot after this item
            string NextType = InventorySlots[Index + 1].AmmoType;

            // Store the item type inside the slot after this item
            string NextItemType = InventorySlots[Index + 1].ItemType;

            // Store if the next item is equipable
            bool NextEquipable = InventorySlots[Index + 1].Equipable;

            // Store if the next item is combinable
            bool NextCombinable = InventorySlots[Index + 1].Combinable;

            // Store if the next item is stackable
            bool NextStackable = InventorySlots[Index + 1].Stackable;

            // Store the next item's health increase
            int NextHealthIncrease = InventorySlots[Index + 1].HealthIncrease;

            // Store the next item's max stack
            int NextMaxStack = InventorySlots[Index + 1].MaxStack;

            // Store if the next item takes up multiple slots or not
            bool NextTakeMultipleSlots = InventorySlots[Index + 1].TakeMultipleSlots;

            // Store how the icon for the item inside the slot after this item
            Sprite NextIcon = ItemIcons[Index + 1].sprite;

            // Move the first part of this item to the right in the inventory
            InventorySlots[Index + 1].ItemName = NewItem.ItemName;
            InventorySlots[Index + 1].Amount = 1;
            InventorySlots[Index + 1].AmmoType = NewItem.AmmoType;
            InventorySlots[Index + 1].ItemType = NewItem.ItemType;
            InventorySlots[Index + 1].Equipable = NewItem.Equipable;
            InventorySlots[Index + 1].Combinable = NewItem.Combinable;
            InventorySlots[Index + 1].Stackable = NewItem.IsStackable;
            InventorySlots[Index + 1].HealthIncrease = NewItem.HealthIncrease;
            InventorySlots[Index + 1].MaxStack = NewItem.MaxStack;
            InventorySlots[Index + 1].TakeMultipleSlots = NewItem.TakeMultipleSlots;
            //InventorySlots[Index + 1].ItemIcon = NewItem.ItemIcon;

            // Change the width of the icon to fit it across multiple UI slots
            ItemIcons[Index].rectTransform.sizeDelta = new Vector2(400f, 175f);

            // Swaps items if the second part of the item is not directly after the first part to prevent the item from being swapped with an empty slot
            if (Index2 != Index + 1)
            {
                // Swaps whatever item was in the index after the item into a empty slot
                InventorySlots[Index2].ItemName = NextItem;

                // Swaps the amount whatever item was in the index after the item into a empty slot
                InventorySlots[Index2].Amount = NextAmount;

                // Swaps the ammo type into an empty slot
                InventorySlots[Index2].AmmoType = NextType;

                // Swaps the ammo type into an empty slot
                InventorySlots[Index2].ItemType = NextItemType;

                // Swaps if its equipable into an empty slot
                InventorySlots[Index2].Equipable = NextEquipable;

                // Swaps if its combinable into an empty slot
                InventorySlots[Index2].Combinable = NextCombinable;

                // Swaps if its stackable into an empty slot
                InventorySlots[Index2].Stackable = NextStackable;

                // Swaps the health increase into an empty slot
                InventorySlots[Index2].HealthIncrease = NextHealthIncrease;

                // Swaps the max stack into an empty slot
                InventorySlots[Index2].MaxStack = NextMaxStack;

                // Swaps if it takes up multiple slots or not into an empty slot
                InventorySlots[Index2].TakeMultipleSlots = NextTakeMultipleSlots;

                // Swaps the item icon that was in the index after the item into a empty slot
                ItemIcons[Index2].sprite = NextIcon;
            }

            ItemAmountText[Index].gameObject.SetActive(false);
        }

        // Else if the item takes up only 1 slot
        else
        {
            // Change the width of the icon to fit it across one UI slot
            ItemIcons[Index].rectTransform.sizeDelta = new Vector2(175f, 175f);
        }

        // If the object can be stacked
        if (NewItem.IsStackable)
        {
            Items Items = Pickup.GetComponent<Items>();

            // Generate a random number for how many of the object come stacked together upon pick up if the object has not already been picked up before
            if (Items.Amount == 0)
            {
                AddedAmount = UnityEngine.Random.Range(NewItem.MinRandomAmount, NewItem.MaxRandomAmount);
            }

            else
            {
                AddedAmount = Items.Amount;
            }

            // Add the new stack to the array
            InventorySlots[Index].Amount += AddedAmount;

            // If the new stack exceedes the stack limit
            if (InventorySlots[Index].Amount > NewItem.MaxStack)
            {
                // Find extra objects that sent it over the limit
                int Difference = InventorySlots[Index].Amount - NewItem.MaxStack;

                // Find a different potential instance of the same object type to stack with
                for (int Item = Index + 1; Item < InventorySlots.Length; Item++)
                {
                    if (InventorySlots[Item].ItemName == NewItem.ItemName)
                    {
                        // If the new instance can fit any of the left over items
                        if (InventorySlots[Item].Amount < NewItem.MaxStack)
                        {
                            // If the new instance can fit all of the items
                            if (InventorySlots[Item].Amount + Difference <= NewItem.MaxStack)
                            {
                                // Max out the previous overloaded stack
                                InventorySlots[Index].Amount = NewItem.MaxStack;

                                // Add the left over items to the other stack
                                InventorySlots[Item].Amount += Difference;
                                Destroy(Pickup);
                                return;
                            }

                            // If the new instance cannot fit all of the items
                            else
                            {
                                // Calculate how much space can be used to store some of the items
                                int CurrentFreeSpace = NewItem.MaxStack - InventorySlots[Item].Amount;

                                // Store the amount of items that will potentially be stacked int each slot later on
                                if (Item == 1 || Item == 2 || Item == 3 || Item == 4 || Item == 5 || Item == 6 || Item == 7)
                                {
                                    FreeSpace[Item] = CurrentFreeSpace;
                                }
                               
                                // Subtract from the difference to prevent too many items being added later on
                                Difference -= CurrentFreeSpace;
                            }
                        }
                    }
                }

                // Looks for the first empty slot if there is no other items of the same type to stack with the left over items
                for (int Item = 0; Item < InventorySlots.Length; Item++)
                {
                    // If an empty slot is found
                    if (InventorySlots[Item].ItemName == null)
                    {
                        // Max out the previous overloaded stack
                        InventorySlots[Index].Amount = NewItem.MaxStack;

                        // Add the object type to the new slot
                        InventorySlots[Item].ItemName = NewItem.ItemName;

                        // Add the left over items saved previously to each stack 
                        if (FreeSpace[1] > 0)
                        {
                            InventorySlots[1].ItemName = NewItem.ItemName;
                            InventorySlots[1].Amount += FreeSpace[1];
                            InventorySlots[1].AmmoType = NewItem.AmmoType;
                            InventorySlots[1].ItemType = NewItem.ItemType;
                            InventorySlots[1].Equipable = NewItem.Equipable;
                            InventorySlots[1].Combinable = NewItem.Combinable;
                            InventorySlots[1].Stackable = NewItem.IsStackable;
                            InventorySlots[1].HealthIncrease = NewItem.HealthIncrease;
                            InventorySlots[1].MaxStack = NewItem.MaxStack;
                            InventorySlots[1].TakeMultipleSlots = NewItem.TakeMultipleSlots;
                        }

                        if (FreeSpace[2] > 0)
                        {
                            InventorySlots[2].ItemName = NewItem.ItemName;
                            InventorySlots[2].Amount += FreeSpace[2];
                            InventorySlots[2].AmmoType = NewItem.AmmoType;
                            InventorySlots[2].ItemType = NewItem.ItemType;
                            InventorySlots[2].Equipable = NewItem.Equipable;
                            InventorySlots[2].Combinable = NewItem.Combinable;
                            InventorySlots[2].Stackable = NewItem.IsStackable;
                            InventorySlots[2].HealthIncrease = NewItem.HealthIncrease;
                            InventorySlots[2].MaxStack = NewItem.MaxStack;
                            InventorySlots[2].TakeMultipleSlots = NewItem.TakeMultipleSlots;
                        }

                        if (FreeSpace[3] > 0)
                        {
                            InventorySlots[3].ItemName = NewItem.ItemName;
                            InventorySlots[3].Amount += FreeSpace[3];
                            InventorySlots[3].AmmoType = NewItem.AmmoType;
                            InventorySlots[3].ItemType = NewItem.ItemType;
                            InventorySlots[3].Equipable = NewItem.Equipable;
                            InventorySlots[3].Combinable = NewItem.Combinable;
                            InventorySlots[3].Stackable = NewItem.IsStackable;
                            InventorySlots[3].HealthIncrease = NewItem.HealthIncrease;
                            InventorySlots[3].MaxStack = NewItem.MaxStack;
                            InventorySlots[3].TakeMultipleSlots = NewItem.TakeMultipleSlots;
                        }

                        if (FreeSpace[4] > 0)
                        {
                            InventorySlots[4].ItemName = NewItem.ItemName;
                            InventorySlots[4].Amount += FreeSpace[4];
                            InventorySlots[4].AmmoType = NewItem.AmmoType;
                            InventorySlots[4].ItemType = NewItem.ItemType;
                            InventorySlots[4].Equipable = NewItem.Equipable;
                            InventorySlots[4].Combinable = NewItem.Combinable;
                            InventorySlots[4].Stackable = NewItem.IsStackable;
                            InventorySlots[4].HealthIncrease = NewItem.HealthIncrease;
                            InventorySlots[4].MaxStack = NewItem.MaxStack;
                            InventorySlots[4].TakeMultipleSlots = NewItem.TakeMultipleSlots;
                        }

                        if (FreeSpace[5] > 0)
                        {
                            InventorySlots[5].ItemName = NewItem.ItemName;
                            InventorySlots[5].Amount += FreeSpace[5];
                            InventorySlots[5].AmmoType = NewItem.AmmoType;
                            InventorySlots[5].ItemType = NewItem.ItemType;
                            InventorySlots[5].Equipable = NewItem.Equipable;
                            InventorySlots[5].Combinable = NewItem.Combinable;
                            InventorySlots[5].Stackable = NewItem.IsStackable;
                            InventorySlots[5].HealthIncrease = NewItem.HealthIncrease;
                            InventorySlots[5].MaxStack = NewItem.MaxStack;
                            InventorySlots[5].TakeMultipleSlots = NewItem.TakeMultipleSlots;
                        }

                        if (FreeSpace[6] > 0)
                        {
                            InventorySlots[6].ItemName = NewItem.ItemName;
                            InventorySlots[6].Amount += FreeSpace[6];
                            InventorySlots[6].AmmoType = NewItem.AmmoType;
                            InventorySlots[6].ItemType = NewItem.ItemType;
                            InventorySlots[6].Equipable = NewItem.Equipable;
                            InventorySlots[6].Combinable = NewItem.Combinable;
                            InventorySlots[6].Stackable = NewItem.IsStackable;
                            InventorySlots[6].HealthIncrease = NewItem.HealthIncrease;
                            InventorySlots[6].MaxStack = NewItem.MaxStack;
                            InventorySlots[6].TakeMultipleSlots = NewItem.TakeMultipleSlots;
                        }

                        if (FreeSpace[7] > 0)
                        {
                            InventorySlots[7].ItemName = NewItem.ItemName;
                            InventorySlots[7].Amount += FreeSpace[7];
                            InventorySlots[7].AmmoType = NewItem.AmmoType;
                            InventorySlots[7].ItemType = NewItem.ItemType;
                            InventorySlots[7].Equipable = NewItem.Equipable;
                            InventorySlots[7].Combinable = NewItem.Combinable;
                            InventorySlots[7].Stackable = NewItem.IsStackable;
                            InventorySlots[7].HealthIncrease = NewItem.HealthIncrease;
                            InventorySlots[7].MaxStack = NewItem.MaxStack;
                            InventorySlots[7].TakeMultipleSlots = NewItem.TakeMultipleSlots;
                        }

                        // Add the left over items to the other stack
                        InventorySlots[Item].Amount += Difference;

                        InventorySlots[Item].AmmoType = NewItem.AmmoType;
                        InventorySlots[Item].ItemType = NewItem.ItemType;
                        InventorySlots[Item].Equipable = NewItem.Equipable;
                        InventorySlots[Item].Combinable = NewItem.Combinable;
                        InventorySlots[Item].Stackable = NewItem.IsStackable;
                        InventorySlots[Item].HealthIncrease = NewItem.HealthIncrease;
                        InventorySlots[Item].MaxStack = NewItem.MaxStack;
                        InventorySlots[Item].TakeMultipleSlots = NewItem.TakeMultipleSlots;

                        // Adds the item sprite into the slot in the inventory menu
                        ItemIcons[Item].sprite = NewItem.ItemIcon;

                        Destroy(Pickup);
                        return;
                    }
                }

                // Resets the amount if it is over the limit
                InventorySlots[Index].Amount -= AddedAmount;

                // Resets the free space values to 0 to prevent extra items being given when the player trys to pick the items up again later when they have more space for the items
                foreach (int FreeSpaceValue in FreeSpace)
                {
                    FreeSpace[FreeSpaceValue] = 0;
                }
            }

            else
            {
                // Destoy the item if it fits into the inventory slot
                Destroy(Pickup);
            }
        }

        else
        {
            // Destoy the item after it is picked up if it is not stackable
            Destroy(Pickup);
        }
    }

    public void RemoveFromInventory(int Index)
    {
        InventorySlots[Index].ItemName = null;
        InventorySlots[Index].Amount = 0;
        InventorySlots[Index].AmmoType = null;
        InventorySlots[Index].ItemType = null;
        InventorySlots[Index].HealthIncrease = 0;
        InventorySlots[Index].MaxStack = 0;
        InventorySlots[Index].ItemIcon = null;
        InventorySlots[Index].Equipable = false;
        InventorySlots[Index].Combinable = false;
        InventorySlots[Index].Stackable = false;

        if (InventorySlots[Index].TakeMultipleSlots)
        {
            ItemAmountText[Index].gameObject.SetActive(true);
        }

        InventorySlots[Index].TakeMultipleSlots = false;

        if (ButtonController.SlotButtons[Index].gameObject == EventSystem.current.currentSelectedGameObject)
        {
            ButtonController.SetEquipText(EventSystem.current.currentSelectedGameObject);
            ButtonController.SetCombineInteractivity(EventSystem.current.currentSelectedGameObject);
        }
    }
}
