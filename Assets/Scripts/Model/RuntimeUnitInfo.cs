using System.Collections.Generic;
using UnityEngine;
using System.Text;
using QFramework;
using System;

namespace AbyssDemo
{
    public class RuntimeUnitInfo
    {
        private string name;
        private List<string> namePrefix;//?待定
        private int exp;
        private int level;//只读
        private string[] skillLists;
        private string group;//Hero或者Enemy
        private List<RuntimeUnitInfo> goTargets;
        private Vector3 startPosition;
        private BaseUnitSO baseUnitSO;
        private float ridiculeSuccessRate;
        private int threat = 0;
        private List<int> gridX = new List<int>(3);
        private List<int> gridY = new List<int>(4);
        private FSM<TurnState> unitFSM;
        private int hashKey;

        public BindableProperty<float> CurActionTime = new BindableProperty<float>();

        public BindableProperty<float> CurHP { get; } = new BindableProperty<float>();
        public BindableProperty<float> MaxHP = new BindableProperty<float>();
        public BindableProperty<float> Atk = new BindableProperty<float>();
        public BindableProperty<float> Def = new BindableProperty<float>();
        public BindableProperty<float> Speed = new BindableProperty<float>();
        private float dodge;

        public BindableProperty<float> FlameResis = new BindableProperty<float>();
        public BindableProperty<float> FrostResis = new BindableProperty<float>();
        public BindableProperty<float> BoltResis = new BindableProperty<float>();
        public BindableProperty<float> PoisionResis = new BindableProperty<float>();
        public BindableProperty<float> ChaosResis = new BindableProperty<float>();

        private bool canBeHeal = true;
        private bool isDying = false;
        public BindableProperty<SkillTargetState> HealthState = new BindableProperty<SkillTargetState>() { Value = SkillTargetState.FullHP};
        public BindableProperty<ElementType> ElementState = new BindableProperty<ElementType>() { Value = ElementType.Physics};
        //public HashSet<ElementBuffType> ElementBuffStates { get; set; } = new HashSet<ElementBuffType>(11);
        private float currectedValue;

        public EasyEvent OnDestroy { get; set; } = new EasyEvent();
        public EasyEvent<float> OnHited { get; set; } = new EasyEvent<float>();
        public EasyEvent<float> OnHealed { get; set; } = new EasyEvent<float>();
        public EasyEvent OnDodged { get; set; } = new EasyEvent();
        public bool CanBeHeal { get { return canBeHeal; } set { canBeHeal = value; } }
        public bool IsDying { get { return isDying; } set { isDying = value; } }
        public string[] SkillLists => skillLists;
        public string PerformSkill { get; set; }
        public string Name
        {
            get
            {
                if (namePrefix.Count > 0)
                {
                    var sb = new StringBuilder();
                    if (namePrefix.Count == 1)
                    {
                        sb.Append(namePrefix[0]).Append("之");
                    }
                    else 
                    {
                        for (int i = 0; i < namePrefix.Count; i++)
                        {
                            if (i == 0)
                            {
                                sb.Append(namePrefix[i]).Append("之");
                            }
                            else if (i == namePrefix.Count - 1)
                            {
                                sb.Append(namePrefix[i]).Append("的");
                            }
                            else sb.Append(namePrefix[i]);
                        } 
                    }
                    sb.Append(name);
                    return sb.ToString();
                }
                else return name;
            }
        }
        public int Exp => exp;
        public int Level => level;
        public int HashKey => hashKey;
        public float Dodge { 
            get { return dodge; } 
            set {
                if (value > 0 && value < 1)
                {
                    dodge = value;
                }
                else if (Mathf.Approximately(0, value) || Mathf.Approximately(1, value))
                {
                    dodge = value;
                };
            } }
        public float BodySizeDodge
        {
            get
            {
                if(baseUnitSO.bodySize == 1) { return 1f; }
                if(baseUnitSO.bodySize == 2) { return 0.66f; }
                if(baseUnitSO.bodySize == 3) { return 0.33f; }
                return 0.25f;
            }
        }
        public float RidiculeSuccessRate 
        { 
            get { return ridiculeSuccessRate; } 
            set 
            { 
                if(value > 0 && value < 1)
                {
                    ridiculeSuccessRate = value;
                }
                else if(Mathf.Approximately(0,value) || Mathf.Approximately(1, value))
                {
                    ridiculeSuccessRate = value;
                };
            }
            
        }
        public string Group
        {
            get { return group; }
            set { group = value; }
        }
        public List<RuntimeUnitInfo> GoTargets { get { return goTargets; }}
        public Vector3 StartPosition
        {
            get { return startPosition; }
            set
            {
                startPosition= value;
            }
        }
        public List<int> GridX { get { return gridX; }}
        public List<int> GridY { get { return gridY; }}
        public FSM<TurnState> UnitFSM => unitFSM;
        public int Threat
        {
            get { return threat + baseUnitSO.bodySize; }
            set { threat= value; }
        }
        //public BindableProperty<int> CurHP { get { return curHP; } set { curHP = value; } }

        //public int MaxHP { get { return maxHP; } set { maxHP = value; } }

        public int CombatPower => Mathf.FloorToInt(CurHP.Value * (Atk.Value * 100 / Speed.Value + (Def.Value - 3 * Mathf.Pow(1.5f, level - 1))) * currectedValue / Mathf.Pow(1.5f, level) / 10 + 0.5f);
        //public int Atk { get { return atk; } set { atk = value; } }

