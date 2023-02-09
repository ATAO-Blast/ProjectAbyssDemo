using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.Linq;

namespace AbyssDemo
{
    public interface IUnitSkillModel : IModel
    {
        List<IUnitSkill> GetUnitSkillList(int unitHash);
        void AddSkill(int unitHash,IUnitSkill skill);
        void RemoveUnit(int unitHash);
    }
    public class UnitSkillModel : AbstractModel, IUnitSkillModel
    {
        private Dictionary<int, List<IUnitSkill>> unitSkillsDictionary = new Dictionary<int, List<IUnitSkill>>();
         
        
        protected override void OnInit()
        {
            
        }
        public List<IUnitSkill> GetUnitSkillList(int unitHash)
        {
            if (unitSkillsDictionary.TryGetValue(unitHash, out var skill)) return skill;
            else return null;
        }
        public void AddSkill(int unitHash, IUnitSkill skill)
        {
            if (unitSkillsDictionary.ContainsKey(unitHash))
            {
                var unitSkills= unitSkillsDictionary[unitHash];
                if (unitSkills != null)
                {
                    if(unitSkills.Contains(skill)) return;
                    else unitSkills.Add(skill);
                }
                else
                {
                    unitSkillsDictionary[unitHash] = new List<IUnitSkill>() { skill};
                }
            }
            else unitSkillsDictionary.Add(unitHash, new List<IUnitSkill>() { skill});
        }
        public void RemoveUnit(int unitHash)
        {
            if (unitSkillsDictionary.ContainsKey(unitHash))
            {
                var unitSkills = unitSkillsDictionary[unitHash];
                if (unitSkills != null)
                {
                    unitSkills.Clear();
                }
                unitSkillsDictionary.Remove(unitHash);
            }
        }
    }
    
}