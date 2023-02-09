using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.Linq;

namespace AbyssDemo
{
    public class BasicUnitSkill : UnitSkill
    {
        public BasicUnitSkill(string name, int priority, SkillTarget skillTarget, SkillSelect skillSelect, SkillTargetState skillTargetState, SkillDmgType skillDmgType, ElementType skillElementType, float attackMul = 1) : base(name, priority, skillTarget, skillSelect, skillTargetState, skillDmgType, skillElementType, attackMul)
        {

        }
        public override bool SkillPreCheck(BaseFSM mTarget, FSM<TurnState> mFSM)
        {
            return base.SkillPreCheck(mTarget, mFSM);
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
                    var goTarget = chooseSkillSystem.ChooseTargetByThreat(heroInfoList);
                    //仇恨重新鉴定
                    goTarget = chooseSkillSystem.ReChooseTargetByRidicule(heroInfoList, goTarget);
                    //......
                    mRuntimeUnit.AddGoTarget(goTarget);
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
                    mRuntimeUnit.AddGoTarget(goTarget);
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