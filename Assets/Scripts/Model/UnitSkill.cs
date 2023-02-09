using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace AbyssDemo
{
    public interface IUnitSkill
    {
        string Name { get; }
        int Priority { get; }
        SkillTarget SkillTarget { get; }
        SkillSelect SkillSelect { get; }
        SkillTargetState SkillTargetState { get; }
        SkillDmgType SkillDmgType { get; }
        ElementType SkillElementType { get; }
        bool SkillPreCheck(BaseFSM mTarget, FSM<TurnState> mFSM);

        void SkillChooseTarget(BaseFSM mTarget, FSM<TurnState> mFSM);
        void SkillPerform(BaseFSM mTarget, FSM<TurnState> mFSM);

    }
    public abstract class UnitSkill : IUnitSkill,IController
    {
        
        private string name;
        private int priority = 0;
        private SkillTarget skillTarget = SkillTarget.Opposite;
        private SkillSelect skillSelect = SkillSelect.Once;
        private SkillTargetState skillTargetState = SkillTargetState.Any;
        private SkillDmgType skillDmgType = SkillDmgType.Remote;
        private ElementType skillElementType = ElementType.Physics;
        private float attackMul = 1f;
        public string Name=> name;
        public int Priority => priority;
        public SkillTarget SkillTarget => skillTarget;
        public SkillSelect SkillSelect => skillSelect;
        public SkillTargetState SkillTargetState=>skillTargetState;
        public SkillDmgType SkillDmgType=>skillDmgType;
        public ElementType SkillElementType => skillElementType;
        public float AttackMul => attackMul;
        public UnitSkill(string name,int priority, SkillTarget skillTarget, SkillSelect skillSelect, SkillTargetState skillTargetState, SkillDmgType skillDmgType, ElementType skillElementType, float attackMul = 1f)
        {
            this.name = name;
            this.priority = priority;
            this.skillTarget = skillTarget;
            this.skillSelect = skillSelect;
            this.skillTargetState = skillTargetState;
            this.skillDmgType = skillDmgType;
            this.skillElementType = skillElementType;
            this.attackMul = attackMul;
        }

        public virtual bool SkillPreCheck(BaseFSM mTarget,FSM<TurnState> mFSM)
        {
            return true;
        }
        public virtual void SkillChooseTarget(BaseFSM mTarget, FSM<TurnState> mFSM)
        {

        }
        public virtual void SkillPerform(BaseFSM mTarget,FSM<TurnState> mFSM)
        {

        }

        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }
    }
    public enum SkillTarget
    {
        Any,
        Opposite,
        Self,
        EnemyFront,
        HeroFront
    }
    public enum SkillSelect
    {
        Once,
        Twice,
        Thrice,
        OneRow,
        OneColumn,
        All
    }
    public enum SkillTargetState
    {
        Any,
        Dead,
        Surviving,
        Dying,
        FullHP,
        CanHeal
    }
    public enum SkillDmgType
    {
        Close,
        Remote,
        Environment,
        Self
    }
    public enum ElementType
    {
        Physics,
        Flame,
        Frost,
        Bolt,
        Poision,
        Chaos,
        Real,
        Healing
    }
    public enum ElementBuffType
    {
        None,
        Frozen,
        Melting,
        SuperConduct,
        Weak,
        Burning,
        Overloading,
        Explosion,
        Electrifying,
        Confusion,
        Hypertoxic
    }
}