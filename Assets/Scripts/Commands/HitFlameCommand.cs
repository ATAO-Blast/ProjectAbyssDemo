using QFramework;
using UnityEngine;

namespace AbyssDemo
{
    public class HitFlameCommand : AbstractCommand
    {
        private RuntimeUnitInfo performUnit;
        private RuntimeUnitInfo targetUnit;
        public HitFlameCommand(RuntimeUnitInfo performUnit, RuntimeUnitInfo targetUnit)
        {
            this.performUnit = performUnit;
            this.targetUnit = targetUnit;
        }

        protected override void OnExecute()
        {
            
        }
    }
}
