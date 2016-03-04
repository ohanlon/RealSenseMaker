using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using RealSenseMaker.Model;

namespace RealSenseMaker.ViewModel
{
  public class MainWindowViewModel : ViewModelBase
  {
    private ICommand transformCommand;
    private readonly RealSenseFolderBrowserViewModel realSense;
    private readonly SourceFolderBrowserViewModel sourceFolder;
    private readonly ConfigurationInformation configurationInformation;
    private string message;

    public MainWindowViewModel()
    {
      realSense = SimpleIoc.Default.GetInstance<RealSenseFolderBrowserViewModel>();
      sourceFolder = SimpleIoc.Default.GetInstance<SourceFolderBrowserViewModel>();
      configurationInformation = SimpleIoc.Default.GetInstance<ConfigurationInformation>();

      configurationInformation.LoadConfiguration();
    }

    public ICommand TransformCommand => transformCommand ?? (transformCommand = new RelayCommand(TransformReferences, CanTransformReferences));

    public string Message
    {
      get { return message; }
      set
      {
        message = value;
        RaisePropertyChanged();
      }
    }

    private void TransformReferences()
    {
      Message = "Starting";
      new SolutionFiles().UpdateSolutions(realSense.Folder, sourceFolder.Folder);

      configurationInformation.SaveConfiguration();
      Message = "Done";
    }

    private bool CanTransformReferences()
    {
      return realSense.IsValid() && sourceFolder.IsValid();
    }
  }
}
