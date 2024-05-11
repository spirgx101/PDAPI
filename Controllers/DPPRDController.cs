using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PDAPI.Data;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Globalization;
using PDAPI.Models;
using PDAPI.DataRepositories;
using System.Data.SQLite;
using Ionic.Zip;

namespace PDAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class DPPRDController : Controller
    {
        private readonly IDbConnection _connection;
        private readonly IConfiguration _configuration;
        private readonly string DOWNLOAD_PATH = string.Empty;

        public DPPRDController(IDbConnectionFactory dbConnectionFactory, IConfiguration configuration)
        {
            this._connection = new DbCSCRepositoryBase(dbConnectionFactory).DbConnection;
            this._configuration = configuration;
            DOWNLOAD_PATH = _configuration.GetValue<string>("Setting:DownloadPath");
        }

        [Route("~/v1/dpprd")]
        [HttpPost]
        public async Task<IActionResult> POST([FromBody] WSParmContents wspc)
        {
            APIResponse apiResponse = null;
            if (wspc.fcode.Trim() != "1")
            {
                apiResponse = new APIResponse() { flag = "-1", msg = "fcode錯誤。" };
                return Json(apiResponse);
            }

            if (wspc.file_name.Trim().ToUpper() != "DPPRD.DAT")
            {
                apiResponse = new APIResponse() { flag = "-2", msg = "下傳的檔案錯誤。" };
                return Json(apiResponse);  
            }

            //沒有傳入檔案日期，自動新增一筆
            if (wspc.last_date.Trim() == string.Empty || DateTime.TryParseExact(wspc.last_date, "yyyyMMdd_HHmmss", null, DateTimeStyles.None, out _) == false)
            {
                wspc.last_date = "19110101_000000";
            }

            string storeId = wspc.str_no;

            if (!Directory.Exists(Path.Combine(DOWNLOAD_PATH, storeId)))
                Directory.CreateDirectory(Path.Combine(DOWNLOAD_PATH, storeId));

            string dpprd_file = "DPPRD.DB";
            string dpprd_path = Path.Combine(DOWNLOAD_PATH, storeId, dpprd_file);
            string zip_name = "DPPRD.ZIP";
            string zip_path = Path.Combine(DOWNLOAD_PATH, storeId, zip_name);

            if (System.IO.File.Exists(zip_path))
            {
                string pda_crt_date = wspc.last_date;
                string server_crt_date = System.IO.File.GetLastWriteTime(zip_path).ToString("yyyyMMdd_HHmmss");
                if (string.Compare(server_crt_date, pda_crt_date) > 0)
                {
                    apiResponse = new APIResponse() { flag = "5", msg = $"http://{this.Request.Host.ToString().Trim()}/v1/dpprd/{wspc.str_no}" };
                    return Json(apiResponse);
                }
            }

            var sql = "_sp_csc_1O_GenData";
            var para = new DynamicParameters();
            para.Add("@StoreID", value: storeId);
            para.Add("@retcode", dbType: DbType.Int32, direction: ParameterDirection.Output);
            para.Add("@PScript", value: false, dbType: DbType.Boolean);

            await _connection.ExecuteAsync(sql, para, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            Int32 retcode = para.Get<Int32>("@retcode");


            DataTable dpprd = new DataTable();
            sql = "_sp_csc_1O_dnPrdt";
            para = new DynamicParameters();
            para.Add("@StoreID", value: storeId);
            para.Add("@DType", value: "1", dbType: DbType.String);
            para.Add("@PScript", value: false, dbType: DbType.Boolean);

            var dr_dpprd = await _connection.ExecuteReaderAsync(sql, para, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            dpprd.Load(dr_dpprd);

            DataTable odpprd = new DataTable();
            sql = "_sp_csc_1O_dnPrdt";
            para = new DynamicParameters();
            para.Add("@StoreID", value: storeId);
            para.Add("@DType", value: "2", dbType: DbType.String);
            para.Add("@PScript", value: false, dbType: DbType.Boolean);

            var dr_odpprd = await _connection.ExecuteReaderAsync(sql, para, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            odpprd.Load(dr_odpprd);

            CreateDPPRD(dpprd, odpprd, dpprd_path);

            Compressed(new string[] { dpprd_path }, zip_path);

            apiResponse = new APIResponse() { flag = "5", msg = $"http://{this.Request.Host.ToString().Trim()}/v1/dpprd/{wspc.str_no}"};
            return Json(apiResponse);
        }

        [Route("~/v1/[controller]/{store_id}")]
        [HttpGet]
        public async Task<IActionResult> GET(string store_id)
        {
            string file_name = "DPPRD.ZIP";
            string file_path = Path.Combine(DOWNLOAD_PATH, store_id, file_name);

            return await TransFile(file_path, file_name);
        }

        private bool CreateDPPRD(DataTable tb_dpprd, DataTable tb_odpprd, string file_path)
        {
            bool is_success = false;

            try
            {
                if (System.IO.File.Exists(file_path)) System.IO.File.Delete(file_path);

                using (IDbConnection cn = new SQLiteConnection(@"data source=" + file_path))
                {
                    cn.Open();

                    cn.Execute(@"
                        CREATE TABLE DPPRD (
                            vcitocod  char(13),
                            prdtcode  char(13),
                            plu_no    char(13),
                            dockcode  char(8),
                            vcitqty   char(2),
                            prdtslpr  char(8),
                            prdtcisd  char(1),
                            batno     char(4),
                            indc      char(8),
                            vcittype  char(5),
                            onpack    char(1),
                            freshfood char(1)
                        );

                        CREATE TABLE ODPPRD (
                            vcitocod  char(13),
                            prdtcode  char(13),
                            plu_no    char(13),
                            space     char(1),
                            indc1     char(1),
                            indc2     char(1),
                            sup_no    char(5),
                            vcitqty   char(2),
                            prdtslpr  char(8),
                            prdtcisd  char(1),
                            prdtmlqy  char(7),
                            prdtmiqy  char(7)
                        );
                    ");


                    using (var tran = cn.BeginTransaction())
                    {
                        foreach (DataRow dr in tb_dpprd.Rows)
                        {
                            string cmd = @"
                            INSERT INTO DPPRD VALUES (@vcitocod, @prdtcode, @plu_no, @dockcode, 
                                @vcitqty, @prdtslpr, @prdtcisd, @batno, @indc, @vcittype, @onpack, @freshfood) ";
                            DPPRD dpprd = new DPPRD(dr["flatString"].ToString().Substring(0, 13),
                                                        dr["flatString"].ToString().Substring(13, 13),
                                                        dr["flatString"].ToString().Substring(26, 13),
                                                        dr["flatString"].ToString().Substring(39, 8),
                                                        dr["flatString"].ToString().Substring(47, 2),
                                                        dr["flatString"].ToString().Substring(49, 8),
                                                        dr["flatString"].ToString().Substring(57, 1),
                                                        dr["flatString"].ToString().Substring(58, 4),
                                                        dr["flatString"].ToString().Substring(62, 8),
                                                        dr["flatString"].ToString().Substring(70, 5),
                                                        dr["flatString"].ToString().Substring(75, 1),
                                                        dr["flatString"].ToString().Substring(76, 1));

                            cn.Execute(cmd, dpprd);
                        }

                        tran.Commit();
                    }

                    using (var tran = cn.BeginTransaction())
                    {
                        foreach (DataRow dr in tb_odpprd.Rows)
                        {
                            string cmd = @"
                                INSERT INTO ODPPRD VALUES (@vcitocod, @prdtcode, @plu_no, @space, @indc1, @indc2, 
                                    @sup_no, @vcitqty, @prdtslpr, @prdtcisd, @prdtmlqy, @prdtmiqy)  ";
                            ODPPRD odpprd = new ODPPRD(dr["flatString"].ToString().Substring(0, 13),
                                                        dr["flatString"].ToString().Substring(13, 13),
                                                        dr["flatString"].ToString().Substring(26, 13),
                                                        dr["flatString"].ToString().Substring(39, 1),
                                                        dr["flatString"].ToString().Substring(40, 1),
                                                        dr["flatString"].ToString().Substring(41, 1),
                                                        dr["flatString"].ToString().Substring(42, 5),
                                                        dr["flatString"].ToString().Substring(47, 2),
                                                        dr["flatString"].ToString().Substring(49, 8),
                                                        dr["flatString"].ToString().Substring(57, 1),
                                                        dr["flatString"].ToString().Substring(58, 7),
                                                        dr["flatString"].ToString().Substring(65, 7));
                            cn.Execute(cmd, odpprd);
                        }

                        tran.Commit();
                    }

                }

                is_success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                is_success = false;
            }


            return is_success;
        }

        private void Compressed(string[] files, string zip_name)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.Password = "PxmarT";
                foreach (string file in files)
                {
                    zip.AddFile(file, "");
                }
                zip.Save(zip_name);
            }
        }

        private async Task<IActionResult> TransFile(string file_path, string file_name)
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(file_path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/octet-stream", file_name);
        }
    }
}
