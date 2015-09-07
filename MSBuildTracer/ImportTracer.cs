using System;
using System.Collections.Generic;
using System.Text;
using MBEV = Microsoft.Build.Evaluation;

namespace MSBuildTracer
{
    class ImportTracer
    {
        private readonly MBEV.Project project;
        public bool ShowFullPath { get; set; }

        public ImportTracer(MBEV.Project project)
        {
            this.project = project;
        }

        public void Trace()
        {
            IDictionary<string, int> indents = new Dictionary<string, int>();
            int maxDepth = 0;
            foreach (var import in project.Imports)
            {
                string file = import.ImportingElement.ContainingProject.Location.File;
                if (!indents.ContainsKey(file))
                {
                    indents[file] = maxDepth++;
                }

                int indent = indents[file];
                string path = import.ImportingElement.Project;
                if (ShowFullPath)
                {
                    path = import.ImportedProject.Location.File;
                }

                PrintImportInfo(path, indent);
            }
        }

        private void PrintImportInfo(string importProject, int indentCount)
        {
            var indent = indentCount > 1 ? new StringBuilder().Insert(0, "|   ", indentCount - 1).ToString() : "";
            var tree = indentCount > 0 ? "|   " : "";

            Console.WriteLine($"{indent}{tree}{importProject}");
        }
    }
}
