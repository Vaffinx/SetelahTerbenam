using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    public PlayableDirector pd;
    private PlayerMovment player;
    public GameObject[] cameraBrain;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponent<BoxCollider2D>().enabled = false;
            pd.Play();
            
            foreach(GameObject Camera in cameraBrain)
            {
                Camera.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach(GameObject Camera in cameraBrain)
            {
                Camera.SetActive(false);
            }
        }
    }
}
