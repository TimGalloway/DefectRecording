using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DefectServer.Models
{
    public class BaseModel
    {
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}