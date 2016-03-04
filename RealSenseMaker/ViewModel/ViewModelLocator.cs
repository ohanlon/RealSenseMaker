/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:RealSenseMaker"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using RealSenseMaker.Model;

namespace RealSenseMaker.ViewModel
{
  /// <summary>
  /// This class contains static references to all the view models in the
  /// application and provides an entry point for the bindings.
  /// </summary>
  public class ViewModelLocator
  {
    /// <summary>
    /// Initializes a new instance of the ViewModelLocator class.
    /// </summary>
    public ViewModelLocator()
    {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

      SimpleIoc.Default.Register<BrowsableViewModelBase>();
      SimpleIoc.Default.Register<SourceFolderBrowserViewModel>();
      SimpleIoc.Default.Register<RealSenseFolderBrowserViewModel>();
      SimpleIoc.Default.Register<MainWindowViewModel>();
      SimpleIoc.Default.Register<ConfigurationInformation>();
      SimpleIoc.Default.Register<IBrowseFoldersDialog, BrowseFoldersDialog>();
    }

    public MainWindowViewModel Main => ServiceLocator.Current.GetInstance<MainWindowViewModel>();

    public SourceFolderBrowserViewModel SourceFolder => ServiceLocator.Current.GetInstance<SourceFolderBrowserViewModel>();

    public RealSenseFolderBrowserViewModel RealSenseFolder
      => ServiceLocator.Current.GetInstance<RealSenseFolderBrowserViewModel>();

    public static void Cleanup()
    {
    }
  }
}