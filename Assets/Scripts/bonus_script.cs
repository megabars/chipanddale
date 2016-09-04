using UnityEngine;
using System.Collections;


public class bonus_script : MonoBehaviour
{
    

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<AudioSource>().Play();
            Destroy(gameObject);
        }
    }
}
