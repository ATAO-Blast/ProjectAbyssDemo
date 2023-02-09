using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace AbyssDemo
{
    public interface IRuntimeUnitModel : IModel
    {
        RuntimeUnitInfo GetRuntimeUnit(int hash);
        void AddRuntimeUnit(int hash, RuntimeUnitInfo info);
        void RemoveRuntimeUnit(int hash);
    }
    public class RuntimeUnitModel : AbstractModel,IRuntimeUnitModel
    {
        private Dictionary<int, RuntimeUnitInfo> runtimeUnitDic = new Dictionary<int, RuntimeUnitInfo>();

        protected override void OnInit()
        {
            
        }
        public RuntimeUnitInfo GetRuntimeUnit(int hash)
        {
            if(runtimeUnitDic.TryGetValue(hash, out var info)) return info;
            else return null;
        }
        
        public void AddRuntimeUnit(int hash, RuntimeUnitInfo info)
        {
            if(runtimeUnitDic.ContainsKey(hash))
            {
                runtimeUnitDic[hash] = info;
            }
            else runtimeUnitDic.Add(hash, info);
        }
        public void RemoveRuntimeUnit(int hash)
        {
            if (runtimeUnitDic.ContainsKey(hash))
            {
                runtimeUnitDic.Remove(hash);
            }
        }
    }
}