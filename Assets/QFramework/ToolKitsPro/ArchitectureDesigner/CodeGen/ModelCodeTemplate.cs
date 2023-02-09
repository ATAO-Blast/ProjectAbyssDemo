#if UNITY_EDITOR
using System.IO;
using System.Text;

namespace QFramework.Pro
{
    public class ModelCodeTemplate
    {
        public static void Write(ArchitectureGraph architectureGraph, ModelNode modelNode)
        {
            var folderPath = (architectureGraph.ScriptsFolderPath + "/Model").CreateDirIfNotExists();
            var filePath = folderPath + $"/{modelNode.ClassName}.cs";

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
                                (interfaceScope) => { });
                        }

                        ns.Class(modelNode.ClassName, "AbstractModel", modelNode.WithInterface , false, classScope =>
                        {
                            foreach (var propertyInfo in modelNode.Properties)
                            {
                                classScope.Custom(propertyInfo.AccessType.ToString().ToLower() + " " +
                                                  propertyInfo.Type.ToString().ToLower() + " " + propertyInfo.Name +
                                                  " {get;set;}");
                            }

                            classScope.CustomScope("protected override void OnInit()", false,
                                method => { method.EmptyLine(); });
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