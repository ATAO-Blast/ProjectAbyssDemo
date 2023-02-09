using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace AbyssDemo
{
    public class BattleSystem : MonoSingleton<BattleSystem>,IController
    {
        //public GameObject endUI;
        public enum PerformAction
        {
            READY,
            WAIT,
            TAKEACTION,
            PERFORMACTION
        }
        public PerformAction battleStates;

        private IRuntimeUnitModel runtimeUnitModel;

        public List<BattleTurn> performList = new List<BattleTurn>();
        private List<GameObject> herosInBattle = new List<GameObject>();
        private List<RuntimeUnitInfo> herosUnitInfo = new List<RuntimeUnitInfo>();
        public List<GameObject> HerosInBattle => herosInBattle;
        public List<RuntimeUnitInfo> HerosUnitInfo => herosUnitInfo;

        private List<GameObject> enemysInBattle = new List<GameObject>();
        private List<RuntimeUnitInfo> enemysUnitInfo = new List<RuntimeUnitInfo>();
        public List<GameObject> EnemysInBattle => enemysInBattle;
        public List<RuntimeUnitInfo> EnemysUnitInfo => enemysUnitInfo;
        // Start is called before the first frame update
        void Start()
        {
            battleStates = PerformAction.READY;
            runtimeUnitModel = this.GetModel<IRuntimeUnitModel>();
        }

        // Update is called once per frame
        void Update()
        {
            switch (battleStates)
            {
                case PerformAction.READY:
                    if (Input.GetKeyUp(KeyCode.P))
                    {
                        herosInBattle.Clear();
                        herosUnitInfo.Clear();
                        enemysInBattle.Clear();
                        enemysUnitInfo.Clear();
                        performList.Clear();
                        enemysInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
                        foreach (var enemy in enemysInBattle)
                        {
                            enemysUnitInfo.Add(runtimeUnitModel.GetRuntimeUnit(enemy.GetHashCode()));
                        }
                        herosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
                        foreach (var hero in herosInBattle)
                        {
                            herosUnitInfo.Add(runtimeUnitModel.GetRuntimeUnit(hero.GetHashCode()));
                        }
                        battleStates = PerformAction.WAIT;
                    }
                    break;
                case PerformAction.WAIT:
                    if (performList.Count > 0)
                    {
                        battleStates = PerformAction.TAKEACTION;
                    }
                    if (herosInBattle.Count == 0 || enemysInBattle.Count == 0)
                    {
                        //Call OnBattleEnd Event
                        this.GetSystem<ElementBuffSystem>().ClearAllUnitBuffs();
                        TypeEventSystem.Global.Send(new OnBattleEnd());
                        battleStates = PerformAction.READY;
                    }
                    break;
                case PerformAction.TAKEACTION:
                    RuntimeUnitInfo performer = performList[0].GoToAttack;
                    if (this.GetModel<IRuntimeUnitModel>().GetRuntimeUnit(performer.HashKey) != null)
                    {
                        var unitFSM = performer.UnitFSM;
                        
                        unitFSM.ChangeState(TurnState.ACTION);
                        battleStates = PerformAction.PERFORMACTION;
                    }
                    else {
                        RemovePerformList(0);
                        battleStates = PerformAction.WAIT; }
                    break;
                case PerformAction.PERFORMACTION:
                    
                    break;
            }
        }
        public void AddPerformList(BattleTurn battleTurn)
        {
            performList.Add(battleTurn);
        }
        public void RemovePerformList(int index)
        {
            if (performList.Count >0)
            {
                performList.RemoveAt(index);
            }
        }
        public void RemoveHeroList(GameObject hero)
        {
            var heroInfo = runtimeUnitModel.GetRuntimeUnit(hero.GetHashCode());
            herosUnitInfo.Remove(heroInfo);
            herosInBattle.Remove(hero);
        }
        public void RemoveEnemyList(GameObject enemy)
        {
            var enemyInfo = runtimeUnitModel.GetRuntimeUnit(enemy.GetHashCode());
            enemysUnitInfo.Remove(enemyInfo);
            enemysInBattle.Remove(enemy);
        }

        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }
        //public bool CheckPerformList(GameObject gameObject)
        //{
        //    if (performList[0].goToTarget == gameObject) { return true; }
        //    return false;
        //}



    }
}