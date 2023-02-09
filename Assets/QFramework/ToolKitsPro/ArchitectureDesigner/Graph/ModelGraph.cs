#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace QFramework.Pro
{
    public class ModelGraph : IMGUIGraph
    {
        public ModelNode Model;
    }

    [CustomNodeGraphEditor(typeof(ModelGraph))]
    public class ModelGraphEditor : IMGUIGraphEditor
    {
        public override void AddContextMenuItems(GenericMenu menu)
        {
            Vector2 pos = IMGUIGraphWindow.current.WindowToGridPosition(Event.current.mousePosition);

            // base.AddContextMenuItems(menu);

            menu.AddItem(new GUIContent("SimpleClass"), false, () =>
            {
                var node = CreateNode(typeof(SimpleClassNode), pos);
                IMGUIGraphWindow.current.AutoConnect(node);
            });

            menu.AddItem(new GUIContent("返回"), false,
                () => { IMGUIGraphWindow.OpenWithGraph(target.As<ModelGraph>().Model.graph); });
        }
    }
}
#endif
