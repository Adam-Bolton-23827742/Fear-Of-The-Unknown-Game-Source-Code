using TMPro;
using UnityEngine;

public class InventoryItemAmount : MonoBehaviour
{
    private TextMeshProUGUI ItemAmount;
    [SerializeField] private int Slot;
    [SerializeField] private Inventory Inventory;
    [SerializeField] private Transform RightHand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ItemAmount = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Inventory.InventorySlots[Slot].ItemType == "Weapon")
        {
            foreach (Transform Item in RightHand)
            {
                if (Item.name == Inventory.InventorySlots[Slot].ItemName)
                {
                    ItemAmount.text = Item.gameObject.GetComponent<Shooting>().CurrentAmmo.ToString();
                    break;
                }
            }
        }

        // Convert the amount of items in the referenced inventory slot to text. Only if the number of items is greater than 1.
        else if (Inventory.InventorySlots[Slot].Amount > 1)
        {
            ItemAmount.text = Inventory.InventorySlots[Slot].Amount.ToString();
        }

        else
        {
            // Set the amount invisible if the player only has 0 or 1 items in the inventory slot
            ItemAmount.text = null;
        }
    }
}
