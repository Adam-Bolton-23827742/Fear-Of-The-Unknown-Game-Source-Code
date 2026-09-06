using System.Collections;
using UnityEngine;

public class UseHammer : MonoBehaviour
{
    private ToggleInventoryMenu ToggleInventoryMenu;
    [HideInInspector] public bool BreakPlank = false, IsColliding = false;
    [SerializeField] private GameObject LockedImage;
    [SerializeField] private GameObject[] Planks;
    private AudioSource AudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ToggleInventoryMenu = GameObject.Find("Player").GetComponent<ToggleInventoryMenu>();
        AudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (BreakPlank)
        {
            StartCoroutine(BreakPlanks());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            IsColliding = true;
            LockedImage.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            IsColliding = false;
        }
    }

    private IEnumerator BreakPlanks()
    {
        BreakPlank = false;

        // break the plank if the player is near the plank and uses the hammer
        ToggleInventoryMenu.LoadInventory(false);

        AudioSource.Play();

        yield return new WaitUntil(() => !AudioSource.isPlaying);

        foreach (GameObject Plank in Planks)
        {
            Destroy(Plank);
        }
    }
}
