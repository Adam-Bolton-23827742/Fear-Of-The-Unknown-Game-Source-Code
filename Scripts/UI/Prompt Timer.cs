using System.Collections;
using UnityEngine;

public class PromptTimer : MonoBehaviour
{
    [SerializeField] private float Time;
    private void OnEnable()
    {
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(Time);
        gameObject.SetActive(false);
    }
}
