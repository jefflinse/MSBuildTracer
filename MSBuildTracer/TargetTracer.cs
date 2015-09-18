using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MBEV = Microsoft.Build.Evaluation;
using MBEX = Microsoft.Build.Execution;

namespace MSBuildTracer
{
    class TargetTracer
    {
        private MBEV.Project project;

        public TargetTracer(MBEV.Project project)
        {
            this.project = project;
        }

        public void TraceAll(string query)
        {
            var targets = project.Targets.Where(t => TargetTracer.TargetNameMatchesPattern(t.Key, query)).Select(t => t.Value);

            if (targets.Any())
            {
                foreach (var target in targets)
                {
                    Trace(target);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine($"No target matching '{query}' is defined anywhere for this project.");
            }
        }

        private void Trace(MBEX.ProjectTargetInstance target, int traceLevel = 0)
        {
            if (target == null)
            {
                return;
            }

            PrintTargetInfo(target, traceLevel);

            if (!string.IsNullOrWhiteSpace(target.DependsOnTargets))
            {
                foreach (var dependency in target.Dependencies(project))
                {
                    Trace(dependency, traceLevel + 1);
                }
            }
        }

        private static void PrintTargetInfo(MBEX.ProjectTargetInstance target, int indentCount)
        {
            var indent = indentCount > 1 ? new StringBuilder().Insert(0, "|   ", indentCount - 1).ToString() : "";
            var tree = indentCount > 0 ? "|   " : "";
            var targetColor = indentCount > 0 ? (target.Name.StartsWith("_") ? ConsoleColor.DarkGreen : ConsoleColor.Green) : ConsoleColor.Cyan;

            Utils.WriteColor(indent + tree, ConsoleColor.White);
            Utils.WriteLineColor(target.Name, targetColor);
        }

        private static bool TargetNameMatchesPattern(string targetName, string pattern)
        {
            return new Regex($"^{pattern.Replace("*", ".*")}$", RegexOptions.IgnoreCase).Match(targetName).Success;
        }
    }
}
