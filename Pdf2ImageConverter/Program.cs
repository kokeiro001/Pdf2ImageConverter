using System.IO;
using System.Linq;

namespace Pdf2ImageConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var tmpPdfFilePath = @"C:\tmp\Pdf2ImageConverter\original.pdf";
            var tmpDirectory = Path.GetDirectoryName(tmpPdfFilePath);

            if (!Directory.Exists(tmpDirectory))
            {
                Directory.CreateDirectory(tmpDirectory);
            }

            var pdfConverter = new PdfConverter();

            var sourceDirectries = new string[] 
            {


            };
            var filePaths = sourceDirectries
                .SelectMany(x => Directory.GetFiles(x, "*.pdf", SearchOption.TopDirectoryOnly));

            var imageFormat = PdfConverter.ImageFormat.Png;
            var extension = imageFormat.ToString().ToLower();

            foreach (var sourceFilePath in filePaths)
            {
                // 日本語ファイルを扱えないため一時ファイルとしてコピーしてから作業する
                File.Copy(sourceFilePath, tmpPdfFilePath, true);

                var tmpOutputDirectory = Path.Combine(Path.GetDirectoryName(tmpPdfFilePath), extension);

                var outputFile = Path.Combine(tmpOutputDirectory, $"%03d.{extension}");

                pdfConverter.ExtractImages(tmpPdfFilePath, outputFile, imageFormat);

                var sourceFileDirectory = Path.GetDirectoryName(sourceFilePath);
                var sourceFileName = Path.GetFileNameWithoutExtension(sourceFilePath);

                var outputDirectory = Path.Combine(sourceFileDirectory, sourceFileName);

                if (Directory.Exists(outputDirectory))
                {
                    Directory.Delete(outputDirectory, true);
                }

                DirectoryCopy(tmpOutputDirectory, outputDirectory, true);
            }

            if (File.Exists(tmpPdfFilePath))
            {
                File.Delete(tmpPdfFilePath);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
