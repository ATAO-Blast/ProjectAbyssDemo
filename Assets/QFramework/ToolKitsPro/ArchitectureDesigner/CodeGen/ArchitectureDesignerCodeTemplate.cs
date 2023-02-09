#if UNITY_EDITOR
using System.IO;

using System.Text;
using QFramework.Pro;

namespace QFramework
{
    public class ArchitectureDesignerCodeTemplate
    {
        public static void Write(ArchitectureGraph architectureGraph)
        {
            var folderPath = architectureGraph.ScriptsFolderPath.CreateDirIfNotExists();
            
            var filePath = folderPath + $"/{architectureGraph.Name}.Designer.cs";

            
            var rootCode = new RootCode()
                .Using("QFramework")
                .Using("System")
                .EmptyLine()
                .Namespace(architectureGraph.Namespace,
                    (ns) =>
                    {
                        ns.Class(architectureGraph.Name, $"Architecture<{architectureGraph.Name}>", true, false, classScope =>
                        {
                            classScope.CustomScope("protected override void Init()", false, method =>
                            {
                                foreach (var modelInfo in architectureGraph.Models)
                                {
                                    if (modelInfo.WithInterface)
                                    {
                                        method.Custom(
                                            $"this.RegisterModel<I{modelInfo.ClassName}>(new {modelInfo.ClassName}());");
                                    }
                                    else
                                    {
                                        method.Custom($"this.RegisterModel(new {modelInfo.ClassName}());");

                                    }
                                }

                                method.Custom("OnInit();");
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