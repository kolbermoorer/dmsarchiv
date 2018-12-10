using System.Collections.Generic;

namespace SimpleDMS.Client.ViewModels
{
    public class Mask
    {
        public int Maskno { get; set; }
        public string Maskname { get; set; }
        public string Maskindex { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }

    public class Metadata
    {
        public int ParentId { get; set; }
        public string FieldKey { get; set; }
        public string FieldData { get; set; }
        public int LineType { get; set; }
        public int MaskNo { get; set; }
    }

    public class masklines
    {
        public int Maskno { get; set; }
        public int Mlineno { get; set; }
        public int Linetype { get; set; }
        public string Linebez { get; set; }
        public string Linekey { get; set; }
        public int Linemin { get; set; }
        public int Linemax { get; set; }
        public string Linedefault { get; set; }
    }
}
