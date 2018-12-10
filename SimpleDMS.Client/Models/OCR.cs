using IronOcr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDMS.Client.Models
{
    public class OCR
    {
        public string AddDocumentToFulltext(string id)
        {
            string file = SimpleServerExtensionMethods.GetFilePath(id);

            var ocr = new AdvancedOcr()
            {
                CleanBackgroundNoise = true,
                EnhanceContrast = true,
                EnhanceResolution = true,
                Language = IronOcr.Languages.German.OcrLanguagePack,
                Strategy = IronOcr.AdvancedOcr.OcrStrategy.Advanced,
                ColorSpace = AdvancedOcr.OcrColorSpace.Color,
                DetectWhiteTextOnDarkBackgrounds = true,
                InputImageType = AdvancedOcr.InputTypes.AutoDetect,
                RotateAndStraighten = true,
                ReadBarCodes = false,
                ColorDepth = 4
            };

            var results = ocr.Read(file);

            _saveFullText(file, results.Text);

            return results.Text;
        }

        private static void _saveFullText(string file, string fullText)
        {
            var textFile = Path.ChangeExtension(file, ".txt").Replace("basis", "fulltext");

            (new FileInfo(textFile)).Directory.Create();
            File.WriteAllText(textFile, fullText);
        }

        public void RemoveDocumentFromFulltext(string id)
        {
            string file = SimpleServerExtensionMethods.GetFilePath(id);
            var textFile = Path.ChangeExtension(file, ".txt").Replace("basis", "fulltext");
            File.Delete(textFile);
        }
    }
}
