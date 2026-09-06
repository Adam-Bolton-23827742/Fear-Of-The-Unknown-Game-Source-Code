using Unity.Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera CurrentCamera;
    [SerializeField] private Camera[] Cameras;
    [SerializeField] private Camera[] MirrorCam;
    [SerializeField] private Doors BossDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (BossDoor != null)
            {
                BossDoor.IsLocked = true;
            }

            foreach (Camera Cam in Cameras)
            {
                Cam.gameObject.SetActive(false);
            }

            // Switch camera when the player enters the trigger box
            foreach (Camera Cam in MirrorCam)
            {
                Cam.gameObject.SetActive(true);
            }
                
            CurrentCamera.Prioritize();
        }
    }
}
