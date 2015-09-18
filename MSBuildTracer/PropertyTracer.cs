using System;
using System.Linq;
using System.Text.RegularExpressions;

using MBEV = Microsoft.Build.Evaluation;

namespace MSBuildTracer
{
    class PropertyTracer
    {
        private MBEV.Project project;

        public PropertyTracer(MBEV.Project project)
        {
            this.project = project;
        }

        public void TraceAll(string query)
        {
            var properties = project.AllEvaluatedProperties.Where(
                p => PropertyTracer.PropertyNameMatchesPattern(p.Name, query) &&
                !p.IsPredecessor(project));

            if (properties.Any())
            {
                foreach (var property in properties)
                {
                    Utils.WriteLineColor($"[{property.Name}]", ConsoleColor.Cyan);
                    PropertyTracer.Trace(property);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine($"No property matching '{query}' is defined anywhere for this project.");
            }
        }

        public static void Trace(MBEV.ProjectProperty property, int traceLevel = 0)
        {
            if (property == null || property.IsGlobalProperty || property.IsReservedProperty)
            {
                return;
            }

            PrintPropertyInfo(property, traceLevel);

            if (property.IsImported)
            {
                Trace(property.Predecessor, traceLevel + 1);
            }
        }

        private static void PrintPropertyInfo(MBEV.ProjectProperty property, int indentCount)
        {
            var indent = new string('\t', indentCount);
            string location;

            if (property.IsEnvironmentProperty)
            {
                location = "(environment)";
            }
            else if (property.IsReservedProperty)
            {
                location = "(reserved)";
            }
            else
            {
                location = $"{property.Xml.Location.File}:{property.Xml.Location.Line}";
            }

            Utils.WriteColor($"{indent}Loc:  ", ConsoleColor.White);
            Utils.WriteLineColor(location, ConsoleColor.DarkCyan);

            if (property.UnevaluatedValue == property.EvaluatedValue)
            {
                Utils.WriteColor($"{indent}Val:  ", ConsoleColor.White);
                Utils.WriteLineColor(property.EvaluatedValue, ConsoleColor.Green);
            }
            else
            {
                Utils.WriteColor($"{indent}Val:  ", ConsoleColor.White);
                Utils.WriteLineColor(property.UnevaluatedValue, ConsoleColor.DarkGreen);
                Utils.WriteColor($"{indent}      ", ConsoleColor.White);
                Utils.WriteLineColor(property.EvaluatedValue, ConsoleColor.Green);
            }
        }

        public static bool PropertyNameMatchesPattern(string propertyName, string pattern)
        {
            return new Regex($"^{pattern.Replace("*", ".*")}$", RegexOptions.IgnoreCase).Match(propertyName).Success;
        }
    }
}
