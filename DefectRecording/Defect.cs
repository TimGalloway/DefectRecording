using System.Collections.Generic;

namespace DefectRecording
{
    public class Defect
    {
        public string id { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public string ImageBase64 { get; set; }
    }
}