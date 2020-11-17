using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Pdf2ImageConverter
{
    class Program
    {
        static void Main()
        {
            var tmpPdfFilePath = @"C:\tmp\Pdf2ImageConverter\original.pdf";
            var tmpDirectory = Path.GetDirectoryName(tmpPdfFilePath);

            DirectoryUtility.CreateDirectoryIfNotExists(tmpDirectory);

            var pdfConverter = new PdfConverter();

            var sourceDirectries = new string[] 
            {


            };
            var filePaths = sourceDirectries
                .SelectMany(x => Directory.GetFiles(x, "*.pdf", SearchOption.TopDirectoryOnly));

            var imageFormat = PdfConverter.ImageFormat.Jpeg;
            var extension = imageFormat.ToString().ToLower();

            foreach (var sourceFilePath in filePaths)
            {
                Console.WriteLine($"copy {sourceFilePath} -> {tmpPdfFilePath}");
                // ライブラリが日本語ファイルを扱えないため一時ファイルとしてコピーしてから作業する
                File.Copy(sourceFilePath, tmpPdfFilePath, true);

                var tmpOutputDirectory = Path.Combine(Path.GetDirectoryName(tmpPdfFilePath), extension);

                var tmpOutputFile = Path.Combine(tmpOutputDirectory, $"%03d.{extension}");

                Console.WriteLine($"image extract from pdf");
                pdfConverter.ExtractImages(tmpPdfFilePath, tmpOutputFile, imageFormat);

                var sourceFileDirectory = Path.GetDirectoryName(sourceFilePath);
                var sourceFileName = Path.GetFileNameWithoutExtension(sourceFilePath);

                var extractImagesDirectoryPath = Path.Combine(sourceFileDirectory, sourceFileName);

                DirectoryUtility.DeleteDirectoryIfExists(extractImagesDirectoryPath);

                DirectoryUtility.Copy(tmpOutputDirectory, extractImagesDirectoryPath, true);

                var zipOutputPath = Path.Combine(sourceFileDirectory, $"{sourceFileName}.zip");

                Console.WriteLine($"CreateZip: {zipOutputPath}");

                if (File.Exists(zipOutputPath))
                {
                    File.Delete(zipOutputPath);
                }
                ZipFile.CreateFromDirectory(extractImagesDirectoryPath, zipOutputPath);

                DirectoryUtility.DeleteDirectoryIfExists(extractImagesDirectoryPath);
            }

            if (File.Exists(tmpPdfFilePath))
            {
                File.Delete(tmpPdfFilePath);
            }
        }
    }
}
