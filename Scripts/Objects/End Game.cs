using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    private Enemies Boss;
    [SerializeField] private Dialogue Dialogue;
    [SerializeField] private string DialogueText;
    [HideInInspector] public bool LoadCredits;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Boss = GetComponent<Enemies>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Boss.CurrentHealth <= 0)
        {
            StartCoroutine(Dialogue.SetDialogue(DialogueText));
        }

        if (LoadCredits)
        {
            SceneManager.LoadScene("Credits");
        }
    }
}
