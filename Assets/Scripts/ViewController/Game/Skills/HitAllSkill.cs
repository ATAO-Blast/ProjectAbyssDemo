using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    public class HitAllSkill : UnitSkill
    {
        public HitAllSkill(string name, int priority, SkillTarget skillTarget, SkillSelect skillSelect, SkillTargetState skillTargetState, SkillDmgType skillDmgType, ElementType skillElementType, float attackMul = 1) : base(name, priority, skillTarget, skillSelect, skillTargetState, skillDmgType, skillElementType, attackMul)
        {
        }

        public override void SkillChooseTarget(BaseFSM mTarget, FSM<TurnState> mFSM)
        {
            var runtimeUnitModel = this.GetModel<IRuntimeUnitModel>();
            var chooseSkillSystem = this.GetSystem<ChooseSkillSystem>();
            var mRuntimeUnit = runtimeUnitModel.GetRuntimeUnit(mTarget.gameObject.GetHashCode());
            if (mTarget.gameObject.tag == "Enemy")
            {
                var heroInfoList = BattleSystem.Instance.HerosUnitInfo;

                if (heroInfoList.Count > 0)
                {
                    heroInfoList.ForEach(hero => { mRuntimeUnit.AddGoTarget(hero); });
                    
                    //BattleTurn battleTurn = new BattleTurn(mRuntimeUnit);
                    //BattleSystem.Instance.AddPerformList(battleTurn);
                }
            }
            if (mTarget.gameObject.tag == "Hero")
            {
                var enemyInfoList = BattleSystem.Instance.EnemysUnitInfo;

                if (enemyInfoList.Count > 0)
                {
                    enemyInfoList.ForEach(enemy => mRuntimeUnit.AddGoTarget(enemy));
                    //BattleTurn battleTurn = new BattleTurn(mRuntimeUnit);
                    //BattleSystem.Instance.AddPerformList(battleTurn);
                }
            }
            //mFSM.ChangeState(TurnState.WAITING);
        }
        public override void SkillPerform(BaseFSM mTarget, FSM<TurnState> mFSM)
        {
            this.GetSystem<PerformSkillSystem>().HitOnceAction(mTarget, mFSM);
        }
    }
}