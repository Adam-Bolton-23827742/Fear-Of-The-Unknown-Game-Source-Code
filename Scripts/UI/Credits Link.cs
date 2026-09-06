using UnityEngine;

public class CreditsLink : MonoBehaviour
{
    [SerializeField] private string Link;

    public void OpenLink()
    {
        Application.OpenURL(Link);
    }
}
