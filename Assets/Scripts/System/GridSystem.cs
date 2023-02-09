using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbyssDemo
{
    public class GridSystem  : AbstractSystem
    {
        private GridCell heroGridCell;
        private GridCell enemyGridCell;

        public GridCell HeroGridCell => heroGridCell;
        public GridCell EnemyGridCell => enemyGridCell;
        protected override void OnInit()
        {
            heroGridCell = new GridCell(3, 4, 1.5f, new Vector3(-6, -2, 0));
            enemyGridCell = new GridCell(3, 4, 1.5f, new Vector3(1.5f, -2, 0));
        }

        
    }
}