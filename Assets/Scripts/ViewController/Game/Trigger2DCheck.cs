using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    public class Trigger2DCheck : MonoBehaviour
    {
        public LayerMask TargetLayer;

        public int EnterCount;
        private bool triggered = false;
        public bool Triggered => triggered;
        
        private void Update()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0.6f,0,0), Vector2.right, 1f);
            Debug.DrawLine(transform.position + new Vector3(0.6f,0,0), transform.position + new Vector3(1.6f, 0, 0),Color.yellow);
            if (hit.collider)
            {
                if (IsInLayerMask(hit.collider.gameObject, TargetLayer))
                    triggered = true;
                else triggered = false;
            }
        }

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (IsInLayerMask(collision.gameObject, TargetLayer))
        //    {
        //        EnterCount++;
        //    }
        //}

        //private void OnTriggerExit2D(Collider2D collision)
        //{
        //    if (IsInLayerMask(collision.gameObject, TargetLayer))
        //    {
        //        EnterCount--;
        //    }
        //}

        bool IsInLayerMask(GameObject obj, LayerMask mask)
        {
            //obj的layer属性是一个int值，通过移位运算将它转化成运行时物理计算需要的LayerMask值
            //比如obj.layer = 7，则对应的layerMask值为：00000000000000000000000010000000
            var layerMaskofObj = 1 << obj.layer;
            //然后通过逻辑位运算来进行匹配，如果相同则为1，否则为0
            return (layerMaskofObj & mask) > 0;
        }
    }
}