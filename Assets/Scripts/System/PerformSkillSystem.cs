using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    public class PerformSkillSystem : AbstractSystem
    {
        private bool actionStarted = false;
        private IRuntimeUnitModel runtimeUnitModel;
        private HitUnitSystem hitUnitSystem;
        protected override void OnInit()
        {
            runtimeUnitModel = this.GetModel<IRuntimeUnitModel>();
            hitUnitSystem= this.GetSystem<HitUnitSystem>();
        }
        public void HitOnceAction(BaseFSM mTarget, FSM<TurnState> mFSM)
        {
            mTarget.StartCoroutine(DoingAction(mTarget, mFSM));
        }
        public void HitMultipleTimesAction(BaseFSM mTarget, FSM<TurnState> mFSM,int times)
        {
            mTarget.StartCoroutine(DoingMultipleHitAction(mTarget, mFSM, times));
        }
        public void HealOnceAction(BaseFSM mTarget,FSM<TurnState> mFSM)
        {
            mTarget.StartCoroutine(DoingHealAction(mTarget, mFSM));
        }
        IEnumerator DoingAction(BaseFSM mTarget, FSM<TurnState> mFSM)
        {
            if (actionStarted) { yield break; }
            actionStarted = true;
            var mRuntimeUnit = runtimeUnitModel.GetRuntimeUnit(mTarget.gameObject.GetHashCode());
            var currentTargets = CheckPerformTargetsIsExist(mRuntimeUnit, mFSM);
            if(currentTargets == null) { yield return null; }
            else
            {
                List<Vector2> targetPositions = new List<Vector2>(currentTargets.Count);
                foreach (var currentTarget in currentTargets)
                {
                    targetPositions.Add(currentTarget.StartPosition);
                }
                foreach (var target in targetPositions)
                {
                    Debug.DrawLine(mRuntimeUnit.StartPosition, target, Color.red, 0.5f);
                }
                yield return new WaitForSeconds(0.5f);

                //var demage = mRuntimeUnit.Atk;
                foreach (var currentTarget in currentTargets)
                {
                    hitUnitSystem.HitUnit(mRuntimeUnit, currentTarget);
                    //mTarget.SendCommand<HitCommand>(new HitCommand(mRuntimeUnit, currentTarget));
                }

                mRuntimeUnit.ClearGoTarget();
                BattleSystem.Instance.RemovePerformList(0);
                BattleSystem.Instance.battleStates = BattleSystem.PerformAction.WAIT;

                mFSM.ChangeState(TurnState.PROCESSING);
                actionStarted = false;
            }

        }
        IEnumerator DoingMultipleHitAction(BaseFSM mTarget,FSM<TurnState> mFSM,int times)
        {
            if (actionStarted) { yield break; }
            actionStarted = true;
            var chooseSkillSystem = this.GetSystem<ChooseSkillSystem>();
            var mRuntimeUnit = runtimeUnitModel.GetRuntimeUnit(mTarget.gameObject.GetHashCode());

            int time = 1;
            while (time <= times)
            {
                RuntimeUnitInfo goTar = null;
                if (mTarget.gameObject.tag == "Enemy")
                {
                    var heroInfoList = BattleSystem.Instance.HerosUnitInfo;

                    if (heroInfoList.Count > 0)
                    {
                        var goTarget = chooseSkillSystem.ChooseTargetByThreat(heroInfoList);
                        //仇恨重新鉴定
                        goTarget = chooseSkillSystem.ReChooseTargetByRidicule(heroInfoList, goTarget);
                        //......
                        goTar = goTarget;
                    }
                }
                if (mTarget.gameObject.tag == "Hero")
                {
                    var enemyInfoList = BattleSystem.Instance.EnemysUnitInfo;

                    if (enemyInfoList.Count > 0)
                    {
                        var goTarget = chooseSkillSystem.ChooseTargetByThreat(enemyInfoList);
                        //仇恨重新鉴定
                        goTarget = chooseSkillSystem.ReChooseTargetByRidicule(enemyInfoList, goTarget);
                        //......
                        goTar= goTarget;
                    }
                }
                if (goTar == null)
                {
                    mRuntimeUnit.ClearGoTarget();
                    BattleSystem.Instance.RemovePerformList(0);
                    BattleSystem.Instance.battleStates = BattleSystem.PerformAction.WAIT;

                    Debug.Log(mRuntimeUnit.Threat + mRuntimeUnit.Name + "在SkillPerform时没有找到有效目标");
                    mFSM.ChangeState(TurnState.PROCESSING);
                    actionStarted = false;
                    yield return null;
                }

                else
                {
                    Debug.DrawLine(mRuntimeUnit.StartPosition, goTar.StartPosition, Color.red, 0.5f);
                    yield return new WaitForSeconds(0.5f);
                    hitUnitSystem.HitUnit(mRuntimeUnit,goTar);
                    //mTarget.SendCommand(new HitCommand(mRuntimeUnit, goTar));
                }
                time += 1;
                yield return null;
            }
            mRuntimeUnit.ClearGoTarget();
            BattleSystem.Instance.RemovePerformList(0);
            BattleSystem.Instance.battleStates = BattleSystem.PerformAction.WAIT;

            mFSM.ChangeState(TurnState.PROCESSING);
            actionStarted = false;
        }
        IEnumerator DoingHealAction(BaseFSM mTarget, FSM<TurnState> mFSM)
        {
            if (actionStarted) { yield break; }
            actionStarted = true;
            var mRuntimeUnit = runtimeUnitModel.GetRuntimeUnit(mTarget.gameObject.GetHashCode());
            var currentTargets = CheckPerformTargetsIsExist(mRuntimeUnit, mFSM);
            if (currentTargets == null) { yield return null; }
            else
            {
                List<Vector2> targetPositions = new List<Vector2>(currentTargets.Count);
                foreach (var currentTarget in currentTargets)
                {
                    targetPositions.Add(currentTarget.StartPosition);
                }
                foreach (var target in targetPositions)
                {
                    Debug.DrawLine(mRuntimeUnit.StartPosition, target, Color.green, 0.5f);
                }
                yield return new WaitForSeconds(1);

                var healing = mRuntimeUnit.Atk;
                foreach (var currentTarget in currentTargets)
                {
                    currentTarget.HealHp(healing);
                }

                mRuntimeUnit.ClearGoTarget();
                BattleSystem.Instance.RemovePerformList(0);
                BattleSystem.Instance.battleStates = BattleSystem.PerformAction.WAIT;

                mFSM.ChangeState(TurnState.PROCESSING);
                actionStarted = false;
            }

        }
        List<RuntimeUnitInfo> CheckPerformTargetsIsExist(RuntimeUnitInfo mRuntimeUnit,FSM<TurnState> mFSM) 
        {
            var currentTargets = new List<RuntimeUnitInfo>();
            foreach (var goTarget in mRuntimeUnit.GoTargets)
            {
                if (runtimeUnitModel.GetRuntimeUnit(goTarget.HashKey) != null)
                {
                    currentTargets.Add(goTarget);
                }
            }
            if (currentTargets.Count == 0)
            {
                mRuntimeUnit.ClearGoTarget();
                BattleSystem.Instance.RemovePerformList(0);
                BattleSystem.Instance.battleStates = BattleSystem.PerformAction.WAIT;

                Debug.Log(mRuntimeUnit.Threat + mRuntimeUnit.Name + "在SkillPerform时没有找到有效目标");
                mFSM.ChangeState(TurnState.PROCESSING);
                actionStarted= false;
                return null;
            }
            return currentTargets;
        }
    }
}