        //public int Def { get { return def; } set { def = value; } }

        //public int Speed
        public RuntimeUnitInfo(BaseUnitSO baseUnitSo, FSM<TurnState> unitFSM, int hashKey)
        {
            this.baseUnitSO = baseUnitSo;
            name = baseUnitSo.name;
            namePrefix = new List<string>();
            goTargets= new List<RuntimeUnitInfo>();
            exp = baseUnitSo.exp;
            level = baseUnitSo.level;
            ridiculeSuccessRate= baseUnitSo.ridiculeSuccessRate;
            CurHP.Value = baseUnitSo.HP;
            MaxHP.Value = baseUnitSo.HP;
            Atk.Value = baseUnitSo.ATK;
            Def.Value = baseUnitSo.DEF;
            Speed.Value = baseUnitSo.speed;
            dodge = baseUnitSo.dodge;
            currectedValue = baseUnitSo.currectedValue;

            FlameResis.Value = baseUnitSo.flameResis;
            FrostResis.Value = baseUnitSo.frostResis;
            BoltResis.Value = baseUnitSo.boltResis;
            FrostResis.Value = baseUnitSo.poisionResis;
            ChaosResis.Value = baseUnitSo.chaosResis;

            skillLists = baseUnitSo.skills;
            this.unitFSM = unitFSM;
            this.hashKey = hashKey;
        }
        public void AddNamePrefix(string prefix)
        {
            namePrefix.Add(prefix);
        }
        
        public void AddGoTarget(RuntimeUnitInfo goTarget)
        {
            if (!goTargets.Contains(goTarget))
            {
                this.goTargets.Add(goTarget);
            }
        }
        public void ClearGoTarget() { this.goTargets.Clear(); }
        
        public void AddGridX(int x)
        {
            this.gridX.Add(x);
        }
        public void RemoveGridX(int x)
        {
            if (gridX.Contains(x))
            {
                gridX.Remove(x);
            }
        }
        public void AddGridY(int y)
        {
            this.gridY.Add(y);
        }
        public void RemoveGridY(int y)
        {
            if (gridY.Contains(y))
            {
                gridY.Remove(y);
            }
        }
        public void HitHp(float hit)
        {
            if (CurHP.Value > 0)
            {
                CurHP.Value -= hit;
                //Debug.Log(threat + group + hit);
                if(CurHP.Value < 0 || Mathf.Approximately(CurHP.Value,0))
                {
                    unitFSM.ChangeState(TurnState.DEAD);
                    OnDestroy?.Trigger();
                    return;
                }
                OnHited?.Trigger(hit);
                
                if(CurHP.Value > 0 && CurHP.Value < MaxHP.Value)
                {
                    HealthState.Value = SkillTargetState.CanHeal;
                    if (CurHP.Value < MaxHP.Value * 0.3f)
                    {
                        isDying = true;
                    }
                    else isDying = false;
                }
                else if(Mathf.Approximately(CurHP.Value,MaxHP.Value))
                {
                    HealthState.Value = SkillTargetState.FullHP;
                    isDying= false;
                }
            }
        }
        public void HealHp(float heal)
        {
            if(canBeHeal && HealthState.Value == SkillTargetState.CanHeal)
            {
                var diffHeal = MaxHP.Value - CurHP.Value;
                if(heal< diffHeal)
                {
                    CurHP.Value += heal;
                    OnHealed.Trigger(heal);
                }
                else
                {
                    CurHP.Value += diffHeal;
                    OnHealed.Trigger(diffHeal);
                }
                if (CurHP.Value > 0 && CurHP.Value < MaxHP.Value)
                {
                    HealthState.Value = SkillTargetState.CanHeal;
                    if (CurHP.Value < MaxHP.Value * 0.3f)
                    {
                        isDying = true;
                    }
                    else isDying = false;
                }
                else if (Mathf.Approximately(CurHP.Value, MaxHP.Value))
                {
                    HealthState.Value = SkillTargetState.FullHP;
                    isDying = false;
                }
            }
        }
        public string DisplayInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<size=36>").Append(GetColorType()).Append(Name).Append("</color></size>").AppendLine(Environment.NewLine);
            sb.Append("生命值：\t").Append(CurHP.Value).Append('/').Append(MaxHP.Value).AppendLine();
            sb.Append("攻击力：\t").Append(Atk.Value).AppendLine();
            sb.Append("防御力：\t").Append(Def.Value).AppendLine();
            sb.Append("速度：\t").Append(Speed.Value).AppendLine();
            if (baseUnitSO.skills.Length > 0)
            {
                sb.Append("技能：").AppendLine();
                foreach (string skill in baseUnitSO.skills)
                {
                    sb.Append(skill).AppendLine();
                }
            }
            sb.Append("战力：").Append(CombatPower);
            sb.AppendLine(Environment.NewLine);
            sb.Append(baseUnitSO.description);
            return sb.ToString();
        }
        string GetColorType()
        {
            if (level == 1) return "<color=white>";
            else if (level == 2) return "<color=green>";
            else if (level == 3) return "<color=blue>";
            else if (level == 4) return "<color=#ff00ffff>";
            else if (level == 5) return "<color=#ffff00ff>";
            else return null;
        }
    }
}