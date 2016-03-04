using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using RealSenseMaker.Model;

namespace RealSenseMaker.ViewModel
{
  public class BrowsableViewModelBase : ViewModelBase
  {
    private ICommand browseCommand;
    private string folder;
    private readonly IBrowseFoldersDialog browseFolders;

    public BrowsableViewModelBase()
    {
      browseFolders = SimpleIoc.Default.GetInstance<IBrowseFoldersDialog>();
    }

    public ICommand BrowseCommand => browseCommand ?? (browseCommand = new RelayCommand(BrowseForFolder));

    public string Folder
    {
      get { return folder; }
      set
      {
        if (folder == value) return;
        folder = value;
        RaisePropertyChanged();
      }
    }

    public bool IsValid()
    {
      if (string.IsNullOrWhiteSpace(folder)) return false;
      return Directory.Exists(folder);
    }

    private void BrowseForFolder()
    {
      string browsed = browseFolders.Open(Folder);
      if (!string.IsNullOrWhiteSpace(browsed))
      {
        Folder = browsed;
      }
    }
  }
}