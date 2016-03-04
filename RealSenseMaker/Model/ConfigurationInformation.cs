using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GalaSoft.MvvmLight.Ioc;
using RealSenseMaker.ViewModel;

namespace RealSenseMaker.Model
{
  public class ConfigurationInformation
  {
    private const string RealSensePath = "RealSensePath";
    private const string LastSource = "LastSource";

    private readonly RealSenseFolderBrowserViewModel realSense;
    private readonly SourceFolderBrowserViewModel sourceFolder;

    public ConfigurationInformation()
    {
      realSense = SimpleIoc.Default.GetInstance<RealSenseFolderBrowserViewModel>();
      sourceFolder = SimpleIoc.Default.GetInstance<SourceFolderBrowserViewModel>();
    }

    public void SaveConfiguration()
    {
      XDocument document = new XDocument();
      document.Add(new XElement("Configuration",
        new XElement(RealSensePath, new XText(realSense.Folder)),
        new XElement(LastSource, new XText(sourceFolder.Folder))));

      document.Save(ConfigurationPath);
    }

    public void LoadConfiguration()
    {
      string config = ConfigurationPath;
      if (!File.Exists(config)) return;

      XDocument document = XDocument.Load(config);
      IEnumerable<XElement> configuration = from p in document.Elements("Configuration")
        select p;
      if (!configuration.Any()) return;
      realSense.Folder = configuration.Elements(RealSensePath).First().Value;
      sourceFolder.Folder = configuration.Elements(LastSource).First().Value;
    }

    private string ConfigurationPath => Path.Combine(ConfigurationFolder(), "RealSenseMaker.Settings.xml");

    private string ConfigurationFolder()
    {
      string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RealSenseMaker");
      Directory.CreateDirectory(path);
      return path;
    }
  }
}
