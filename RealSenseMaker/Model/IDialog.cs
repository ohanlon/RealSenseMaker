using System.Windows.Forms;

namespace RealSenseMaker.Model
{
  public interface IBrowseFoldersDialog
  {
    string Open(string path);
  }

  public class BrowseFoldersDialog : IBrowseFoldersDialog
  {
    public string Open(string path)
    {
      FolderBrowserDialog folderBrowser = new FolderBrowserDialog()
      {
        ShowNewFolderButton = false,
        SelectedPath = path
      };
      try
      {
        DialogResult result = folderBrowser.ShowDialog();
        return result == DialogResult.OK ? folderBrowser.SelectedPath : string.Empty;
      }
      finally
      {
        folderBrowser.Dispose();
      }
    }
  }
}
