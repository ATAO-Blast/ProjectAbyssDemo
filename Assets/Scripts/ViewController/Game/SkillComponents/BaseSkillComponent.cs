using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace AbyssDemo
{
    public class BaseSkillComponent : MonoBehaviour, IController
    {
        [SerializeField] private int priority = 0;
        [SerializeField] ElementType elementType = ElementType.Physics;
        private IUnitSkillModel unitSkillModel;
        public IArchitecture GetArchitecture()
        {
            return AbyssDemoArchi.Interface;
        }

        // Start is called before the first frame update
        void Start()
        {
            unitSkillModel = this.GetModel<IUnitSkillModel>();
            unitSkillModel.AddSkill(this.gameObject.GetHashCode(), new BasicUnitSkill(
                nameof(BasicUnitSkill),
                priority,
                SkillTarget.Opposite,
                SkillSelect.Once,
                SkillTargetState.Surviving,
                SkillDmgType.Close,
                elementType
                ));
        }


        private void OnDestroy()
        {
            unitSkillModel.RemoveUnit(this.gameObject.GetHashCode());
        }
    }
}