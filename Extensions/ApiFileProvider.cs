using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace PDAPI.Extensions
{
    public class ApiFileProvider : PhysicalFileProvider
    {
        /// <summary>
        /// 別名
        /// </summary>
        public string Alias { get; set; }

        public ApiFileProvider(string root, string alias) :  base(root)
        {
            this.Alias = alias;
        }

        public ApiFileProvider(string root, ExclusionFilters filters, string alias): base(root, filters)
        {
            this.Alias = alias;
        }
    }
}
