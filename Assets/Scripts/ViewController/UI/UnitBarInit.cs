using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AbyssDemo
{
    public class UnitBarInit : MonoBehaviour,IController
    {
        private GameObject goInit;
        private IRuntimeUnitModel runtimeUnitModel;
        public GameObject bars;
        public GameObject canvas;
        public Sprite[] elementUISprites;

        // Start is called before the first frame update
        void Start()
        {
            runtimeUnitModel = this.GetModel<IRuntimeUnitModel>();
            TypeEventSystem.Global.Register<OnUnitInitEvent>(e =>
            {
                goInit = e.gameObject;
                OnBarInit(e.gameObject);
            }).UnRegisterWhenGameObjectDestroyed(this);
        }
        private void OnBarInit(GameObject go)
        {
            var nbar = GameObject.Instantiate(bars,canvas.transform).GetComponent<BarFollow>();
            var goRuntimeInfo = runtimeUnitModel.GetRuntimeUnit(go.GetHashCode());
            nbar.goToFollow = goInit;
            goRuntimeInfo.CurHP.Register(hp =>
            {
                var mHp = goRuntimeInfo.CurHP.Value;
                var nHp = goRuntimeInfo.MaxHP.Value;
                float colHp = mHp / nHp;
                nbar.HPBar.fillAmount = Mathf.Clamp01(colHp);
            }).UnRegisterWhenGameObjectDestroyed(nbar.gameObject);

            goRuntimeInfo.CurActionTime.Register(actionTime =>
            {
                nbar.ProgerssBar.fillAmount = actionTime;
            }).UnRegisterWhenGameObjectDestroyed(nbar.gameObject);
            goRuntimeInfo.ElementState.Register(elementState =>
            {
                if(elementState == ElementType.Physics)
                {
                    nbar.ElementState.gameObject.SetActive(false);
                }
                else if(elementState == ElementType.Bolt)
                { 
                    nbar.ElementState.gameObject.SetActive(true);
                    nbar.ElementState.sprite = elementUISprites[0];
                }
                else if (elementState == ElementType.Flame)
                {
                    nbar.ElementState.gameObject.SetActive(true);
                    nbar.ElementState.sprite = elementUISprites[1];
                }
                else if (elementState == ElementType.Frost)
                {
                    nbar.ElementState.gameObject.SetActive(true);
                    nbar.ElementState.sprite = elementUISprites[2];
                }
                else if (elementState == ElementType.Poision)
                {
                    nbar.ElementState.gameObject.SetActive(true);
                    nbar.ElementState.sprite = elementUISprites[3];
                }
                else if (elementState == ElementType.Chaos)
                {
                    nbar.ElementState.gameObject.SetActive(true);
                    nbar.ElementState.sprite = elementUISprites[4];
                }
            }).UnRegisterWhenGameObjectDestroyed(nbar.gameObject);

            goRuntimeInfo.OnDestroy.Register(() => Destroy(nbar.gameObject))
                .UnRegisterWhenGameObjectDestroyed(nbar.gameObject);
            
        }

        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }
    }
}