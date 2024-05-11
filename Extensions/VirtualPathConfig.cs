using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDAPI.Extensions
{
    public class VirtualPathConfig
    {
        public List<PathContent> VirtualPath { get; set; }
    }

    public class PathContent
    {
        public string RealPath { get; set; }
        public string RequestPath { get; set; }
        public string Alias { get; set; }
    }
}
