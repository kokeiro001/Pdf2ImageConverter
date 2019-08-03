using Ghostscript.NET.Processor;
using System;
using System.Collections.Generic;
using System.IO;

// C:\Program Files (x86)\gs\gs9.27

namespace Pdf2ImageConverter
{
    class PdfConverter
    {
        public enum ImageFormat
        {
            Png,
            Jpeg,
        }

        public void ExtractImages(string inputFile, string outputFile, ImageFormat imageFormat)
        {
            var outputDirectory = Path.GetDirectoryName(outputFile);

            DirectoryUtility.DeleteDirectoryIfExists(outputDirectory);

            Directory.CreateDirectory(outputDirectory);

            using (GhostscriptProcessor ghostscript = new GhostscriptProcessor())
            {
                ghostscript.Processing += new GhostscriptProcessorProcessingEventHandler(ghostscript_Processing);

                List<string> switches = new List<string>();
                switches.Add("-empty");
                switches.Add("-dSAFER");
                switches.Add("-dBATCH");
                switches.Add("-dNOPAUSE");
                switches.Add("-dNOPROMPT");

                switch (imageFormat)
                {
                    case ImageFormat.Png:
                        switches.Add("-sDEVICE=png16m");
                        switches.Add("-dTextAlphaBits=4");
                        switches.Add("-dGraphicsAlphaBits=4");
                        break;
                    case ImageFormat.Jpeg:
                        switches.Add("-sDEVICE=jpeg");
                        switches.Add("-dJPEGQ=100");
                        break;
                    default:
                        throw new Exception($"not support ImageFormat:{imageFormat}");
                }
                switches.Add("-r200");

                switches.Add(@"-sOutputFile=" + outputFile);
                switches.Add(@"-f");
                switches.Add(inputFile);

                ghostscript.Process(switches.ToArray());
            }
        }

        void ghostscript_Processing(object sender, GhostscriptProcessorProcessingEventArgs e)
        {
            Console.WriteLine($"{e.CurrentPage}/{e.TotalPages}");
        }
    }
}
