using UnityEngine;
using System.Collections;

public class destroy_obj : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collider)
    {

        
        if (collider.tag == "Box" || collider.tag == "Apple")
        {
            Destroy(collider.gameObject);
        }
    }

}
