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
            var dependencyValues = ExpandValues(target.DependsOnTargets, project);

            foreach (var name in dependencyValues)
            {
                dependencies.Add(project.Targets[name]);
            }

            return dependencies;
        }

        private static IEnumerable<string> ExpandValues(string dependencyValue, MBEV.Project project)
        {
            var retval = new List<string>();

            foreach (var potentialProperty in dependencyValue.Split(';'))
            {
                var value = potentialProperty.Replace(Environment.NewLine, "").Trim();

                // recursively expand properties and add their values to the list
                var propertyRegex = new Regex(@"^\$\(([\w\d_]+)\)$");
                var match = propertyRegex.Match(value);
                if (match.Success)
                {
                    var rawPropertyValue = project.GetPropertyValue(match.Groups[1].Value);
                    retval.AddRange(ExpandValues(rawPropertyValue, project));
                }
                else
                {
                    retval.Add(value);
                }
            }

            return retval;
        }
    }
}
