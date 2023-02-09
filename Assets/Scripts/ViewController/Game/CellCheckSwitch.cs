using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    public class CellCheckSwitch : MonoBehaviour,IController
    {
        private GameObject child1, child2;
        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }

        // Start is called before the first frame update
        void Start()
        {
            child1 = transform.GetChild(0).gameObject;
            child2= transform.GetChild(1).gameObject;
            this.RegisterEvent<OnCellCheck>(e =>
            {
                child1.SetActive(e.enable);
                child2.SetActive(e.enable);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

    }
}