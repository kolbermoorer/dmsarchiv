using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDMS.Client.Models
{
    public static class DocumentExtensionMethods
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool MergePDFs(List<string> files)
        {
            string intrayPath = SimpleServerExtensionMethods.GetIntrayPath("DMSArchiv");
            string firstFileName = Path.GetFileName(files.First());
            string targetPath = Path.Combine(intrayPath, firstFileName);

            try
            {
                using (PdfDocument targetDoc = new PdfDocument())
                {
                    foreach (string fileName in files)
                    {
                        using (PdfDocument pdfDoc = PdfReader.Open(fileName, PdfDocumentOpenMode.Import))
                            for (int i = 0; i < pdfDoc.PageCount; i++)
                                targetDoc.AddPage(pdfDoc.Pages[i]);

                        File.Delete(fileName);
                    }
                    targetDoc.Save(targetPath);
                }
                return true;
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return false;
            }  
        }

        public static bool SplitPDFs(List<string> files)
        {
            string intrayPath = SimpleServerExtensionMethods.GetIntrayPath("DMSArchiv");
            string firstFileName = Path.GetFileName(files.First());

            int pageNumber;

            try
            {
                foreach (string fileName in files)
                {
                    pageNumber = 1;
                    string pathName = Path.GetDirectoryName(fileName);
                    string fileNameOnly = Path.Combine(pathName, Path.GetFileNameWithoutExtension(fileName));
                    
                    using (PdfDocument pdfDoc = PdfReader.Open(fileName, PdfDocumentOpenMode.Import))
                        for (int i = 0; i < pdfDoc.PageCount; i++)
                            using (PdfDocument targetDoc = new PdfDocument())
                            {
                                targetDoc.AddPage(pdfDoc.Pages[i]);
                                string targetPath = fileName.Replace(fileNameOnly, fileNameOnly + "_" + pageNumber.ToString().PadLeft(4, '0'));
                                targetDoc.Save(targetPath);
                                pageNumber++;
                            }
                }
                    

                foreach (string pdf in files)
                    File.Delete(pdf);

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return false;
            }
        }
    }
}
