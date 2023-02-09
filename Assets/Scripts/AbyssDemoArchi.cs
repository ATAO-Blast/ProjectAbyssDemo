using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    public class AbyssDemoArchi : Architecture<AbyssDemoArchi>
    {
        protected override void Init()
        {
            this.RegisterModel<IUnitSkillModel>(new UnitSkillModel());
            this.RegisterModel<IRuntimeUnitModel>(new RuntimeUnitModel());
            this.RegisterSystem(new GridSystem());
            this.RegisterSystem(new ChooseSkillSystem());
            this.RegisterSystem(new PerformSkillSystem());
            this.RegisterSystem(new ElementBuffSystem());
            this.RegisterSystem(new HitUnitSystem());
        }

        
    }
}