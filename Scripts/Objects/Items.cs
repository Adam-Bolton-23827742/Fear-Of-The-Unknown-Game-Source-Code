using UnityEngine;

public class Items : MonoBehaviour
{
    private Inventory Inventory;
    private bool IsTriggering = false;
    [SerializeField] private ItemScriptableObjects ItemObject;
    public int Amount;
    private void Start()
    {
        Inventory = GameObject.Find("Player").GetComponent<Inventory>();
    }

    private void Update()
    {
        // If the player is colliding with the object, is not aiming, and left clicks
        if (IsTriggering && Input.GetAxis("Aim") <= 0 && Input.GetButtonDown("Shoot")
            || IsTriggering && !Input.GetButton("Aim") && Input.GetButtonDown("Shoot"))
        {
            // If the object can be stacked in an inventory slot
            if (ItemObject.IsStackable)
            {
                // Find another potential instance of the same object type to stack with
                Inventory.FindStackableSlot(ItemObject, gameObject);
                return;
            }
            
            // Looks for the first empty slot if the object is not stackable
            Inventory.FindEmptySlot(ItemObject, gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if the object collides with the player
        if (other.gameObject.tag == "Player")
        {
            // Using a boolean instead of OnTriggerStay due to that function being bugged on this Unity version, plus it also improves responsiveness
            IsTriggering = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Collision with the player stops
            IsTriggering = false;
        }
    }
}
