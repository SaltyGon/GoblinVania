using UnityEngine;
using System.Collections;

public class scr_KillSnake : MonoBehaviour
{
    public GameObject snake;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }
}
