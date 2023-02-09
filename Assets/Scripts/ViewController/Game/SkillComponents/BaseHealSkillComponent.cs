using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace AbyssDemo
{
    public class BaseHealSkillComponent : MonoBehaviour, IController
    {
        private IUnitSkillModel unitSkillModel;
        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }

        // Start is called before the first frame update
        void Start()
        {
            unitSkillModel = this.GetModel<IUnitSkillModel>();
            unitSkillModel.AddSkill(this.gameObject.GetHashCode(), new BaseHealSkill(
                nameof(BaseHealSkill),
                1,
                SkillTarget.Self,
                SkillSelect.Once,
                SkillTargetState.CanHeal,
                SkillDmgType.Remote,
                ElementType.Healing
                ));
        }


        private void OnDestroy()
        {
            unitSkillModel.RemoveUnit(this.gameObject.GetHashCode());
        }
    }
}