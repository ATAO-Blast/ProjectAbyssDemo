#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;

using UnityEditor;

using UnityEngine;

namespace QFramework.Pro
{
    [CreateNodeMenu("Model")]
    public class ModelNode : IMGUIGraphNode
    {
        [SerializeField] public string Name;
        public bool WithInterface = true;

        public List<PropertyInfo> Properties;

        [HideInInspector] public ModelGraph ModelGraph;


        [System.Serializable]
        public class PropertyInfo
        {
            public enum AccessTypes
            {
                Public,
                Private,
            }

            public enum Types
            {
                Int,
                String,
                Float,
                Double,
                Custom,
            }
#if UNITY_EDITOR
            public AccessType AccessType;
#endif
            public Types Type;

            public string Name;
        }

        public string ClassName => Name.EndsWith("Model") ? Name : Name + "Model";

        private void OnValidate()
        {
            if (Name.IsNotNullAndEmpty())
            {
                name = Name + "Model";
            }

            if (ModelGraph == null)
            {
                ModelGraph = CreateInstance<ModelGraph>();
                ModelGraph.Model = this;
#if UNITY_EDITOR
                AssetDatabase.AddObjectToAsset(ModelGraph, graph);
#endif
            }

            ModelGraph.name = ClassName + "Graph";
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ModelNode))]
    public class ModelNodeInspector : IMGUIGlobalGraphNodeInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.ObjectField("ModelGraph", target.As<ModelNode>().ModelGraph, typeof(ModelGraph), false);
        }
    }

    [CustomNodeEditor(typeof(ModelNode))]
    public class ModelNodeView : IMGUIGraphNodeEditor
    {
        public override void OnHeaderGUI()
        {
            if (target.As<ModelNode>().Name.IsNotNullAndEmpty())
            {
                GUILayout.Label(target.As<ModelNode>().Name + " Model",
                    IMGUIGraphResources.styles.NodeHeader,
                    GUILayout.Height(30));
            }
            else
            {
                GUILayout.Label(target.As<ModelNode>().Name,
                    IMGUIGraphResources.styles.NodeHeader,
                    GUILayout.Height(30));
            }
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (GUILayout.Button("Open Graph"))
            {
                IMGUIGraphWindow.OpenWithGraph(target.As<ModelNode>().ModelGraph);
            }
        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            base.AddContextMenuItems(menu);

            if (Selection.objects.Length == 1 && Selection.activeObject is ModelNode)
            {
                var node = Selection.activeObject as ModelNode;

                menu.AddItem(new GUIContent("Open "), false, () =>
                {
                    IMGUIGraphWindow.OpenWithGraph(node.ModelGraph);
                    // var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
                    // AssetDatabase.OpenAsset(monoScript);
                });


                var filePath = (node.graph as ArchitectureGraph).ScriptsFolderPath + $"/Model/{node.ClassName}.cs";

                if (File.Exists(filePath))
                {
                    menu.AddItem(new GUIContent("Open " + node.ClassName + ".cs"), false, () =>
                    {
                        var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
                        AssetDatabase.OpenAsset(monoScript);
                    });
                }
            }
        }
    }
#endif
}
#endif