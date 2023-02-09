#if UNITY_EDITOR
using System.IO;
using System.Text;

namespace QFramework.Pro
{
    public class ModelCodeDesignerTemplate
    {
        public static void Write(ArchitectureGraph architectureGraph, ModelNode modelNode)
        {
            var folderPath = (architectureGraph.ScriptsFolderPath + "/Model").CreateDirIfNotExists();
            var filePath = folderPath + $"/{modelNode.ClassName}.Designer.cs";

            var rootCode = new RootCode()
                .Using("QFramework")
                .Using("System")
                .EmptyLine()
                .Namespace(architectureGraph.Namespace,
                    (ns) =>
                    {
                        if (modelNode.WithInterface)
                        {
                            ns.CustomScope($"public partial interface I{modelNode.ClassName} : IModel", false,
                                (interfaceScope) =>
                                {
                                    foreach (var propertyInfo in modelNode.Properties)
                                    {
                                        interfaceScope.Custom(propertyInfo.Type.ToString().ToLower() + " " +
                                                              propertyInfo.Name +
                                                              " { get;set; }");
                                    }
                                });
                        }

                        ns.Class(modelNode.ClassName, $"I{modelNode.ClassName}", true, false, classScope =>
                        {
                            foreach (var propertyInfo in modelNode.Properties)
                            {
                                classScope.Custom(propertyInfo.AccessType.ToString().ToLower() + " " +
                                                  propertyInfo.Type.ToString().ToLower() + " " + propertyInfo.Name +
                                                  " { get;set; }");
                            }
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