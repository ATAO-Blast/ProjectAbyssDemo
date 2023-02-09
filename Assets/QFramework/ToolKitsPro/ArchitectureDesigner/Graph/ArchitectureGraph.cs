#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace QFramework.Pro
{
    // 版本迭代
    [CreateAssetMenu(menuName = "@ArchitectureDesigner/Create Graph", fileName = "Architecture Graph")]
    public class ArchitectureGraph : IMGUIGraph
    {
        public string Namespace;

        public new string Name;

        // Model
        // System
        //  Command
        //  Event
        // Model
        // Utility

        public List<CommandNode> Commands => nodes.Where(n => n is CommandNode).Cast<CommandNode>().ToList();
        public List<ModelNode> Models => nodes.Where(n => n is ModelNode).Cast<ModelNode>().ToList();


        private void OnValidate()
        {
            if (Namespace.IsNullOrEmpty() && Name.IsNullOrEmpty())
            {
                Namespace = AssetDatabase.GetAssetPath(this).GetFolderPath().GetFileName();
                Name = AssetDatabase.GetAssetPath(this).GetFolderPath().GetFileName();
            }
        }

        public string ScriptsFolderPath => $"Assets/{Name}/Scripts";
    }

    [CustomEditor(typeof(ArchitectureGraph), false)]
    public class ArchitectureGraphInspector : IMGUIGlobalGraphInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Code"))
            {
                ArchitectureDesigner.GenerateCode(target as ArchitectureGraph);
            }
        }
    }

    [CustomNodeGraphEditor(typeof(ArchitectureGraph))]
    public class ArchitectureGraphEditor : IMGUIGraphEditor
    {
        public override void OnGUI()
        {
            GUILayout.Toolbar(-1, new string[] { "1", "2" });
            base.OnGUI();
        }
    }
}
#endif