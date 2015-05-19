using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.Framework.Logging;
using Roslyn.Utilities;

namespace OmniSharp.Services
{
    public class MEFCodeActionProvider : ICodeActionProvider
    {
        private ICodeRefactoringService service;
        private ILogger logger;

        public MEFCodeActionProvider(OmnisharpWorkspace workspace, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.Create<MEFCodeActionProvider>();
            logger.WriteInformation("Loading MEF CodeRefactorings");
            logger.WriteInformation("workspace null? " + (workspace == null).ToString());
            logger.WriteInformation("services null? " + (workspace.Services == null).ToString());
            this.service = workspace.Services.GetService<ICodeRefactoringService>();
            logger.WriteInformation("did shit fall over? - " + (service == null));
        }

        public IEnumerable<CodeRefactoringProvider> GetProviders()
        {
            return this.service.LazyProviders.Select(l => l.Value);
        }
    }

    internal interface ICodeRefactoringService : IWorkspaceService
    {
        IEnumerable<Lazy<CodeRefactoringProvider>> LazyProviders { get; set; }
    }

    [ExportWorkspaceService(typeof(ICodeRefactoringService)), Shared]
    internal class CodeRefactoringService : ICodeRefactoringService
    {
        public IEnumerable<Lazy<CodeRefactoringProvider>> LazyProviders { get; set; }

        [ImportingConstructor]
        public CodeRefactoringService(
            [ImportMany] IEnumerable<Lazy<CodeRefactoringProvider>> providers)
        {
            this.LazyProviders = providers;
        }
    }
}
