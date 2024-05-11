using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PDAPI.Data;
using PDAPI.Extensions;

namespace PDAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IDbConnection _connection;
        private readonly IConfiguration _configuration;
        private readonly string UPLOAD_PATH = string.Empty;


        public UploadController(IWebHostEnvironment environment, IDbConnection connection, IConfiguration configuration)
        {
            this._environment = environment;
            this._connection = connection;
            this._configuration = configuration;
            UPLOAD_PATH = _configuration.GetValue<string>("Setting:UploadPath");
        }


        [Route("~/v1/upload")]
        [HttpPost]
        public async Task<IActionResult> Upload([ModelBinder(BinderType = typeof(JsonModelBinder))] 
                                                                WSParmContents wspc, IList<IFormFile> files)
        {
            APIResponse apiResponse;
            string storeId = wspc.str_no;
            string mac = wspc.mac;

            if (!Directory.Exists(Path.Combine(UPLOAD_PATH, storeId, mac)))
                Directory.CreateDirectory(Path.Combine(UPLOAD_PATH, storeId, mac));

            var size = files.Sum(f => f.Length);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var path = Path.Combine(UPLOAD_PATH, storeId, mac, file.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            apiResponse = new APIResponse() { flag = "5", msg = "上傳成功" };

            return Json(apiResponse);
        }
    }
}
