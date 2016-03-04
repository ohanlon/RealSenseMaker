using System.IO;

namespace RealSenseMaker.Model
{
  internal class SdkFolders
  {
    private readonly string baseDirectory;
    public SdkFolders(string folder)
    {
      baseDirectory = folder;
    }

    public string X86 => Path.Combine(baseDirectory, @"bin\win32");
    public string X64 => Path.Combine(baseDirectory, @"bin\x64");
  }
}
