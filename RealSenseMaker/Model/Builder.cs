using System.IO;

namespace RealSenseMaker.Model
{
  internal class Builder
  {
    private const string ManagedFile = "libpxcclr.cs.dll";
    private const string UnmanagedFile = "libpxccpp2c.dll";

    public void GetFiles(string realsenseFolder, string directory)
    {
      SdkFolders folders = new SdkFolders(realsenseFolder);
      GetAndCopy(folders.X64, @"Libs\x64", directory);
      GetAndCopy(folders.X86, @"Libs\x86", directory);
    }

    private static void GetAndCopy(string source, string destination, string directory)
    {
      destination = Path.Combine(directory, destination);
      Directory.CreateDirectory(destination);
      string path1 = Path.Combine(source, ManagedFile);
      if (File.Exists(Path.Combine(destination, ManagedFile)))
      {
        return;
      }
      File.Copy(path1, Path.Combine(destination, ManagedFile));
      string path2 = Path.Combine(source, UnmanagedFile);
      File.Copy(path2, Path.Combine(destination, UnmanagedFile));
    }
  }
}
