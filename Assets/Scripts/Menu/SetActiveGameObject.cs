using UnityEngine;

public class SetActiveGameObject : MonoBehaviour
{
    public GameObject[] target;

    public void SetActive()
    {
        foreach (GameObject t in target)
        {
            if (t != null) t.SetActive(true);
        }
    }

    public void SetInactive()
    {
        foreach (GameObject t in target)
        {
            if (t != null) t.SetActive(false);
        }
    }
}
