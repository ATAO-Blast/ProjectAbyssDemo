#if UNITY_EDITOR
using System.IO;
using System.Text;

namespace QFramework.Pro
{
    public class CommandCodeTemplate
    {
        public static void Write(ArchitectureGraph architectureGraph, CommandNode commandNode)
        {
            var folderPath = (architectureGraph.ScriptsFolderPath + "/Command").CreateDirIfNotExists();
            var filePath = folderPath + $"/{commandNode.ClassName}.cs";

            if (File.Exists(filePath)) return;
            
            var rootCode = new RootCode()
                .Using("QFramework")
                .Using("System")
                .EmptyLine()
                .Namespace(architectureGraph.Namespace,
                    (ns) =>
                    {
                        ns.Class(commandNode.ClassName, "AbstractCommand", false, false, classScope =>
                        {
                            classScope.CustomScope("protected override void OnExecute()", false, (method) =>
                            {
                                method.EmptyLine();
                            });
                        });
                    });
            
            var stringWriter = new StringBuilder();
            rootCode.Gen(new StringCodeWriter(stringWriter));
            
            using (var fileWriter = File.CreateText(filePath))
            {
                rootCode.Gen(new FileCodeWriter(fileWriter));
            }
        }
    }
}
#endif