using QFramework;
using QFramework.Experimental;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Burst.CompilerServices;

namespace AbyssDemo
{
    public class ReadyState : AbstractState<TurnState, BaseFSM>,IController
    {
        public ReadyState(FSM<TurnState> fsm, BaseFSM target) : base(fsm, target) { }

        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }

        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                //this.GetModel<IRuntimeUnitModel>().GetRuntimeUnit(mTarget.gameObject.GetHashCode()).SetStatPosition( mTarget.transform.position);
                mFSM.ChangeState(TurnState.PROCESSING);

            }

        }
    }
    public class ProcessingState : AbstractState<TurnState, BaseFSM>,IController
    {
        private float curTime;
        private RuntimeUnitInfo mRuntimeUnit;
        private ElementBuffSystem mElementBuffSystem;
        bool hasBuff = false;
        public ProcessingState(FSM<TurnState> fsm, BaseFSM target) : base(fsm, target) 
        { 
            mRuntimeUnit = this.GetModel<IRuntimeUnitModel>().GetRuntimeUnit(mTarget.gameObject.GetHashCode()); 
            mElementBuffSystem = this.GetSystem<ElementBuffSystem>();
        }

        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }

        protected override void OnEnter()
        {
            curTime = 0;
            var buff = mElementBuffSystem.GetUnitBuffDic(mRuntimeUnit);
            if (buff != null) { hasBuff= true; }
            else { hasBuff = false; }
        }
        
        protected override void OnUpdate()
        {
            if (BattleSystem.Instance.battleStates == BattleSystem.PerformAction.PERFORMACTION)
            {

            }
            else
            {
                if(hasBuff) this.GetSystem<ElementBuffSystem>().AddingBuffTime(mTarget, mFSM,mRuntimeUnit);
                float speed = mRuntimeUnit.Speed.Value/100;
                curTime += Time.deltaTime * speed;
                float calCooldown = curTime / 2f;
                mRuntimeUnit.CurActionTime.Value = Mathf.Clamp01(calCooldown);
                if (curTime > 2f)
                {
                    BattleTurn battleTurn = new BattleTurn(mRuntimeUnit);
                    BattleSystem.Instance.AddPerformList(battleTurn);
                    mFSM.ChangeState(TurnState.WAITING);
                }
            }
        }
    }
    public class ChooseActionState : AbstractState<TurnState, BaseFSM>, IController
    {
        public ChooseActionState(FSM<TurnState> fsm, BaseFSM target) : base(fsm, target)
        {
        }

        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }

        protected override void OnEnter()
        {
            #region Old
            //if (mTarget.gameObject.tag == "Enemy")
            //{
            //    var heroList = BattleSystem.Instance.HerosInBattle;
            //    if (heroList.Count > 0)
            //    {
            //        var threadvalue = 0;
            //        for (int i = 0; i < heroList.Count; i++)
            //        {
            //            if (heroList[i].GetComponent<BaseFSM>().threat >= threadvalue)
            //                threadvalue = heroList[i].GetComponent<BaseFSM>().threat;
            //        }
            //        var targetList = heroList.Where(go=>go.GetComponent<BaseFSM>().threat == threadvalue).ToList();



            //        mTarget.GoTarget = targetList[Random.Range(0, targetList.Count)];
            //        BattleTurn battleTurn = new BattleTurn(mTarget.gameObject, mTarget.GoTarget);
            //        BattleSystem.Instance.CollectActions(battleTurn);
            //    }
            //}
            //if (mTarget.gameObject.tag == "Hero")
            //{
            //    var enemyList = BattleSystem.Instance.EnemysInBattle;
            //    if (enemyList.Count > 0)
            //    {
            //        var threadvalue = 0;
            //        for (int i = 0; i < enemyList.Count; i++)
            //        {
            //            if (enemyList[i].GetComponent<BaseFSM>().threat >= threadvalue)
            //                threadvalue = enemyList[i].GetComponent<BaseFSM>().threat;
            //        }
            //        var targetList = enemyList.Where(go => go.GetComponent<BaseFSM>().threat == threadvalue).ToList();

            //        mTarget.GoTarget = targetList[Random.Range(0, targetList.Count)];
            //        BattleTurn battleTurn = new BattleTurn(mTarget.gameObject, mTarget.GoTarget);
            //        BattleSystem.Instance.CollectActions(battleTurn);
            //    }
            //}
            //mFSM.ChangeState(TurnState.WAITING);
            #endregion
            //this.GetSystem<ChooseSkillSystem>().ChooseActionBySkill(mTarget, mFSM);
        }
    }
    public class WaitingState : AbstractState<TurnState, BaseFSM>
    {
        public WaitingState(FSM<TurnState> fsm, BaseFSM target) : base(fsm, target)
        {

        }
        protected override bool OnCondition()
        {
            return mFSM.CurrentStateId == TurnState.PROCESSING;
        }

    }
    public class ActionState : AbstractState<TurnState, BaseFSM>,IController
    {
        private bool actionStarted = false;
        public ActionState(FSM<TurnState> fsm, BaseFSM target) : base(fsm, target)
        {
        }

        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }

        protected override void OnEnter()
        {
            this.GetSystem<ChooseSkillSystem>().ChooseActionBySkill(mTarget, mFSM);
        }
        protected override void OnUpdate()
        {
            var runtimeUnitModel = this.GetModel<IRuntimeUnitModel>();
            var chooseSkillSystem = this.GetSystem<ChooseSkillSystem>();
            var mRuntimeUnit = runtimeUnitModel.GetRuntimeUnit(mTarget.gameObject.GetHashCode());
            var frozenBuff = this.GetSystem<ElementBuffSystem>().GetUnitBuff(mRuntimeUnit,ElementBuffType.Frozen);
            var confusionBuff = this.GetSystem<ElementBuffSystem>().GetUnitBuff(mRuntimeUnit,ElementBuffType.Confusion);
            if (frozenBuff != null)
            {
                mRuntimeUnit.ClearGoTarget();
                BattleSystem.Instance.RemovePerformList(0);
                BattleSystem.Instance.battleStates = BattleSystem.PerformAction.WAIT;

                mFSM.ChangeState(TurnState.PROCESSING);
            }
            else if (confusionBuff != null)
            {
                mTarget.StartCoroutine(ConfusionBuffAction(mRuntimeUnit, chooseSkillSystem));
            }
            else this.GetSystem<ChooseSkillSystem>().CheckedSkill.SkillPerform(mTarget, mFSM);
        }
        protected override void OnExit()
        {
            var elementBuffSystem = this.GetSystem<ElementBuffSystem>();
            var runtimeUnitInfo = this.GetModel<IRuntimeUnitModel>().GetRuntimeUnit(mTarget.gameObject.GetHashCode());
            var unitBuffDic = elementBuffSystem.GetUnitBuffDic(runtimeUnitInfo);
            if(unitBuffDic != null)
            {
                if(unitBuffDic.ContainsKey(ElementBuffType.Explosion))
                {
                    unitBuffDic.Remove(ElementBuffType.Explosion);Debug.Log("ExplosionRemoved");
                }
                if(unitBuffDic.ContainsKey(ElementBuffType.Electrifying))
                {
                    unitBuffDic.Remove(ElementBuffType.Electrifying); Debug.Log("EletrifyingRemoved");
                }
                if(unitBuffDic.ContainsKey(ElementBuffType.SuperConduct))
                {
                    unitBuffDic.Remove(ElementBuffType.SuperConduct);Debug.Log("SuperConductRemoved");
                }
            }
        }
        IEnumerator ConfusionBuffAction(RuntimeUnitInfo mRuntimeUnit,ChooseSkillSystem chooseSkillSystem)
        {
            if (actionStarted) { yield break; }
            actionStarted = true;
            RuntimeUnitInfo goTar = null;
            var checkNum = Random.Range(0, 1);
            if (checkNum == 0)//Enemy
            {
                var mList = new List<RuntimeUnitInfo>(BattleSystem.Instance.EnemysUnitInfo);
                if (mList.Count > 0)
                {
                    if (mRuntimeUnit.Group == "Enemy") mList.Remove(mRuntimeUnit);
                    if (mList.Count != 0)
                    {
                        var goTarget = chooseSkillSystem.ChooseTargetByThreat(mList);
                        //仇恨重新鉴定
                        goTarget = chooseSkillSystem.ReChooseTargetByRidicule(mList, goTarget);
                        //......
                        goTar = goTarget;
                    }
                }
            }
            if (checkNum == 1)//Hero
            {
                var mList = new List<RuntimeUnitInfo>(BattleSystem.Instance.HerosUnitInfo);
                if (mList.Count > 0)
                {
                    if (mRuntimeUnit.Group == "Hero") mList.Remove(mRuntimeUnit);
                    if (mList.Count != 0)
                    {
                        var goTarget = chooseSkillSystem.ChooseTargetByThreat(mList);
                        //仇恨重新鉴定
                        goTarget = chooseSkillSystem.ReChooseTargetByRidicule(mList, goTarget);
                        //......
                        goTar = goTarget;
                    }
                }
            }
            if (goTar != null)
            {
                yield return new WaitForSeconds(0.5f);
                var hit = mRuntimeUnit.Atk - goTar.Def;
                TypeEventSystem.Global.Send(new OnHitOccur() { hitInfo = $"{mRuntimeUnit.Name}对{goTar.Name}造成了{hit}点伤害\n" });
                chooseSkillSystem.SendEvent(new OnDamage() { demageColor = Color.red, demageNum = Mathf.FloorToInt(hit + 0.5f), position = goTar.StartPosition });

                chooseSkillSystem.SendEvent(new OnLineRender() { performPos = mRuntimeUnit.StartPosition, targetPos = goTar.StartPosition, lineEndColor = Color.red });

                goTar.HitHp(hit);

                mRuntimeUnit.ClearGoTarget();
                BattleSystem.Instance.RemovePerformList(0);
                BattleSystem.Instance.battleStates = BattleSystem.PerformAction.WAIT;
                actionStarted = false;
                mFSM.ChangeState(TurnState.PROCESSING);
            }
            else
            {
                mRuntimeUnit.ClearGoTarget();
                BattleSystem.Instance.RemovePerformList(0);
                BattleSystem.Instance.battleStates = BattleSystem.PerformAction.WAIT;
                actionStarted = false;
                mFSM.ChangeState(TurnState.PROCESSING);
            }
        }
        #region Old
        //IEnumerator DoingAction()
        //{
        //    if (actionStarted) { yield break; }
        //    actionStarted = true;
        //    //if (!BattleSystem.Instance.CheckPerformList(mTarget.GoTarget))
        //    //{
        //    //    BattleSystem.Instance.battleStates = BattleSystem.PerformAction.WAIT;

        //    //    actionStarted = false;
        //    //    curCooldown = 0;
        //    //    UIGo.SetProcessingBar(0);
        //    //    actionFinished = true;
        //    //    currentState = TurnState.PROCESSING;
        //    //    yield return null;
        //    //}
        //    Vector2 targetPosition = Vector2.zero;
        //    if (mTarget.gameObject.tag == "Enemy") targetPosition = new Vector2(mTarget.GoTarget.transform.position.x, mTarget.GoTarget.transform.position.y);
        //    else if (mTarget.gameObject.tag == "Hero") targetPosition = new Vector2(mTarget.GoTarget.transform.position.x, mTarget.GoTarget.transform.position.y);

        //    //while (MoveTowardsTarget(targetPosition))
        //    //{
        //    //    yield return null;
        //    //}
        //    Debug.DrawLine(mTarget.transform.position, targetPosition, Color.red, 0.5f);

        //    yield return new WaitForSeconds(1f);
        //    //do demage
        //    var demage = Random.Range(10, 30);
        //    var targetFSM = mTarget.GoTarget.GetComponent<BaseFSM>();
        //    targetFSM.HitHP(demage);

        //    //Call Damage Event
        //    //TMPText.text += $"{gameObject.name}对{goTarget.name}造成了{demage}伤害\n";
        //    if (targetFSM.curHP < 0 || Mathf.Approximately(targetFSM.curHP, 0))
        //    {
        //        targetFSM.SimpleUnitFSM.ChangeState(TurnState.DEAD);
        //    }

        //    //Vector3 firstPosition = mTarget.StartPosition;
        //    //while (MoveTowardsTarget(startPosition))
        //    //{
        //    //    yield return null;
        //    //}

        //    BattleSystem.Instance.RemovePerformList(0);
        //    BattleSystem.Instance.battleStates = BattleSystem.PerformAction.WAIT;

        //    mFSM.ChangeState(TurnState.PROCESSING);
        //    actionStarted = false;

        //}
        //bool MoveTowardsTarget(Vector3 target)
        //{
        //    var mPos = mTarget.gameObject.transform.position;
        //    var mMoveSpeed = mTarget.baseUnit.moveSpeed;
        //    return target != (mPos = Vector3.MoveTowards(mPos, target, mMoveSpeed * Time.deltaTime));
        //}
        #endregion
    }

    public class DeadState : AbstractState<TurnState, BaseFSM>
    {
        public DeadState(FSM<TurnState> fsm, BaseFSM target) : base(fsm, target)
        {
        }
        protected override void OnEnter()
        {
            if (mTarget.gameObject.tag == "Enemy")
            {
                BattleSystem.Instance.RemoveEnemyList(mTarget.gameObject);
            }
            if (mTarget.gameObject.tag == "Hero")
            {
                BattleSystem.Instance.RemoveHeroList(mTarget.gameObject);
            }
            //Debug.Log(mTarget.GetComponent<BaseFSM>().threat + mTarget.gameObject.tag);
            //GameObject.Destroy(mTarget.gameObject);
        }
    }
}