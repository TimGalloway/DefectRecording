using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefectServer.Models
{
    public class Defect : BaseModel
    {
        public Int32 Id { get; set; }
        public String Location { get; set; }
        public String Description { get; set; }
        public String ImageName { get; set; }
        public String ImageBase64 { get; set; }
    }
}