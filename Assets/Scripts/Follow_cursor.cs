using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_cursor : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
    }
}
