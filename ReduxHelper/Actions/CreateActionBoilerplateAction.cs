using ReduxHelper.Factory;
using ReduxHelper.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxHelper.Actions
{
    public class CreateActionBoilerplateAction : IAction
    {
        public const string CsExtension = ".cs";
        public const string ActionsFolder = "Actions";
        public const string ActionFile = "Action.tmp";
        public const string ActionTargetFile = "Action.cs";
        public const string ActionFailureFile = "ActionFailure.tmp";
        public const string ActionFailureTargetFile = "ActionFailure.cs";
        public const string ActionSuccessFile = "ActionSuccess.tmp";
        public const string ActionSuccessTargetFile = "ActionSuccess.cs";
        public const string EffectsFolder = "Effects";
        public const string Effect = "Effect";
        public const string EffectFile = "Effect.tmp";
        public const string EffectTargetFile = "Effect.cs";
        public const string ReducersFolder = "Reducers";
        public const string ReducerFile = "Reducer.tmp";
        public const string ReducerTargetFile = "Reducer.cs";
        public const string SharedFolder = "Shared";
        public const string StateFolder = "State";

        public string Name => nameof(CreateActionBoilerplateAction);

        public async Task ExecuteAsync(ArgumentOptions options)
        {
            Log.Debug("Path: " + options.Path + " leads to computed path: " + options.PathComputed);
            if (!DirectoryFactory.TryCreateDirectoriesIfNotExists(options.PathComputed, "Actions", "Effects", "Reducers", Path.Combine("Actions", options.ActionName)))
                throw new DirectoryNotFoundException("Could not create directories for action");
            Dictionary<string, string> replacements = new Dictionary<string, string>()
      {
        {
          "namespace",
          this.GetNamespace(options.PathComputed)
        },
        {
          "actionName",
          options.ActionName
        },
        {
          "actionsFolder",
          "Actions"
        },
        {
          "effectsFolder",
          "Effects"
        },
        {
          "stateName",
          options.StateNameComputed
        },
        {
          "reducersFolder",
          "Reducers"
        },
        {
          "sharedNamespace",
          this.GetSharedNamespace(options.PathComputed)
        },
        {
          "stateNamespace",
          this.GetStateNamespace(options.PathComputed)
        }
      };
            FileTemplateFactory.CopyTemplate(this.Name, "Action.tmp", Path.Combine(options.PathComputed, "Actions", options.ActionName, options.ActionName + "Action.cs"), replacements);
            FileTemplateFactory.CopyTemplate(this.Name, "ActionFailure.tmp", Path.Combine(options.PathComputed, "Actions", options.ActionName, options.ActionName + "ActionFailure.cs"), replacements);
            FileTemplateFactory.CopyTemplate(this.Name, "ActionSuccess.tmp", Path.Combine(options.PathComputed, "Actions", options.ActionName, options.ActionName + "ActionSuccess.cs"), replacements);
            FileTemplateFactory.CopyTemplate(this.Name, "Effect.tmp", Path.Combine(options.PathComputed, "Effects", options.ActionName + "Effect.cs"), replacements);
            FileTemplateFactory.CopyTemplate(this.Name, "Reducer.tmp", Path.Combine(options.PathComputed, "Reducers", options.ActionName + "Reducer.cs"), replacements);
            await Task.CompletedTask;
        }

        public bool IsHandling(TemplateAction action) => action == TemplateAction.CreateAction;

        private string GetCustomNamespace(string path, string subFolder, int parents)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            for (int index = 0; index < parents; ++index)
            {
                if(directoryInfo.Parent == null)
                {
                    break;
                }
                directoryInfo = directoryInfo.Parent;
            }
                
            return this.GetNamespace(new DirectoryInfo(Path.Combine(directoryInfo.FullName, subFolder)).FullName);
        }

        private string GetSharedNamespace(string path) => GetCustomNamespace(path, Path.Combine("Shared", "Actions"), 1);

        private string GetStateNamespace(string path) => GetCustomNamespace(path, "State", 2);

        private string GetNamespace(string path, bool recursive = true, List<string>? subFolders = null)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo? fileInfo = Enumerable.FirstOrDefault(directoryInfo.GetFiles(), (file => file.Extension == ".cs"));
            if (fileInfo != null)
            {
                string? str1 = Enumerable.FirstOrDefault(Enumerable.Select(Enumerable.Where(File.ReadAllLines(fileInfo.FullName), (Func<string, bool>)(line => !string.IsNullOrWhiteSpace(line))), (Func<string, string>)(line => line.Trim())), (Func<string, bool>)(line => line.StartsWith("namespace")));
                if (!string.IsNullOrEmpty(str1))
                {
                    string str2 = Enumerable.First(Enumerable.Skip(str1.Split(" ", StringSplitOptions.None), 1));
                    foreach (string str3 in subFolders ?? new List<string>())
                        str2 = str2 + "." + str3;
                    return str2;
                }
            }
            if (directoryInfo.Root.FullName == directoryInfo.FullName || !recursive)
                throw new DirectoryNotFoundException("Could not find namespace");
            if (subFolders == null)
                subFolders = new List<string>();
            subFolders.Add(directoryInfo.Name);
            if(directoryInfo.Parent == null)
            {
                throw new NullReferenceException("parent directory can not be null");
            }
            return this.GetNamespace(directoryInfo.Parent.FullName, recursive, subFolders);
        }
    }
}
