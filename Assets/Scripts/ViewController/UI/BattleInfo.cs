using System.Collections;
using UnityEngine;
using TMPro;
using QFramework;

namespace AbyssDemo
{
    public class BattleInfo : MonoBehaviour
    {
        public TextMeshProUGUI textMeshProUGUI;
        // Use this for initialization
        void Start()
        {
            TypeEventSystem.Global.Register<OnHitOccur>(e =>
            {
                textMeshProUGUI.text += e.hitInfo;
            });
        }
        
    }
}