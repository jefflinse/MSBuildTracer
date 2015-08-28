﻿using System;
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

        public void Trace(MBEX.ProjectTargetInstance target, int traceLevel = 0)
        {
            if (target == null)
            {
                return;
            }

            this.PrintTargetInfo(target, traceLevel);

            if (!string.IsNullOrWhiteSpace(target.DependsOnTargets))
            {
                foreach (var dependency in target.Dependencies(project))
                {
                    Trace(dependency, traceLevel + 1);
                }
            }
        }

        private void PrintTargetInfo(MBEX.ProjectTargetInstance target, int indentCount)
        {
            var indent = indentCount > 1 ? new StringBuilder().Insert(0, "|   ", indentCount - 1).ToString() : "";
            var tree = indentCount > 0 ? "|   " : "";

            Console.WriteLine($"{indent}{tree}{target.Name}");
        }

        public static bool TargetNameMatchesPattern(string targetName, string pattern)
        {
            return new Regex($"^{pattern.Replace("*", ".*")}$", RegexOptions.IgnoreCase).Match(targetName).Success;
        }
    }
}