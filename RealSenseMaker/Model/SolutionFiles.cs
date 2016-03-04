using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RealSenseMaker.Model
{
  internal class SolutionFiles
  {
    private const string Debug = "Debug";
    private const string Release = "Release";

    public void UpdateSolutions(string realsensePath, string path)
    {
      ProjectFiles projects = new ProjectFiles();
      string[] files = Directory.GetFiles(path, "*.sln", SearchOption.AllDirectories);
      foreach (string file in files)
      {
        string directory = Path.GetDirectoryName(file);
        new Builder().GetFiles(realsensePath, directory);
        var guidMaps = projects.AddLibraries(directory);
        // Now we need to go through the files updating the guid maps.
        UpdateSolutionFile(file, guidMaps);
      }
    }

    private static void UpdateSolutionFile(string solution, Dictionary<string, string> guidMaps)
    {
      string fileText = File.ReadAllText(solution);
      string text = GetSolutionContents(fileText, Debug);
      text = GetSolutionContents(text, Release);
      // Now, it's time to iterate over the files...
      foreach (var guid in guidMaps)
      {
        foreach (string guidMapping in guidMaps.Values)
        {
          text = GetActiveConfig(text, guidMapping, Debug, "ActiveCfg");
          text = GetActiveConfig(text, guidMapping, Debug, "Build.0");
          text = GetActiveConfig(text, guidMapping, Release, "ActiveCfg");
          text = GetActiveConfig(text, guidMapping, Release, "Build.0");
        }
      }
      File.WriteAllText(solution, text);
    }

    private static string GetActiveConfig(string fileText, string guid, string buildConfiguration, string cfg)
    {
      StringBuilder sb = new StringBuilder();
      string lookFor = string.Format("{0}.{1}|Any CPU.{2} = {1}|Any CPU", guid, buildConfiguration, cfg);
      string x86 = string.Format("\t\t{0}.{1}|x86.{2} = {1}|x86", guid, buildConfiguration, cfg);
      string x64 = string.Format("\t\t{0}.{1}|x64.{2} = {1}|x64", guid, buildConfiguration, cfg);
      string[] repl = fileText.Split(new[] { lookFor }, StringSplitOptions.RemoveEmptyEntries);
      if (repl.Length <= 0) return sb.ToString();
      sb.Append(repl[0]);
      sb.AppendLine(lookFor);
      sb.AppendLine(x86);
      sb.Append(x64);
      sb.Append(repl[1]);
      return sb.ToString();
    }

    private static string GetSolutionContents(string fileText, string buildConfiguration)
    {
      StringBuilder sb = new StringBuilder();
      var repl = fileText.Split(new[] { string.Format("{0}|Any CPU = {0}|Any CPU", buildConfiguration) }, StringSplitOptions.RemoveEmptyEntries);
      if (repl.Length <= 0) return sb.ToString();
      sb.Append(repl[0]);
      sb.AppendLine(string.Format("{0}|Any CPU = {0}|Any CPU", buildConfiguration));
      sb.AppendLine(string.Format("\t\t{0}|x64 = {0}|x64", buildConfiguration));
      sb.Append(string.Format("\t\t{0}|x86 = {0}|x86", buildConfiguration));
      sb.Append(repl[1]);
      return sb.ToString();
    }

  }
}
