using UnityEngine;

public class DiscoverShotgunRoom : MonoBehaviour
{
    [SerializeField] private GameObject RoomImage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            RoomImage.SetActive(true);
        }
    }
}
