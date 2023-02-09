using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    public class BuffHashSetTest : MonoBehaviour
    {
        private HashSet<ElementBuffType> _buffs;

        private void Start()
        {
            _buffs = new HashSet<ElementBuffType>();
        }
        private void OnGUI()
        {
            if(GUILayout.Button("Add Burning"))
            {
                _buffs.Add(ElementBuffType.Burning);
            }
            if (GUILayout.Button("Add Confusion"))
            {
                _buffs.Add(ElementBuffType.Confusion);
            }
            if (GUILayout.Button("Debug Output"))
            {
                foreach(ElementBuffType type in _buffs)
                {
                    Debug.Log(type.ToString());
                }
            }
        }
    }
}