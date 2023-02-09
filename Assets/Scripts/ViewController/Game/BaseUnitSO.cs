using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using Unity.VisualScripting;

namespace AbyssDemo
{
    [CreateAssetMenu(fileName ="NewUnit",menuName ="@@CreatUnit")]
    public class BaseUnitSO : ScriptableObject
    {
        public new string name;
        [TextArea]
        public string description;
        public float actionWaitTime = 3f;
        [Range(1,6)]
        public int bodySize = 1;
        public int level = 1;
        public float HP = 100;
        public int MP = 100;
        public float currectedValue = 1;
        public float ATK = 10;
        public float DEF = 20;
        public float moveSpeed = 10f;
        public int exp = 50;
        public float speed = 100;

        [Range(0f, 1f)] public float dodge = 0;
        [Range(0f, 1f)] public float ridiculeSuccessRate = 0.5f;

        [Header("Element Resistance")]

        [Range(0f, 1f)] public float flameResis = 0;
        [Range(0f, 1f)] public float frostResis = 0;
        [Range(0f, 1f)] public float boltResis = 0;
        [Range(0f, 1f)] public float poisionResis = 0;
        [Range(0f, 1f)] public float chaosResis = 0;
        [Header("Skills")]
        public string[] skills = new string[1] {"BaseSkill"};

        public string DisplaySOInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<size=36>").Append(GetColorType()).Append(name).Append("</color></size>").AppendLine(Environment.NewLine);
            sb.Append("生命值：\t").Append(HP).AppendLine();
            sb.Append("攻击力：\t").Append(ATK).AppendLine();
            sb.Append("防御力：\t").Append(DEF).AppendLine();
            sb.Append("速度：\t").Append(speed).AppendLine();
            if (skills.Length > 0) 
            {
                sb.Append("技能：").AppendLine();
                foreach (string skill in skills)
                {
                    sb.Append(skill).AppendLine();
                }
            }
            sb.Append("战力：").Append(GetCombatPower());
            sb.AppendLine(Environment.NewLine);
            sb.Append(description);
            return sb.ToString();
        }
        string GetColorType()
        {
            if (level == 1) return "<color=white>";
            else if (level == 2) return "<color=green>";
            else if (level == 3) return "<color=blue>";
            else if (level == 4) return "<color=#ff00ffff>";
            else if (level == 5) return "<color=#ffff00ff>";
            else if (level == 6) return "<color=#ffff00ff>";
            else return null;
        }
        int GetCombatPower()
        {
            return Mathf.FloorToInt(HP * (ATK * 100 / speed + (DEF - 3 * Mathf.Pow(1.5f, level - 1))) * currectedValue / Mathf.Pow(1.5f, level) / 10 + 0.5f);
        }
    }
}