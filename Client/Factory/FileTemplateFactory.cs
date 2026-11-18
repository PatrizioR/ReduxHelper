using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReduxHelper.Factory
{
    public static class FileTemplateFactory
    {
        public const string TemplatesFolder = "Templates";

        public static void CopyTemplate(
          string actionsFolder,
          string templateFilename,
          string destination,
          Dictionary<string, string>? replacements,
          bool overwrite = false)
        {
            if (File.Exists(destination) && !overwrite)
                return;
            StringBuilder stringBuilder = new StringBuilder(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "", "Templates", actionsFolder, templateFilename)));
            foreach (KeyValuePair<string, string> keyValuePair in replacements ?? new Dictionary<string, string>())
                stringBuilder.Replace("{" + keyValuePair.Key + "}", keyValuePair.Value);
            File.WriteAllText(destination, stringBuilder.ToString());
        }
    }
}
