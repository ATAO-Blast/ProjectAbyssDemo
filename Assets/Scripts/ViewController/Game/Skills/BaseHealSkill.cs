using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.Linq;

namespace AbyssDemo
{
    public class BaseHealSkill : UnitSkill
    {
        public BaseHealSkill(string name, int priority, SkillTarget skillTarget, SkillSelect skillSelect, SkillTargetState skillTargetState, SkillDmgType skillDmgType, ElementType skillElementType, float attackMul = 1) : base(name, priority, skillTarget, skillSelect, skillTargetState, skillDmgType, skillElementType, attackMul)
        {
        }
        public override bool SkillPreCheck(BaseFSM mTarget, FSM<TurnState> mFSM)
        {
            if (mTarget.gameObject.tag == "Enemy")
            {
                var enemyInfoList = BattleSystem.Instance.EnemysUnitInfo;
                if (enemyInfoList.Any(enemy => enemy.HealthState.Value == SkillTargetState.CanHeal)) return true;
                
                
            }
            else if (mTarget.gameObject.tag == "Hero")
            {
                var heroInfoList = BattleSystem.Instance.HerosUnitInfo;

                if(heroInfoList.Any(hero => hero.HealthState.Value == SkillTargetState.CanHeal)) return true;
            }
            return false;
        }
        public override void SkillChooseTarget(BaseFSM mTarget, FSM<TurnState> mFSM)
        {
            var runtimeUnitModel = this.GetModel<IRuntimeUnitModel>();
            var chooseSkillSystem = this.GetSystem<ChooseSkillSystem>();
            var mRuntimeUnit = runtimeUnitModel.GetRuntimeUnit(mTarget.gameObject.GetHashCode());
            if (mTarget.gameObject.tag == "Enemy")
            {
                var enemyInfoList = BattleSystem.Instance.EnemysUnitInfo;

                if (enemyInfoList.Count > 0)
                {
                    var goTarget = enemyInfoList.OrderBy(enemy => enemy.CurHP.Value).FirstOrDefault();   
                    mRuntimeUnit.AddGoTarget(goTarget);
                    //BattleTurn battleTurn = new BattleTurn(mRuntimeUnit);
                    //BattleSystem.Instance.AddPerformList(battleTurn);
                }
            }
            if (mTarget.gameObject.tag == "Hero")
            {
                var heroInfoList = BattleSystem.Instance.HerosUnitInfo;

                if (heroInfoList.Count > 0)
                {
                    var goTarget = heroInfoList.OrderBy(hero => hero.CurHP.Value).FirstOrDefault();
                    mRuntimeUnit.AddGoTarget(goTarget);
                    //BattleTurn battleTurn = new BattleTurn(mRuntimeUnit);
                    //BattleSystem.Instance.AddPerformList(battleTurn);
                }
            }
            //mFSM.ChangeState(TurnState.WAITING);
        }


        public override void SkillPerform(BaseFSM mTarget, FSM<TurnState> mFSM)
        {
            this.GetSystem<PerformSkillSystem>().HealOnceAction(mTarget, mFSM);
        }
    }
}