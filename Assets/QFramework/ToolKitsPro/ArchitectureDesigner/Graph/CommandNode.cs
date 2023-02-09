#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework.Pro
{
    [CreateNodeMenu("Command")]
    public class CommandNode : IMGUIGraphNode
    {
        [SerializeField] public string Name;

        private void OnValidate()
        {
            if (Name.IsNotNullAndEmpty())
            {
                name = Name + "Command";
            }
        }

        public string ClassName => Name.EndsWith("Command") ? Name : Name + "Command";
    }

    [CustomNodeEditor(typeof(CommandNode))]
    public class CommandNodeView : IMGUIGraphNodeEditor
    {
        public override void OnHeaderGUI()
        {
            if (target.As<CommandNode>().Name.IsNotNullAndEmpty())
            {
                GUILayout.Label(target.As<CommandNode>().Name + " Command",
                    IMGUIGraphResources.styles.NodeHeader,
                    GUILayout.Height(30));
            }
            else
            {
                GUILayout.Label(target.As<CommandNode>().Name,
                    IMGUIGraphResources.styles.NodeHeader,
                    GUILayout.Height(30));
            }
        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            base.AddContextMenuItems(menu);

            if (Selection.objects.Length == 1 && Selection.activeObject is CommandNode)
            {
                var node = Selection.activeObject as CommandNode;

                if (node.graph is ArchitectureGraph)
                {
                    var filePath = (node.graph as ArchitectureGraph).ScriptsFolderPath +
                                   $"/Command/{node.ClassName}.cs";

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
    }
}
#endif