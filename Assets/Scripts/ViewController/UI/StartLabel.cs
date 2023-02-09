using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AbyssDemo
{
    public class StartLabel : MonoBehaviour
    {
        public GameObject label;
        
        void Update()
        {
            if(BattleSystem.Instance.battleStates == BattleSystem.PerformAction.READY)
            {
                label.SetActive(true);
            }
            else { label.SetActive(false); }
        }
    }
}