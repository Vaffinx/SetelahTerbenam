using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
    public string Scene;

    public void ChangeScene()
    {
        SceneManager.LoadScene(Scene);
        Debug.Log("Scene changed to " + Scene);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game exited");
    }
}
