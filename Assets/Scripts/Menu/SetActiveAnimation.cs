using UnityEngine;

public class SetActiveAnimation : MonoBehaviour
{
    private Animator anim;
    public GameObject[] UI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetActive()
    {
        anim.SetTrigger("In");
    }

    public void SetInactive()
    {
        anim.SetTrigger("Out");
    }

    public void SetActiveUI()
    {
        foreach (GameObject ui in UI)
        {
            ui.SetActive(true);
        }
    }

    public void SetInactiveUI()
    {
        foreach (GameObject ui in UI)
        {
            ui.SetActive(false);
        }
    }
}
