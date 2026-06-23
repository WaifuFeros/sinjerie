using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [SerializeField] private EventReference clickSound;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        RuntimeManager.PlayOneShot(clickSound);
    }
}