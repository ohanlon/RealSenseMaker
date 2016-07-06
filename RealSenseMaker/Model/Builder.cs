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
      GetAndCopy(folders.X86, @"Libs\AnyCPU", directory);
    }

    private static void GetAndCopy(string source, string destination, string directory)
    {
      destination = Path.Combine(directory, destination);
      Directory.CreateDirectory(destination);
      string path1 = Path.Combine(source, ManagedFile);
      string destinationFile = Path.Combine(destination, ManagedFile);
      string unmanagedFile = Path.Combine(destination, UnmanagedFile);
      if (File.Exists(destinationFile))
      {
        // We're going to overwrite the file here.
        File.Delete(destinationFile);
        File.Delete(unmanagedFile);
      }
      File.Copy(path1, destinationFile);
      string path2 = Path.Combine(source, UnmanagedFile);
      File.Copy(path2, unmanagedFile);
    }
  }
}
