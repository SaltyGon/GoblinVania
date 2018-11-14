using UnityEngine;
using System.Collections;

public class scr_Chest : MonoBehaviour
{

    private bool activated = false;
    public Sprite opened;
    public bool end;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!activated && !end)
            {
                collision.gameObject.GetComponent<scr_CubeControl>().NewCheckpoint();
                GetComponent<SpriteRenderer>().sprite = opened;
                activated = true;
            }
            else if (!activated && end)
            {
                collision.gameObject.GetComponent<scr_CubeControl>().NextLevel();
            }
        }
    }
}
