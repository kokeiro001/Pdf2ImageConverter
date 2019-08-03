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
                // 日本語ファイルを扱えないため一時ファイルとしてコピーしてから作業する
                File.Copy(sourceFilePath, tmpPdfFilePath, true);

                var tmpOutputDirectory = Path.Combine(Path.GetDirectoryName(tmpPdfFilePath), extension);

                var tmpOutputFile = Path.Combine(tmpOutputDirectory, $"%03d.{extension}");

                pdfConverter.ExtractImages(tmpPdfFilePath, tmpOutputFile, imageFormat);

                var sourceFileDirectory = Path.GetDirectoryName(sourceFilePath);
                var sourceFileName = Path.GetFileNameWithoutExtension(sourceFilePath);

                var outputDirectory = Path.Combine(sourceFileDirectory, sourceFileName + "_jpg");

                DirectoryUtility.DeleteDirectoryIfExists(outputDirectory);

                DirectoryUtility.Copy(tmpOutputDirectory, outputDirectory, true);
            }

            if (File.Exists(tmpPdfFilePath))
            {
                File.Delete(tmpPdfFilePath);
            }
        }
    }
}
