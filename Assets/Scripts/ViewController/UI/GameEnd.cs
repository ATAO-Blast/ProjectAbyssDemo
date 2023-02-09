using QFramework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AbyssDemo
{
    public class GameEnd : MonoBehaviour,IController
    {
        public GameObject gameEndPanel;
        // Start is called before the first frame update
        void Start()
        {
            TypeEventSystem.Global.Register<OnBattleEnd>(e =>
            {
                gameEndPanel.SetActive(true);
            });
        }

        public void ReStart()
        {
            var gos = GameObject.FindGameObjectsWithTag("Enemy");
            if (gos != null)
            {
                foreach (var go in gos)
                {
                    var mUnit = this.GetModel<IRuntimeUnitModel>().GetRuntimeUnit(go.GetHashCode());
                    mUnit.OnDestroy.Trigger();
                }
            }
            var gos2 = GameObject.FindGameObjectsWithTag("Hero");
            if (gos2 != null)
            {
                foreach (var go in gos2)
                {
                    var mUnit = this.GetModel<IRuntimeUnitModel>().GetRuntimeUnit(go.GetHashCode());
                    mUnit.OnDestroy.Trigger();
                }
            }
            gameEndPanel.SetActive(false);
        }
        public void Quit()
        {
            Application.Quit();
        }

        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }
    }
}