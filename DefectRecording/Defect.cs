using System.Collections.Generic;

namespace DefectRecording
{
    public class Defect
    {
        public string id { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string ImageName { get; set; }
        public string ImageBase64 { get; set; }
    }
}