using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDetector : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Clicked");
            Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);
            // RaycastHit2D can be either true or null, but has an implicit conversion to bool, so we can use it like this
            if (hitInfo)
            {
                Debug.Log(hitInfo.transform.gameObject.name);
                // Here you can check hitInfo to see which collider has been hit, and act appropriately.
                Mark(hitInfo.transform.gameObject);
            }
        }
    }

    void Mark(GameObject node)
    {
        node.GetComponent<SpriteRenderer>().color = Color.blue;
    }
}
