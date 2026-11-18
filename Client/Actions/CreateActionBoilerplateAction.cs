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
        public const string TemplateExtension = ".templ";
        public const string CsExtension = ".cs";
        public const string ActionsFolder = "Actions";
        public const string ActionFile = $"Action{TemplateExtension}";
        public const string ActionTargetFile = "Action.cs";
        public const string ActionFailureFile = $"ActionFailure{TemplateExtension}";
        public const string ActionFailureTargetFile = "ActionFailure.cs";
        public const string ActionSuccessFile = $"ActionSuccess{TemplateExtension}";
        public const string ActionSuccessTargetFile = "ActionSuccess.cs";
        public const string EffectsFolder = "Effects";
        public const string Effect = "Effect";
        public const string EffectFile = $"Effect{TemplateExtension}";
        public const string EffectTargetFile = "Effect.cs";
        public const string ReducersFolder = "Reducers";
        public const string ReducerFile = $"Reducer{TemplateExtension}";
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
          ActionsFolder
        },
        {
          "effectsFolder",
          EffectsFolder
        },
        {
          "stateName",
          options.StateNameComputed
        },
        {
          "reducersFolder",
         ReducersFolder
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
            FileTemplateFactory.CopyTemplate(this.Name, ActionFile, Path.Combine(options.PathComputed, "Actions", options.ActionName, options.ActionName + ActionTargetFile), replacements);
            FileTemplateFactory.CopyTemplate(this.Name, ActionFailureFile, Path.Combine(options.PathComputed, "Actions", options.ActionName, options.ActionName + ActionFailureTargetFile), replacements);
            FileTemplateFactory.CopyTemplate(this.Name, ActionSuccessFile, Path.Combine(options.PathComputed, "Actions", options.ActionName, options.ActionName + ActionSuccessTargetFile), replacements);
            FileTemplateFactory.CopyTemplate(this.Name, EffectFile, Path.Combine(options.PathComputed, "Effects", options.ActionName + EffectTargetFile), replacements);
            FileTemplateFactory.CopyTemplate(this.Name, ReducerFile, Path.Combine(options.PathComputed, "Reducers", options.ActionName + ReducerTargetFile), replacements);
            await Task.CompletedTask;
        }

        public bool IsHandling(TemplateAction action) => action == TemplateAction.CreateAction;

        private string GetCustomNamespace(string path, string subFolder, int parents)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            for (int index = 0; index < parents; ++index)
            {
                if (directoryInfo.Parent == null)
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
            if (directoryInfo.Parent == null)
            {
                throw new NullReferenceException("parent directory can not be null");
            }
            return this.GetNamespace(directoryInfo.Parent.FullName, recursive, subFolders);
        }
    }
}
