using UnityEngine;
using UnityEngine.SceneManagement; // [WAJIB] Ditambahkan untuk pindah scene

public class Interactable : MonoBehaviour, IInteractable
{
    public string SceneName;

    public void Interact()
    {
        Debug.Log("Scene Changed to " + SceneName);
        SceneManager.LoadScene(SceneName);
    }
}