using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    public class HitTwiceSkill : UnitSkill
    {
        public HitTwiceSkill(string name, int priority, SkillTarget skillTarget, SkillSelect skillSelect, SkillTargetState skillTargetState, SkillDmgType skillDmgType, ElementType skillElementType, float attackMul = 1) : base(name, priority, skillTarget, skillSelect, skillTargetState, skillDmgType, skillElementType, attackMul)
        {
        }

        public override void SkillChooseTarget(BaseFSM mTarget, FSM<TurnState> mFSM)
        {
            //var runtimeUnitModel = this.GetModel<IRuntimeUnitModel>();
            //var mRuntimeUnit = runtimeUnitModel.GetRuntimeUnit(mTarget.gameObject.GetHashCode());
            //BattleTurn battleTurn = new BattleTurn(mRuntimeUnit);
            //BattleSystem.Instance.AddPerformList(battleTurn);
            //mFSM.ChangeState(TurnState.WAITING);
        }
        public override void SkillPerform(BaseFSM mTarget, FSM<TurnState> mFSM)
        {
            this.GetSystem<PerformSkillSystem>().HitMultipleTimesAction(mTarget, mFSM,2);
        }
    }
}