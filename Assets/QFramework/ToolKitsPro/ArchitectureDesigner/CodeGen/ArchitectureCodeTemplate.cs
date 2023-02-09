#if UNITY_EDITOR
using System.IO;
using System.Text;

namespace QFramework.Pro
{
    public class ArchitectureCodeTemplate
    {
        public static void Write(ArchitectureGraph architectureGraph)
        {
            var folderPath = architectureGraph.ScriptsFolderPath.CreateDirIfNotExists();
            var filePath = folderPath + $"/{architectureGraph.Name}.cs";

            var rootCode = new RootCode()
                .Using("QFramework")
                .Using("System")
                .EmptyLine()
                .Namespace(architectureGraph.Namespace,
                    (ns) =>
                    {
                        ns.Class(architectureGraph.Name, "", true, false,
                            classScope =>
                            {
                                classScope.CustomScope("void OnInit()", false, method => { method.EmptyLine(); });
                            });
                    });

            var stringWriter = new StringBuilder();
            rootCode.Gen(new StringCodeWriter(stringWriter));

            if (!File.Exists(filePath))
            {
                using (var fileWriter = File.CreateText(filePath))
                {
                    rootCode.Gen(new FileCodeWriter(fileWriter));
                }
            }
        }
    }
}
#endif