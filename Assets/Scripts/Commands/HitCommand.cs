using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace AbyssDemo
{
    public class HitCommand : AbstractCommand
    {
        float demage;
        RuntimeUnitInfo targetUnit;
        bool isJump;
        Color demageColor;
        public HitCommand(RuntimeUnitInfo targetUnit, float demage, bool isJump, Color demageColor)
        {
            this.demage = demage;
            this.targetUnit = targetUnit;
            this.isJump = isJump;
            this.demageColor = demageColor;
        }

        protected override void OnExecute()
        {
            if(targetUnit.CurHP.Value> 0)
            {
                targetUnit.CurHP.Value -= demage;
                if(targetUnit.CurHP.Value <0 || Mathf.Approximately(targetUnit.CurHP.Value, 0))
                {
                    targetUnit.UnitFSM.ChangeState(TurnState.DEAD);
                    targetUnit.OnDestroy?.Trigger();
                    return;
                }
                if(isJump) targetUnit.OnHited?.Trigger(demage);
                this.SendEvent<OnDamage>(new OnDamage() { demageColor = demageColor, demageNum = Mathf.FloorToInt(demage + 0.5f), position = targetUnit.StartPosition });
                if (targetUnit.CurHP.Value > 0 && targetUnit.CurHP.Value < targetUnit.MaxHP.Value)
                {
                    targetUnit.HealthState.Value = SkillTargetState.CanHeal;
                    if (targetUnit.CurHP.Value < targetUnit.MaxHP.Value * 0.3f)
                    {
                        targetUnit.IsDying = true;
                    }
                    else targetUnit.IsDying = false;
                }
                else if (targetUnit.CurHP.Value > targetUnit.MaxHP.Value || Mathf.Approximately(targetUnit.CurHP.Value, targetUnit.MaxHP.Value))
                {
                    targetUnit.CurHP.Value = targetUnit.MaxHP.Value;
                    targetUnit.HealthState.Value = SkillTargetState.FullHP;
                    targetUnit.IsDying = false;
                }
            }
        }
        
    }
}