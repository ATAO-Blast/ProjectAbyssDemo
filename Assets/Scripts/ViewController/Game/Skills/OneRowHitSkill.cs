using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace AbyssDemo
{
    public class OneRowHitSkill : UnitSkill
    {
        public OneRowHitSkill(string name, int priority, SkillTarget skillTarget, SkillSelect skillSelect, SkillTargetState skillTargetState, SkillDmgType skillDmgType, ElementType skillElementType, float attackMul = 1) : base(name, priority, skillTarget, skillSelect, skillTargetState, skillDmgType, skillElementType, attackMul)
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
                    var goTarget = chooseSkillSystem.ChooseTargetByThreat(heroInfoList);
                    //仇恨重新鉴定
                    goTarget = chooseSkillSystem.ReChooseTargetByRidicule(heroInfoList, goTarget);
                    //......
                    var randomGridYIndex = goTarget.GridY[Random.Range(0,goTarget.GridY.Count)];

                    var finalGoTarget = heroInfoList.Where(hero => hero.GridY.Contains(randomGridYIndex)).ToList();

                    finalGoTarget.ForEach(f=> mRuntimeUnit.AddGoTarget(f));
                    //BattleTurn battleTurn = new BattleTurn(mRuntimeUnit);
                    //BattleSystem.Instance.AddPerformList(battleTurn);
                }
            }
            if (mTarget.gameObject.tag == "Hero")
            {
                var enemyInfoList = BattleSystem.Instance.EnemysUnitInfo;

                if (enemyInfoList.Count > 0)
                {
                    var threadvalue = 0;
                    enemyInfoList.ForEach(enemy => { if (enemy.Threat >= threadvalue) threadvalue = enemy.Threat; });

                    var targetList = enemyInfoList.Where(enemy => enemy.Threat == threadvalue).ToList();

                    var goTarget = chooseSkillSystem.ChooseTargetByThreat(enemyInfoList);
                    //仇恨重新鉴定
                    goTarget = chooseSkillSystem.ReChooseTargetByRidicule(enemyInfoList, goTarget);
                    //......
                    var randomGridYIndex = goTarget.GridY[Random.Range(0, goTarget.GridY.Count)];

                    var finalGoTarget = enemyInfoList.Where(enemy => enemy.GridY.Contains(randomGridYIndex)).ToList();

                    finalGoTarget.ForEach(f => mRuntimeUnit.AddGoTarget(f));

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
