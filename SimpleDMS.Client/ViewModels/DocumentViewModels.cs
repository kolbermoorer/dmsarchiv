using SimpleDMS.Client.ViewModels;
using SimpleDMS.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDMS.Client.Models
{
    public class Item
    {
        public string Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public bool Children { get; set; }
        public string ParentId { get; set; }
        public int ChildCount { get; set; }
        public string Icon { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DocumentDate { get; set; }
        public string Editor { get; set; }
        public string Version { get; set; }
        public string Guid { get; set; }
        public Mask Metadata { get; set; }

    }

    public class TreeNode : Item
    {
        public string Text { get; set; }
        public Dictionary<string, object> Li_attr { get; set; }
    }

    public class IntrayItem : Item
    {
        public string IconBase64 { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
