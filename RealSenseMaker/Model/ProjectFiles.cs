using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RealSenseMaker.Model
{
  internal class ProjectFiles
  {
    private const string PropertyGroup = "PropertyGroup";
    private const string Reference = "Reference";
    private const string ItemGroup = "ItemGroup";

    public Dictionary<string, string> AddLibraries(string directory)
    {
      Dictionary<string, string> projectGuidsMap = new Dictionary<string, string>();
      // Okay, look for all CSPROJ files...
      string[] files = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);
      if (files.Length == 0) return projectGuidsMap;
      foreach (string file in files)
      {
        // First of all, we need to open the project up
        XElement document = XElement.Load(file);
        // Now, see if we have loaded our custom ItemProj in...
        XNamespace ns = document.GetDefaultNamespace();
        if (HasProjectPreviouslyBeenUpdated(document, ns)) continue;

        string guid = ProjectGuid(document, ns);
        if (!string.IsNullOrWhiteSpace(guid))
        {
          projectGuidsMap.Add(file, guid);
        }
        BuildProjectGroups(document, ns, "x86", true);
        BuildProjectGroups(document, ns, "x86", false);
        BuildProjectGroups(document, ns, "x64", true);
        BuildProjectGroups(document, ns, "x64", false);
        BuildItemGroup(document, ns);
        BuildImport(document, ns);
        document.Save(file);
      }

      return projectGuidsMap;
    }

    private static bool HasProjectPreviouslyBeenUpdated(XElement document, XNamespace ns)
    {
      return (
        from p in document.Elements(ns + ItemGroup).Elements(ns + Reference)
        where p.Attribute("Include").Value == "libpxcclr.cs"
        select p).Any();
    }

    private static void BuildProjectGroups(XContainer document, XNamespace ns, string platform, bool isDebug)
    {
      // What we are building here is the XML fragment that makes up the x86 and x64 debug/release property groups
      // Effectively, we are looking to build these:
      //  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Release|x86'">
      //    <OutputPath>bin\x86\Release\</OutputPath>
      //    <DefineConstants>TRACE</DefineConstants>
      //    <DebugType>pdbonly</DebugType>
      //    <PlatformTarget>x86</PlatformTarget>
      //    <ErrorReport>prompt</ErrorReport>
      //    <CodeAnalysisRuleSet>MinimumRecommendedRules.ruleset</CodeAnalysisRuleSet>
      //    <Prefer32Bit>true</Prefer32Bit>
      //  </PropertyGroup>
      //  <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Debug|x64'">
      //    <OutputPath>bin\x64\Debug\</OutputPath>
      //    <DefineConstants>DEBUG;TRACE</DefineConstants>
      //    <DebugType>full</DebugType>
      //    <PlatformTarget>x64</PlatformTarget>
      //    <ErrorReport>prompt</ErrorReport>
      //    <CodeAnalysisRuleSet>MinimumRecommendedRules.ruleset</CodeAnalysisRuleSet>
      //    <Prefer32Bit>true</Prefer32Bit>
      //    <DebugSymbols>true</DebugSymbols>
      //  </PropertyGroup>
      // So, we know that certain elements depend on whether or not we are building the Debug configuration.
      // This makes our job fairly simple here. All we have to do is build up a list of those items that depend
      // on whether or not we are building the debug version and we will inject them into our XML.
      string constants = isDebug ? "DEBUG;TRACE" : "TRACE";
      string outputPath = isDebug ? @"bin\{0}\Debug\" : @"bin\{0}\Release\";
      string condition = isDebug ? "'$(Configuration)|$(Platform)' == 'Debug|{0}'" : "'$(Configuration)|$(Platform)' == 'Release|{0}'";
      string debugType = isDebug ? "full" : "pdbonly";

      XElement propertyGroup = (from p in document.Elements()
                                where p.Name.LocalName == PropertyGroup
                                select p).Last();

      XElement newPropertyGroup = new XElement(ns + PropertyGroup,
        new XAttribute("Condition", string.Format(condition, platform)),
        new XElement(ns + "OutputPath", new XText(string.Format(outputPath, platform))),
        new XElement(ns + "DefineConstants", new XText(string.Format(constants, platform))),
        new XElement(ns + "DebugType", new XText(debugType)),
        new XElement(ns + "PlatformTarget", new XText(platform)),
        new XElement(ns + "ErrorReport", new XText("prompt")),
        new XElement(ns + "CodeAnalysisRuleSet", new XText("MinimumRecommendedRules.ruleset")),
        new XElement(ns + "Prefer32Bit", new XText("true")));

      if (isDebug)
      {
        // The debug settings typically have this attribute - which is
        // missing in other types.
        newPropertyGroup.Add(new XElement(ns + "DebugSymbols", new XText("true")));
      }

      propertyGroup.AddBeforeSelf(newPropertyGroup);
    }

    private static void BuildImport(XContainer document, XNamespace ns)
    {
      XElement target = (from p in document.Elements()
                         where p.Name.LocalName == "Import"
                         select p).Last();
      // Now, let's add our post build event
      XElement postBuild = new XElement(ns + PropertyGroup,
        new XElement(ns + "PostBuildEvent",
          new XText(@"COPY $(SolutionDir)libs\$(PlatformName)\libpxccpp2c.dll $(TargetDir)")));
      target.AddAfterSelf(postBuild);
    }

    private static void BuildItemGroup(XContainer document, XNamespace ns)
    {
      XElement propertyGroup = (from p in document.Elements()
                                where p.Name.LocalName == PropertyGroup
                                select p).Last();
      XElement childElement = new XElement(ns + ItemGroup,
        new XElement(ns + Reference, new XAttribute("Include", "libpxcclr.cs"),
          new XElement(ns + "HintPath",
            new XText(@"$(SolutionDir)Libs\$(Platform)\libpxcclr.cs.dll"))));
      propertyGroup.AddAfterSelf(childElement);
    }

    private static string ProjectGuid(XContainer container, XNamespace ns)
    {
      return (from p in container.Elements(ns + PropertyGroup).Elements()
                  where p.Name.LocalName == "ProjectGuid"
                  select p.Value).SingleOrDefault();
    }
  }
}