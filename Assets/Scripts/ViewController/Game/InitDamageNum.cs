using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    public class InitDamageNum : MonoBehaviour,IController
    {
        public GameObject damagePrefab;

        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }
        private void Start()
        {
            this.RegisterEvent<OnDamage>(e =>
            {
                var damGo = GameObject.Instantiate<GameObject>(damagePrefab,e.position + new Vector3(0.5f,0.5f),Quaternion.identity, transform);
                var damNum = damGo.GetComponent<DamageNum>();
                damNum.demageNum = e.demageNum;
                damNum.demageColor = e.demageColor;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
    }
}