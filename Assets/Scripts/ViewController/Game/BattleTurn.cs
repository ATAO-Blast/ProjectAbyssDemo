using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    [System.Serializable]
    public class BattleTurn
    {
        public RuntimeUnitInfo goToAttack;
        public RuntimeUnitInfo GoToAttack { get { return goToAttack; } }
        public BattleTurn(RuntimeUnitInfo goToAttack)
        {
            this.goToAttack = goToAttack;
        }
    }
}