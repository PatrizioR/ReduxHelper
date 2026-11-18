using ReduxHelper.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxHelper.Actions
{
    public interface IAction
    {
        string Name { get; }

        Task ExecuteAsync(ArgumentOptions options);

        bool IsHandling(TemplateAction action);
    }
}
