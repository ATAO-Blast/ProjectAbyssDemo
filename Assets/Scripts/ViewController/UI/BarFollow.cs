using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AbyssDemo
{
    public class BarFollow : MonoBehaviour
    {
        public GameObject goToFollow;
        public Image ProgerssBar;
        public Image HPBar;
        public Image ElementState;

        // Update is called once per frame
        void Update()
        {
            if(goToFollow != null)
            {
                var mPosition = Camera.main.WorldToScreenPoint(goToFollow.transform.position + new Vector3(0,0.5f,0));
                var nPosition = new Vector3(mPosition.x, mPosition.y, transform.position.z);
                transform.position = nPosition;
                //var unitFSM = goToFollow.GetComponent<BaseFSM>();
                //ProgerssBar.fillAmount = unitFSM.curActionTime;
                
            }
        }
    }
}