using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    public class InitLine : MonoBehaviour,IController
    {
        public GameObject line;

        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }

        void Start()
        {
            this.RegisterEvent<OnLineRender>(e => 
            {
                var lineGo = GameObject.Instantiate(line,transform);
                var lineRenderer = lineGo.GetComponent<LineRenderer>();
                lineRenderer.SetPosition(0, e.performPos);
                lineRenderer.SetPosition(1, e.targetPos);
                lineRenderer.startColor = Color.white;
                lineRenderer.endColor = e.lineEndColor;
                Destroy(lineGo,0.5f);
            });
        }

        
    }
}