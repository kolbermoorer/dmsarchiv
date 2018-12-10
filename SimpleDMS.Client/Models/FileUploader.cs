using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace SimpleDMS.Client.Models
{
    public class FileUploader
    {
        public void SaveFile(HttpFile file, string page)
        {
            string fileName = file.Name;
            Stream stream = file.Value;

            string path = string.Empty;

            if (page.Contains("Intray") == false)
                path = @"D:\SimpleDMS\archive\DMSArchiv\basis\UPR000000";
            else
                path = @"D:\SimpleDMS\intray\DMSArchiv";

            using (var fileStream = File.Create(Path.Combine(path, fileName)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }
    }
}
