using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.Linq;

namespace AbyssDemo
{
    public class OneColumnHitSkill : UnitSkill
    {
        public OneColumnHitSkill(string name, int priority, SkillTarget skillTarget, SkillSelect skillSelect, SkillTargetState skillTargetState, SkillDmgType skillDmgType, ElementType skillElementType, float attackMul = 1) : base(name, priority, skillTarget, skillSelect, skillTargetState, skillDmgType, skillElementType, attackMul)
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
                    var randomGridXIndex = goTarget.GridX[Random.Range(0, goTarget.GridX.Count)];

                    var finalGoTarget = heroInfoList.Where(hero => hero.GridX.Contains(randomGridXIndex)).ToList();

                    finalGoTarget.ForEach(f => mRuntimeUnit.AddGoTarget(f));
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
                    var randomGridXIndex = goTarget.GridX[Random.Range(0, goTarget.GridX.Count)];

                    var finalGoTarget = enemyInfoList.Where(enemy => enemy.GridX.Contains(randomGridXIndex)).ToList();

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