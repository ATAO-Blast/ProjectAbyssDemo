using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    public class CellCheckUpper : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        // Start is called before the first frame update
        void Start()
        {
            spriteRenderer= GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, 20);
            if (hit.collider)
            {
                spriteRenderer.color = Color.red;
            }
        }
    }
}