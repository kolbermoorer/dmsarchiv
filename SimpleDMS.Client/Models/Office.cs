using Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Office.Core;
using System;

namespace SimpleDMS.Client.Models
{
    public class Office
    {
        private string _hexValue;
        private string _hexFolder;
        private string _archive;
        private string _filePath;

        public Office(string archive, string filePath, string hexValue, string hexFolder)
        {
            this._archive = archive;
            this._filePath = filePath;
            this._hexValue = hexValue;
            this._hexFolder = hexFolder;
        }


        public string CreatePDF()
        {
            return _createPDF();
        }

        private string _createPDF()
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();

            // C# doesn't have optional arguments so we'll need a dummy value
            object oMissing = System.Reflection.Missing.Value;

            word.Visible = false;
            word.ScreenUpdating = false;

            //FileInfo wordFile = _filePath;

            // Cast as Object for word Open method
            //Object filename = (Object)wordFile.FullName;
            Object filename = _filePath;

            // Use the dummy value as a placeholder for optional arguments
            Document doc = word.Documents.Open(ref filename, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            doc.Activate();

            //object outputFileName = wordFile.FullName.Replace(".doc", ".pdf");
            object outputFileName = Path.Combine(SimpleServerExtensionMethods.GetArchivePath(_archive), "preview", _hexFolder, _hexValue + ".pdf");
            object fileFormat = WdSaveFormat.wdFormatPDF;

            // Save document into PDF Format
            doc.SaveAs(ref outputFileName,
                ref fileFormat, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            object saveChanges = WdSaveOptions.wdDoNotSaveChanges;
            ((_Document)doc).Close(ref saveChanges, ref oMissing, ref oMissing);
            doc = null;

            ((_Application)word).Quit(ref oMissing, ref oMissing, ref oMissing);
            word = null;

            return outputFileName.ToString();
        }
    }
}
