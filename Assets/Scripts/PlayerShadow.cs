using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    private float buffer = 0.4f;
    private PlayerController pc;
    private void Start()
    {
        pc = this.transform.parent.GetComponent<PlayerController>();
    }
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector2(gameObject.transform.position.x, pc.BaseY-buffer);
    }
}
