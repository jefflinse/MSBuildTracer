using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using MBEV = Microsoft.Build.Evaluation;
using MBEX = Microsoft.Build.Execution;

namespace MSBuildTracer
{
    static class Extensions
    {
        /// <summary>
        /// Determines whether this property is a predecessor to another definition in the project.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="project">The project to look in</param>
        /// <returns></returns>
        public static bool IsPredecessor(this MBEV.ProjectProperty property, MBEV.Project project)
        {
            return project.AllEvaluatedProperties.Any(p => p.Predecessor == property);
        }

        /// <summary>
        /// Gets a collection of ProjectTargetInstances that this target is dependent on in a given project.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="project">The project to look in</param>
        /// <returns></returns>
        public static IEnumerable<MBEX.ProjectTargetInstance> Dependencies(
            this MBEX.ProjectTargetInstance target, MBEV.Project project)
        {
            var dependencies = new List<MBEX.ProjectTargetInstance>();
            var dependencyTargetNames = project.ResolveAllProperties(target.DependsOnTargets)
                                       .Replace(Environment.NewLine, "")
                                       .Split(';');

            foreach (var name in dependencyTargetNames)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    dependencies.Add(project.Targets[name.Trim()]);
                }
            }

            return dependencies;
        }

        /// <summary>
        /// Fully resolves all properties in a given string.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="input">The string to resolve</param>
        /// <returns></returns>
        public static string ResolveAllProperties(this MBEV.Project project, string input)
        {
            var resolvedString = input;
            var propertyRegex = new Regex(@"\$\(([\w\d_]+)\)", RegexOptions.Singleline);
            Match match;

            while ((match = propertyRegex.Match(resolvedString)).Success)
            {
                while (match.Success)
                {
                    resolvedString = resolvedString.Replace($"$({match.Groups[1].Value})", project.GetPropertyValue(match.Groups[1].Value));
                    match = match.NextMatch();
                }
            }

            return resolvedString;
        }

        public static IEnumerable<MBEV.ResolvedImport> Children(this MBEV.ResolvedImport import, MBEV.Project project)
        {
            return project.Imports.Where(
                i => string.Equals(i.ImportingElement.ContainingProject.FullPath,
                                   project.ResolveAllProperties(import.ImportedProject.Location.File),
                                   StringComparison.OrdinalIgnoreCase));
        }

        public static string ReducedCondition(this MBEV.ResolvedImport import)
        {
            return Regex.Replace(import.ImportingElement.Condition, @"Exists\('.+'\)", "the file exists", RegexOptions.IgnoreCase);
        }
    }
}
