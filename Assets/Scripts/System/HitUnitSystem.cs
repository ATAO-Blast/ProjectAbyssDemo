using System.Collections.Generic;
using QFramework;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace AbyssDemo
{
    public class HitUnitSystem : AbstractSystem
    {
        ElementBuffSystem elementBuffSystem;
        ChooseSkillSystem chooseSkillSystem;
        protected override void OnInit()
        {
            elementBuffSystem = this.GetSystem<ElementBuffSystem>();
            chooseSkillSystem= this.GetSystem<ChooseSkillSystem>();
        }
        public void HitUnit(RuntimeUnitInfo performUnit,RuntimeUnitInfo targetUnit)
        {
            var elementType = chooseSkillSystem.CheckedSkill.SkillElementType;
            if (elementType == ElementType.Physics)
            {
                if (elementBuffSystem.CheckDontDodge(targetUnit, elementType))
                {
                    DoHit(Color.red, Color.red, elementType,performUnit,targetUnit);
                }
                else
                {
                    DoDodge(Color.red, elementType,performUnit,targetUnit);
                }
            }
            if (elementType == ElementType.Flame)
            {
                if (elementBuffSystem.CheckDontDodge(targetUnit, elementType))
                {
                    elementBuffSystem.GiveTargetBuff(elementType, performUnit, targetUnit);

                    DoHit( new Color(1, 0.65f, 0), new Color(1, 0.65f, 0), elementType,performUnit,targetUnit);
                }
                else
                {
                    DoDodge(new Color(1, 0.65f, 0), elementType,performUnit,targetUnit);
                }
            }
            if (elementType == ElementType.Poision)
            {
                if (elementBuffSystem.CheckDontDodge(targetUnit, elementType))
                {
                    elementBuffSystem.GiveTargetBuff(elementType, performUnit, targetUnit);


                    DoHit(Color.green, Color.green, elementType, performUnit, targetUnit);
                }
                else
                {
                    DoDodge(Color.green, elementType, performUnit, targetUnit);
                }
            }
            if (elementType == ElementType.Bolt)
            {
                if (elementBuffSystem.CheckDontDodge(targetUnit, elementType))
                {
                    elementBuffSystem.GiveTargetBuff(elementType, performUnit, targetUnit);


                    DoHit(Color.yellow, Color.yellow, elementType,performUnit,targetUnit);
                }
                else
                {
                    DoDodge(Color.yellow, elementType, performUnit, targetUnit);
                }
            }
            if (elementType == ElementType.Frost)
            {
                if (elementBuffSystem.CheckDontDodge(targetUnit, elementType))
                {
                    elementBuffSystem.GiveTargetBuff(elementType, performUnit, targetUnit);

                    DoHit(Color.blue, Color.blue, elementType,performUnit,targetUnit);
                }
                else
                {
                    DoDodge(Color.blue, elementType,performUnit,targetUnit);
                }
            }
            if (elementType == ElementType.Chaos)
            {
                if (elementBuffSystem.CheckDontDodge(targetUnit, elementType))
                {
                    elementBuffSystem.GiveTargetBuff(elementType, performUnit, targetUnit);

                    DoHit(Color.magenta, Color.magenta, elementType,performUnit,targetUnit);
                }
                else
                {
                    DoDodge(Color.magenta, elementType,performUnit,targetUnit);
                }
            }

        }
        public void DoHit(Color demageColor,Color lineEndColor,ElementType elementType,RuntimeUnitInfo performUnit,RuntimeUnitInfo targetUnit)
        {
            var overLoadingBuff = elementBuffSystem.GetUnitBuff(performUnit, ElementBuffType.Overloading);
            var meltingBuff = elementBuffSystem.GetUnitBuff(performUnit, ElementBuffType.Melting);
            var weakBuff = elementBuffSystem.GetUnitBuff(targetUnit, ElementBuffType.Weak);
            var explosionBuff = elementBuffSystem.GetUnitBuff(targetUnit, ElementBuffType.Explosion);
            var electrifyingBuff = elementBuffSystem.GetUnitBuff(targetUnit, ElementBuffType.Electrifying);
            var superConductBuff = elementBuffSystem.GetUnitBuff(targetUnit, ElementBuffType.SuperConduct);
            var atk = performUnit.Atk.Value;
            var def = targetUnit.Def.Value;
            if (meltingBuff != null) { atk *= 0.66f; }
            if (weakBuff != null) { def *= 0.33f; }
            if (def < atk)
            {
                var hit = atk - def;
                TypeEventSystem.Global.Send(new OnHitOccur() { hitInfo = $"{performUnit.Name}对{targetUnit.Name}造成了{hit}点{elementType}伤害\n" });
                this.SendEvent(new OnDamage() { demageColor = demageColor, demageNum = Mathf.FloorToInt(hit + 0.5f), position = targetUnit.StartPosition });
                
                this.SendEvent(new OnLineRender() { performPos = performUnit.StartPosition, targetPos = targetUnit.StartPosition, lineEndColor = lineEndColor });

                targetUnit.HitHp(hit);
                
            }
            else
            {
                TypeEventSystem.Global.Send(new OnHitOccur() { hitInfo = $"{performUnit.Name}对{targetUnit.Name}造成了1点{elementType}伤害\n" });
                targetUnit.HitHp(1);
                
            }
            if (overLoadingBuff != null)
            {
                var demage = overLoadingBuff.BuffAtk / 2;
                performUnit.HitHp(demage);
                this.SendEvent(new OnDamage()
                {
                    demageColor = Color.red,
                    demageNum = Mathf.FloorToInt(demage + 0.5f),
                    position = targetUnit.StartPosition
                });
            }
            if (explosionBuff != null)
            {
                if (performUnit.Group == "Hero")
                {
                    var mList = new List<RuntimeUnitInfo>(BattleSystem.Instance.EnemysUnitInfo);
                    mList.Remove(targetUnit);
                    if (mList.Count != 0)
                    {
                        mList.ForEach(unit =>
                        {
                            this.SendEvent(new OnDamage()
                            {
                                demageColor = Color.red,
                                demageNum = Mathf.FloorToInt(explosionBuff.BuffAtk * 0.25f + 0.5f),
                                position = targetUnit.StartPosition
                            });
                            unit.HitHp(explosionBuff.BuffAtk * 0.25f);
                        });
                    }
                }
                if (performUnit.Group == "Enemy")
                {
                    var mList = new List<RuntimeUnitInfo>(BattleSystem.Instance.HerosUnitInfo);
                    mList.Remove(targetUnit);
                    if (mList.Count != 0)
                    {
                        mList.ForEach(unit =>
                        {
                            this.SendEvent(new OnDamage()
                            {
                                demageColor = Color.red,
                                demageNum = Mathf.FloorToInt(explosionBuff.BuffAtk * 0.25f + 0.5f),
                                position = targetUnit.StartPosition
                            });
                            unit.HitHp(explosionBuff.BuffAtk * 0.25f);
                        });
                    }
                }
            }
            if (electrifyingBuff != null)
            {
                int time = 1;
                while (time <= 3)
                {
                    RuntimeUnitInfo goTar = null;
                    if (performUnit.Group == "Hero")
                    {
                        var mList = new List<RuntimeUnitInfo>(BattleSystem.Instance.EnemysUnitInfo);
                        mList.Remove(targetUnit);
                        if (mList.Count != 0) goTar = mList[UnityEngine.Random.Range(0, mList.Count)];
                    }
                    if (performUnit.Group == "Enemy")
                    {
                        var mList = new List<RuntimeUnitInfo>(BattleSystem.Instance.HerosUnitInfo);
                        mList.Remove(targetUnit);
                        if (mList.Count != 0) goTar = mList[UnityEngine.Random.Range(0, mList.Count)];
                    }
                    if (goTar != null)
                    {
                        var demage = electrifyingBuff.BuffAtk * 0.33f;
                        goTar.HitHp(demage);
                        this.SendEvent(new OnDamage()
                        {
                            demageColor = Color.red,
                            demageNum = Mathf.FloorToInt(demage + 0.5f),
                            position = targetUnit.StartPosition
                        });
                    }
                    else break;
                    time++;
                }
            }
            if (superConductBuff != null)
            {
                this.SendEvent<OnDamage>(new OnDamage()
                {
                    demageColor = Color.red,
                    demageNum = Mathf.FloorToInt(superConductBuff.BuffAtk + 0.5f),
                    position = targetUnit.StartPosition
                });
                targetUnit.HitHp(superConductBuff.BuffAtk);
            }
        }
        void DoDodge(Color lineEndColor, ElementType elementType,RuntimeUnitInfo performUnit,RuntimeUnitInfo targetUnit)
        {
            TypeEventSystem.Global.Send(new OnHitOccur() { hitInfo = $"{performUnit.Name}对{targetUnit.Name}的{elementType}伤害被闪避了\n" });
            this.SendEvent(new OnLineRender() { performPos = performUnit.StartPosition, targetPos = targetUnit.StartPosition, lineEndColor = lineEndColor });
            targetUnit.OnDodged.Trigger();
        }
    }
}
