using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace AbyssDemo
{
    public class DamageNum : MonoBehaviour
    {
        public TextMeshPro textMeshPro;
        public int demageNum;
        public Color demageColor;
        // Start is called before the first frame update
        void Start()
        {
            textMeshPro.color = demageColor;
            textMeshPro.text = demageNum.ToString();
        }
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}