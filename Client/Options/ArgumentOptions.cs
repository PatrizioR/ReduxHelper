using CommandLine.Text;
using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxHelper.Options
{
    public class ArgumentOptions
    {
        [Option('a', "action", HelpText = "Action to execute, default create action boilerplate", Required = false)]
        public TemplateAction Action { get; set; }

        [Option('p', "path", HelpText = "Working directory, current as default", Required = false)]
        public string? Path { get; set; }

        [Option('n', "name", HelpText = "Name for action", Required = true)]
        public string ActionName { get; set; } = null!;

        [Option('s', "state", HelpText = "Name of the state, current folder name as default", Required = false)]
        public string? StateName { get; set; }

        [Option('f', "force", HelpText = "Force executing action (e.g. overwrite files and folders)", Required = false)]
        public bool Force { get; set; }

        public string PathComputed => this.Path ?? Directory.GetCurrentDirectory();

        public string StateNameComputed => (this.StateName ?? new DirectoryInfo(this.PathComputed).Name) + "State";
    }
}
