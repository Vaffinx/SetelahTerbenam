using UnityEngine;

public class Pause : MonoBehaviour
{
    public void paused()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
    }

    public void unpaused()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
    }
}
