using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    [SerializeField] private string DialogueText;
    private Dialogue Dialogue;
    [HideInInspector] public bool Colliding = false;

    private void Start()
    {
        Dialogue = GameObject.Find("Dialogue").GetComponent<Dialogue>();
    }

    private void Update()
    {
        if (Colliding && Input.GetButtonDown("Shoot") && !Dialogue.IsRunning && Time.timeScale == 1.0f)
        {
            StartCoroutine(Dialogue.SetDialogue(DialogueText));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Colliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Colliding = false;
        }
    }
}
