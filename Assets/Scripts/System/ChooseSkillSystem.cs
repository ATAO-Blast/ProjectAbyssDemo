using System.Collections;
using UnityEngine;
using QFramework;
using System.Linq;
using System.Collections.Generic;

namespace AbyssDemo
{
    public class ChooseSkillSystem : AbstractSystem
    {
        private IRuntimeUnitModel runtimeUnitModel;
        private IUnitSkillModel unitSkillModel;
        private IUnitSkill checkedSkill;
        public IUnitSkill CheckedSkill { get { return checkedSkill; } }
        protected override void OnInit()
        {
            runtimeUnitModel = this.GetModel<IRuntimeUnitModel>();
            unitSkillModel= this.GetModel<IUnitSkillModel>();
        }
        public void ChooseActionBySkill(BaseFSM mTarget,FSM<TurnState> mFSM)
        {
            var unitInfo = runtimeUnitModel.GetRuntimeUnit(mTarget.gameObject.GetHashCode());
            
            var unitSkills = unitSkillModel.GetUnitSkillList(mTarget.gameObject.GetHashCode());
            if (unitSkills != null)
            {
                if (unitSkills.Count > 1)
                {
                    var priority = unitSkills.Max(unitSkill => unitSkill.Priority);
                    
                    while (true)
                    {
                        if (priority < 0) { mFSM.ChangeState(TurnState.PROCESSING); Debug.Log(unitInfo.Threat + unitInfo.Name + "在SkillPreCheck时没有找到有效目标");break; }
                        var pSkillLists = unitSkills.Where(unitSkill => unitSkill.Priority == priority).ToList();
                        var pSkill = pSkillLists[Random.Range(0, pSkillLists.Count)];
                        if (pSkill.SkillPreCheck(mTarget, mFSM))
                        {
                            checkedSkill = pSkill;
                            //Debug.Log("在Count>1选择了" + checkedSkill.Name);
                            pSkill.SkillChooseTarget(mTarget, mFSM);
                            return;
                        }
                        else
                        {
                            priority -= 1;
                        }
                    }
                }
                else
                {
                    var unitSkill = unitSkills[0];
                    if (unitSkill.SkillPreCheck(mTarget, mFSM))
                    {
                        checkedSkill = unitSkill;
                        //Debug.Log("在Count=1选择了" + checkedSkill.Name);
                        unitSkill.SkillChooseTarget(mTarget, mFSM);
                    }
                    else
                    {
                        Debug.Log(unitInfo.Threat + unitInfo.Name + "在SkillPerCheck时没有找到有效目标");
                        mFSM.ChangeState(TurnState.PROCESSING);
                    }
                }
            }
            else
            {
                Debug.Log(unitInfo.Threat + unitInfo.Name + "没有返回技能列表");
                mFSM.ChangeState(TurnState.PROCESSING);
            }

        }
        public RuntimeUnitInfo ChooseTargetByThreat(List<RuntimeUnitInfo> infos)
        {
            var threadvalue = 0;
            infos.ForEach(info => { if (info.Threat >= threadvalue) threadvalue = info.Threat; });
            var targetList = infos.Where(enemy => enemy.Threat == threadvalue).ToList();

            var goTarget = targetList[Random.Range(0, targetList.Count)];
            return goTarget;
        }
        public RuntimeUnitInfo ReChooseTargetByRidicule(List<RuntimeUnitInfo> infos, RuntimeUnitInfo curTarget)
        {
            var checkNum = Random.Range(0f, 1f);
            if (checkNum > curTarget.RidiculeSuccessRate || Mathf.Approximately(checkNum, curTarget.RidiculeSuccessRate))
            {
                var newGoTarget = infos[Random.Range(0, infos.Count)];
                return newGoTarget;
            }
            else return curTarget;
        }


    }
}