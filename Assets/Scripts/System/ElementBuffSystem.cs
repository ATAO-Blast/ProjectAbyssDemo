using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace AbyssDemo
{
    public class ElementBuffSystem : AbstractSystem
    {
        private Dictionary<RuntimeUnitInfo,Dictionary<ElementBuffType,IUnitElementBuff>> unitsBuffDic = new Dictionary<RuntimeUnitInfo, Dictionary<ElementBuffType, IUnitElementBuff>>();
        private List<ElementType> elementTypes= new List<ElementType>(4) { ElementType.Frost,ElementType.Flame,ElementType.Bolt,ElementType.Poision};
        protected override void OnInit()
        {
        }
        public void ClearAllUnitBuffs() 
        {
            unitsBuffDic.Clear();
        }
        public void AddUnitBuff(RuntimeUnitInfo unitToGetBuff,ElementBuffType elementBuffType, IUnitElementBuff unitElementBuff)
        {
            if(unitsBuffDic.ContainsKey(unitToGetBuff))
            {
                var unitBuffDic = unitsBuffDic[unitToGetBuff];
                if(unitBuffDic != null) 
                {
                    if (unitBuffDic.ContainsKey(elementBuffType))
                    {
                        if (unitBuffDic[elementBuffType].BuffProvider == unitElementBuff.BuffProvider) return;//?
                        else
                        {
                            //unitToGetBuff.ElementBuffStates.Add(elementBuffType);
                            unitBuffDic[elementBuffType] = unitElementBuff;
                        }
                    }
                    else { unitBuffDic.Add(elementBuffType,unitElementBuff); }
                }
                else
                {
                    unitsBuffDic[unitToGetBuff] = new Dictionary<ElementBuffType, IUnitElementBuff>()
                    {
                        {elementBuffType, unitElementBuff},
                    };
                }
                
            }
            else
            {
                unitsBuffDic.Add(unitToGetBuff, new Dictionary<ElementBuffType, IUnitElementBuff>()
                {
                    {elementBuffType, unitElementBuff}
                });
            }
        }
        public void RemoveUnitBuff(RuntimeUnitInfo unitToRemoveBuff,ElementBuffType elementBuffType)
        {
            if (unitsBuffDic.ContainsKey(unitToRemoveBuff))
            {
                var unitBuffDic = unitsBuffDic[unitToRemoveBuff];
                if (unitBuffDic != null)
                {
                    if (unitBuffDic.ContainsKey(elementBuffType))
                    {
                        unitBuffDic.Remove(elementBuffType);
                    }
                    else Debug.LogWarning(unitToRemoveBuff.Name + "Dont have " + elementBuffType.ToString());
                }
                else Debug.LogWarning(unitToRemoveBuff.Name + "Dont Init BuffDic ");
            }
            else Debug.LogWarning("UnitsBuffDic Don't Has" + unitToRemoveBuff.Name + "as Key");
        }
        public void RemoveUnit(RuntimeUnitInfo unitToRemove)
        {
            if(unitsBuffDic.ContainsKey(unitToRemove))
            {
                var unitBuffDic = unitsBuffDic[unitToRemove];
                if(unitBuffDic != null)
                {
                    unitBuffDic.Clear();
                }
                unitsBuffDic.Remove(unitToRemove);
            }
        }
        public Dictionary<ElementBuffType,IUnitElementBuff> GetUnitBuffDic(RuntimeUnitInfo unitToGetBufDic)
        {
            if (unitsBuffDic.ContainsKey(unitToGetBufDic))
            {
                Dictionary<ElementBuffType, IUnitElementBuff> unitBuffDic;
                if (unitsBuffDic.TryGetValue(unitToGetBufDic, out unitBuffDic))
                {
                    return unitBuffDic;
                }
            }
            return null;
        }
        public IUnitElementBuff GetUnitBuff(RuntimeUnitInfo unitToGetBuff,ElementBuffType elementBuffType)
        {
            var unitBuffDic = GetUnitBuffDic(unitToGetBuff);
            if (unitBuffDic != null && unitBuffDic.ContainsKey(elementBuffType))
            {
                IUnitElementBuff unitElementBuff;
                if(unitBuffDic.TryGetValue(elementBuffType,out unitElementBuff))
                {
                    return unitElementBuff;
                }
                else return null;
            }
            return null;
        }
        public void CheckBuffs(BaseFSM mTarget,FSM<TurnState> mFSM)
        {
            var mRuntimeUnit = this.GetModel<IRuntimeUnitModel>().GetRuntimeUnit(mTarget.gameObject.GetHashCode());
            if (CheckUnitDying(mRuntimeUnit)) return;
            var unitBuffDic = GetUnitBuffDic(mRuntimeUnit);
            if(unitBuffDic != null)
            {
                if (unitBuffDic.Keys.Count > 0)
                {
                    
                    if (unitBuffDic.ContainsKey( ElementBuffType.Burning))
                    {
                        var unitBuff = unitBuffDic[ElementBuffType.Burning];
                        if (unitBuff.BurningCount != 0)
                        {
                            mRuntimeUnit.HitHp(unitBuff.BuffProvider.Atk.Value);
                            unitBuff.BurningCount--;
                        }
                        else
                        {
                            RemoveUnitBuff(mRuntimeUnit, ElementBuffType.Burning);
                        }
                        if(CheckUnitDying(mRuntimeUnit)) { return; }
                    }
                    
                }
            }
        }
        public void AddingBuffTime(BaseFSM mTarget,FSM<TurnState> mFSM,RuntimeUnitInfo runtimeUnitInfo)
        {
            
            if (CheckUnitDying(runtimeUnitInfo)) return;
            var unitBuffDic = GetUnitBuffDic(runtimeUnitInfo);
            if (unitBuffDic != null)
            {
                if (unitBuffDic.Keys.Count > 0)
                {
                    
                    if (unitBuffDic.ContainsKey(ElementBuffType.Burning))
                    {
                        var unitBuff = unitBuffDic[ElementBuffType.Burning];
                        unitBuff.BuffInitTime += Time.deltaTime;
                            
                        if (unitBuff.BuffInitTime > 0.75f && unitBuff.BurningCount >= 1)
                        {
                            Debug.Log($"{runtimeUnitInfo.Name}{runtimeUnitInfo.Threat}BuringBuffHited");
                            var demageNum = unitBuff.BuffAtk * 0.4f;
                            runtimeUnitInfo.HitHp(demageNum);
                            this.SendEvent<OnDamage>(new OnDamage() 
                            { demageColor = new Color(1, 0.65f, 0), 
                                demageNum = Mathf.FloorToInt(demageNum + 0.5f), 
                                position = runtimeUnitInfo.StartPosition 
                            });
                                
                            unitBuff.BurningCount--;
                            unitBuff.BuffInitTime = 0f;
                        }
                        else if(unitBuff.BurningCount == 0)
                        {
                            Debug.Log($"{runtimeUnitInfo.Name}{runtimeUnitInfo.Threat}BuringBuffRemoved");
                            RemoveUnitBuff(runtimeUnitInfo, ElementBuffType.Burning);
                        }
                        if (CheckUnitDying(runtimeUnitInfo)) { return; }
                    }

                    if (unitBuffDic.ContainsKey(ElementBuffType.Hypertoxic))
                    {
                        var unitBuff = unitBuffDic[ElementBuffType.Hypertoxic];
                        unitBuff.BuffInitTime += Time.deltaTime;
                        if (unitBuff.BuffInitTime > 0.75f && unitBuff.PosionCount >= 1 )
                        {
                            Debug.Log("PoisionBuffHited");

                            var demageNum = unitBuff.BuffAtk * 0.3f;
                            runtimeUnitInfo.HitHp(demageNum);
                            this.SendEvent<OnDamage>(new OnDamage()
                            {
                                demageColor = new Color(1, 0.65f, 0),
                                demageNum = Mathf.FloorToInt(demageNum + 0.5f),
                                position = runtimeUnitInfo.StartPosition
                            });
                            unitBuff.BuffInitTime = 0f;
                            unitBuff.PosionCount--;
                        }
                        else if (unitBuff.PosionCount == 0)
                        {
                            Debug.Log("PoisionBuffRemoved");
                            RemoveUnitBuff(runtimeUnitInfo, ElementBuffType.Hypertoxic);
                        }
                        if (CheckUnitDying(runtimeUnitInfo)) { return; }
                    }

                    if(unitBuffDic.ContainsKey(ElementBuffType.Overloading)) 
                    {
                        var unitBuff = unitBuffDic[ElementBuffType.Overloading];
                        unitBuff.BuffInitTime += Time.deltaTime;
                        if(unitBuff.BuffInitTime > 6f)
                        {
                            Debug.Log("OverloadingBuffRemoved");
                            RemoveUnitBuff(runtimeUnitInfo,ElementBuffType.Overloading);
                        }
                        
                    }
                    if(unitBuffDic.ContainsKey(ElementBuffType.Melting))
                    {
                        var unitBuff = unitBuffDic[ElementBuffType.Melting];
                        unitBuff.BuffInitTime += Time.deltaTime;
                        if (unitBuff.BuffInitTime > 10f)
                        {
                            Debug.Log("MeltingBuffRemoved");
                            RemoveUnitBuff(runtimeUnitInfo, ElementBuffType.Melting);
                        }
                    }
                    if (unitBuffDic.ContainsKey(ElementBuffType.Weak))
                    {
                        var unitBuff = unitBuffDic[ElementBuffType.Weak];
                        unitBuff.BuffInitTime += Time.deltaTime;
                        if (unitBuff.BuffInitTime > 10f)
                        {
                            Debug.Log("WeakBuffRemoved");
                            RemoveUnitBuff(runtimeUnitInfo, ElementBuffType.Weak);
                        }
                    }
                    if (unitBuffDic.ContainsKey(ElementBuffType.Frozen))
                    {
                        var unitBuff = unitBuffDic[ElementBuffType.Frozen];
                        unitBuff.BuffInitTime += Time.deltaTime;
                        if (unitBuff.BuffInitTime > 4f)
                        {
                            Debug.Log("FrozenBuffRemoved");
                            RemoveUnitBuff(runtimeUnitInfo, ElementBuffType.Frozen);
                        }
                    }
                    if(unitBuffDic.ContainsKey(ElementBuffType.Confusion))
                    {
                        var unitBuff = unitBuffDic[ElementBuffType.Confusion];
                        unitBuff.BuffInitTime += Time.deltaTime;
                        if (unitBuff.BuffInitTime > 4f)
                        {
                            Debug.Log("ConfusionBuffRemoved");
                            RemoveUnitBuff(runtimeUnitInfo,ElementBuffType.Confusion);
                        }
                    }
                    
                }
            }
        }
        bool CheckUnitDying(RuntimeUnitInfo runtimeUnitInfo)
        {
            if(runtimeUnitInfo.CurHP.Value<0 || Mathf.Approximately(runtimeUnitInfo.CurHP.Value, 0))
            {
                RemoveUnit(runtimeUnitInfo);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 根据两个元素类型返回它们生成的元素Buff
        /// </summary>
        /// <param name="elementType1">元素1</param>
        /// <param name="elementType2">元素2</param>
        /// <returns>元素Buff</returns>
        public ElementBuffType GetElementBuffFromElement(ElementType elementType1, ElementType elementType2)
        {
            if (elementType1 == ElementType.Frost)
            {
                if (elementType2 == ElementType.Frost) { return ElementBuffType.Frozen; }
                if (elementType2 == ElementType.Flame) { return ElementBuffType.Melting; }
                if (elementType2 == ElementType.Bolt) { return ElementBuffType.SuperConduct; }
                if (elementType2 == ElementType.Poision) { return ElementBuffType.Weak; }
                if (elementType2 == ElementType.Chaos) { return GetElementBuffFromElement(elementType1, elementTypes[Random.Range(0, elementTypes.Count)]); }
            }
            if (elementType1 == ElementType.Flame)
            {
                if (elementType2 == ElementType.Frost) { return ElementBuffType.Melting; }
                if (elementType2 == ElementType.Flame) { return ElementBuffType.Burning; }
                if (elementType2 == ElementType.Bolt) { return ElementBuffType.Overloading; }
                if (elementType2 == ElementType.Poision) { return ElementBuffType.Explosion; }
                if (elementType2 == ElementType.Chaos) { return GetElementBuffFromElement(elementType1, elementTypes[Random.Range(0, elementTypes.Count)]); }
            }
            if (elementType1 == ElementType.Bolt)
            {
                if (elementType2 == ElementType.Frost) { return ElementBuffType.SuperConduct; }
                if (elementType2 == ElementType.Flame) { return ElementBuffType.Overloading; }
                if (elementType2 == ElementType.Bolt) { return ElementBuffType.Electrifying; }
                if (elementType2 == ElementType.Poision) { return ElementBuffType.Confusion; }
                if (elementType2 == ElementType.Chaos) { return GetElementBuffFromElement(elementType1, elementTypes[Random.Range(0, elementTypes.Count)]); }
            }
            if (elementType1 == ElementType.Poision)
            {
                if (elementType2 == ElementType.Frost) { return ElementBuffType.Weak; }
                if (elementType2 == ElementType.Flame) { return ElementBuffType.Explosion; }
                if (elementType2 == ElementType.Bolt) { return ElementBuffType.Confusion; }
                if (elementType2 == ElementType.Poision) { return ElementBuffType.Hypertoxic; }
                if (elementType2 == ElementType.Chaos) { return GetElementBuffFromElement(elementType1, elementTypes[Random.Range(0, elementTypes.Count)]); }
            }
            if (elementType1 == ElementType.Chaos)
            {
                if (elementType2 == ElementType.Frost) { return GetElementBuffFromElement(elementTypes[Random.Range(0, elementTypes.Count)],ElementType.Frost); }
                if (elementType2 == ElementType.Flame) { return GetElementBuffFromElement(elementTypes[Random.Range(0, elementTypes.Count)], ElementType.Flame); }
                if (elementType2 == ElementType.Bolt) { return GetElementBuffFromElement(elementTypes[Random.Range(0, elementTypes.Count)], ElementType.Bolt); }
                if (elementType2 == ElementType.Poision) { return GetElementBuffFromElement(elementTypes[Random.Range(0, elementTypes.Count)], ElementType.Poision); }
                if (elementType2 == ElementType.Chaos) { return GetElementBuffFromElement(elementType1, elementTypes[Random.Range(0, elementTypes.Count)]); }
            }
            return ElementBuffType.None;
        }

        public void GiveTargetBuff(ElementType elementTypeToGive,RuntimeUnitInfo performUnit,RuntimeUnitInfo targetUnit)
        {
            if(targetUnit.ElementState == ElementType.Physics) 
            {
                targetUnit.ElementState.Value = elementTypeToGive;
                return;
            }
            var buffToCheck = GetElementBuffFromElement(elementTypeToGive,targetUnit.ElementState);
            var targetBuff = GetUnitBuff(targetUnit,buffToCheck);
            if(targetBuff == null || targetBuff.BuffProvider != performUnit)
            {
                targetUnit.ElementState.Value = ElementType.Physics;
                AddUnitBuff(targetUnit, buffToCheck, new UnitElementBuff(performUnit, performUnit.Atk.Value, 4, 6, buffToCheck, 0f));
            }
        }
        /// <summary>
        /// 目标对象没有闪避则返回True
        /// </summary>
        /// <param name="targetUnit">目标对象</param>
        /// <param name="elementType">技能攻击类型</param>
        /// <returns>没有闪避为True</returns>
        public bool CheckDontDodge(RuntimeUnitInfo targetUnit, ElementType elementType)
        {
            if (elementType == ElementType.Physics)
            {
                var checkNum = Random.Range(0f, 1f);
                if (checkNum > targetUnit.Dodge || Mathf.Approximately(checkNum, targetUnit.Dodge))//这个判断的是checkNum值在dodge值和1之间的区间，在区间内就说明闪避失败
                {
                    return true;
                }
                else return false;
            }
            if (elementType == ElementType.Flame)
            {
                var dodgeNum = (1f - targetUnit.FlameResis.Value) * targetUnit.BodySizeDodge;//dodgeNum越小，越容易闪避
                return ElementDodgeCheck(dodgeNum);
            }
            if (elementType == ElementType.Poision)
            {
                var dodgeNum = (1f - targetUnit.PoisionResis.Value) * targetUnit.BodySizeDodge;//dodgeNum越小，越容易闪避
                return ElementDodgeCheck(dodgeNum);
            }
            if (elementType == ElementType.Frost)
            {
                var dodgeNum = (1f - targetUnit.FrostResis.Value) * targetUnit.BodySizeDodge;//dodgeNum越小，越容易闪避
                return ElementDodgeCheck(dodgeNum);
            }
            if (elementType == ElementType.Bolt)
            {
                var dodgeNum = (1f - targetUnit.BoltResis.Value) * targetUnit.BodySizeDodge;//dodgeNum越小，越容易闪避
                return ElementDodgeCheck(dodgeNum);
            }
            if (elementType == ElementType.Chaos)
            {
                var dodgeNum = (1f - targetUnit.ChaosResis.Value) * targetUnit.BodySizeDodge;//dodgeNum越小，越容易闪避
                return ElementDodgeCheck(dodgeNum);
            }
            return true;
        }
        bool ElementDodgeCheck(float dodgeNum)
        {
            var checkNum = Random.Range(0f, 1f);
            if (checkNum < dodgeNum || Mathf.Approximately(checkNum, dodgeNum))//这个判断的是checkNum值在dodgeNum值和0之间的区间，在区间内就说明闪避失败
            {
                return true;
            }
            else return false;
        }
    }
}