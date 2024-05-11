using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PDAPI.Models;
using Ionic.Zip;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace PDAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/{storeId}")]
    public class xDownloadController : Controller
    {

        private readonly IWebHostEnvironment _environment;
        private readonly IDbConnection _connection;
        private readonly IConfiguration _configuration;
        private string DOWNLOAD_PATH = string.Empty;


        public xDownloadController(IWebHostEnvironment environment, IDbConnection connection, IConfiguration configuration)
        {
            this._environment = environment;
            this._connection = connection;
            this._configuration = configuration;
            DOWNLOAD_PATH = _configuration.GetValue<string>("Setting:DownloadPath");
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PRDT(string storeId)
        {
            if (!Directory.Exists(Path.Combine(DOWNLOAD_PATH, storeId)))
                Directory.CreateDirectory(Path.Combine(DOWNLOAD_PATH, storeId));

            //Console.WriteLine(_environment.ContentRootPath);

            string dpprd_file = "DPPRD.DB";
            string dpprd_path = Path.Combine(DOWNLOAD_PATH, storeId, dpprd_file);

            string odpprd_file = "ODPPRD.DB";
            string odpprd_path = Path.Combine(DOWNLOAD_PATH, storeId, odpprd_file);

            string zip_name = "DPPRD.ZIP";
            string zip_path = Path.Combine(DOWNLOAD_PATH, storeId, zip_name);

            var sql = "_sp_csc_1O_GenData";
            var para = new DynamicParameters();
            para.Add("@StoreID", value: storeId);
            para.Add("@retcode", dbType: DbType.Int32, direction: ParameterDirection.Output);
            para.Add("@PScript", value: false, dbType: DbType.Boolean);


            _connection.Execute(sql, para, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            Int32 retcode = para.Get<Int32>("@retcode");

            DataTable dt = new DataTable();
            sql = "_sp_csc_1O_dnPrdt";
            para = new DynamicParameters();
            para.Add("@StoreID", value: storeId);
            para.Add("@DType", value: "1", dbType: DbType.String);
            para.Add("@PScript", value: false, dbType: DbType.Boolean);

            var dr = _connection.ExecuteReader(sql, para, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            dt = new DataTable();
            dt.Load(dr);

            CreateDPPRD(dt, dpprd_path);


            dt = new DataTable();
            sql = "_sp_csc_1O_dnPrdt";
            para = new DynamicParameters();
            para.Add("@StoreID", value: storeId);
            para.Add("@DType", value: "2", dbType: DbType.String);
            para.Add("@PScript", value: false, dbType: DbType.Boolean);

            dr = _connection.ExecuteReader(sql, para, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            dt = new DataTable();
            dt.Load(dr);

            CreateODPPRD(dt, odpprd_path);

            Compressed(new string[] { dpprd_path, odpprd_path }, zip_path);

            return await TransFile(zip_path, zip_name);
        }

        public async Task<IActionResult> PRDT2(string storeId)
        {
            if (!Directory.Exists(Path.Combine(DOWNLOAD_PATH, storeId)))
                Directory.CreateDirectory(Path.Combine(DOWNLOAD_PATH, storeId));

            string dpprd_file = "DPPRD.DB";
            string dpprd_path = Path.Combine(DOWNLOAD_PATH, storeId, dpprd_file);
            string zip_name = "DPPRD.ZIP";
            string zip_path = Path.Combine(DOWNLOAD_PATH, storeId, zip_name);

            var sql = "_sp_csc_1O_GenData";
            var para = new DynamicParameters();
            para.Add("@StoreID", value: storeId);
            para.Add("@retcode", dbType: DbType.Int32, direction: ParameterDirection.Output);
            para.Add("@PScript", value: false, dbType: DbType.Boolean);

            _connection.Execute(sql, para, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            Int32 retcode = para.Get<Int32>("@retcode");


            DataTable dpprd = new DataTable();
            sql = "_sp_csc_1O_dnPrdt";
            para = new DynamicParameters();
            para.Add("@StoreID", value: storeId);
            para.Add("@DType", value: "1", dbType: DbType.String);
            para.Add("@PScript", value: false, dbType: DbType.Boolean);

            var dr_dpprd = _connection.ExecuteReader(sql, para, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            dpprd.Load(dr_dpprd);

            DataTable odpprd = new DataTable();
            sql = "_sp_csc_1O_dnPrdt";
            para = new DynamicParameters();
            para.Add("@StoreID", value: storeId);
            para.Add("@DType", value: "2", dbType: DbType.String);
            para.Add("@PScript", value: false, dbType: DbType.Boolean);

            var dr_odpprd = _connection.ExecuteReader(sql, para, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            odpprd.Load(dr_odpprd);

            CreateDPPRD2(dpprd, odpprd, dpprd_path);

            Compressed(new string[] { dpprd_path }, zip_path);

            return await TransFile(zip_path, zip_name);
        }

        public async Task<IActionResult> STOCK(string storeId)
        {
            if (!Directory.Exists(Path.Combine(DOWNLOAD_PATH, storeId)))
                Directory.CreateDirectory(Path.Combine(DOWNLOAD_PATH, storeId));

            string instant_file = "INSTANT.DB";
            string instant_path = Path.Combine(DOWNLOAD_PATH, storeId, instant_file);
            string zip_name = "INSTANT.ZIP";
            string zip_path = Path.Combine(DOWNLOAD_PATH, storeId, zip_name);

            DataTable dt_instant = new DataTable();
            var sql = "_sp_csc_1O_dnStock";
            var para = new DynamicParameters();
            para.Add("@StoreID", value: storeId);
            para.Add("@PScript", value: false, dbType: DbType.Boolean);

            var dr_instant = _connection.ExecuteReader(sql, para, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            dt_instant = new DataTable();
            dt_instant.Load(dr_instant);

            CreateINSTANT(dt_instant, instant_path);

            Compressed(new string[] { instant_path }, zip_path);

            return await TransFile(zip_path, zip_name);
        }

        public async Task<IActionResult> BOX(string storeId)
        {
            if (!Directory.Exists(Path.Combine(DOWNLOAD_PATH, storeId)))
                Directory.CreateDirectory(Path.Combine(DOWNLOAD_PATH, storeId));

            string prodbox_file = "PRODBOX.DB";
            string prodbox_path = Path.Combine(DOWNLOAD_PATH, storeId, prodbox_file);
            string zip_name = "PRODBOX.ZIP";
            string zip_path = Path.Combine(DOWNLOAD_PATH, storeId, zip_name);

            DataTable dt_prodbox = new DataTable();
            var sql = "_sp_csc_1O_dnBoxCode";
            var para = new DynamicParameters();
            para.Add("@StoreID", value: storeId);
            para.Add("@PScript", value: false, dbType: DbType.Boolean);

            var dr_prodbox = _connection.ExecuteReader(sql, para, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            dt_prodbox = new DataTable();
            dt_prodbox.Load(dr_prodbox);

            CreatePRODBOX(dt_prodbox, prodbox_path);

            Compressed(new string[] { prodbox_path }, zip_path);

            return await TransFile(zip_path, zip_name);
        }

        private bool CreateDPPRD(DataTable table, string file_path)
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
                ");


                    using (var tran = cn.BeginTransaction())
                    {
                        foreach (DataRow dr in table.Rows)
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

        private bool CreateODPPRD(DataTable table, string file_path)
        {
            bool is_success = false;

            try
            {
                if (System.IO.File.Exists(file_path)) System.IO.File.Delete(file_path);

                using (IDbConnection cn = new SQLiteConnection(@"data source=" + file_path))
                {
                    cn.Open();

                    cn.Execute(@"
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
                        foreach (DataRow dr in table.Rows)
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

        private bool CreateDPPRD2(DataTable tb_dpprd, DataTable tb_odpprd, string file_path)
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

        private bool CreateINSTANT(DataTable table, string file_path)
        {
            bool is_success = false;

            try
            {
                if (System.IO.File.Exists(file_path)) System.IO.File.Delete(file_path);

                using (IDbConnection cn = new SQLiteConnection(@"data source=" + file_path))
                {
                    cn.Open();

                    cn.Execute(@"
                        CREATE TABLE INSTANT (
                            prdtcode  char(13),
                            prdtmpqy  char(5),
                            prdtmlqy  char(7),
                            prdtmiqy  char(7)
                        );
                    ");

                    using (var tran = cn.BeginTransaction())
                    {
                        foreach (DataRow dr in table.Rows)
                        {
                            string cmd = @"
                                INSERT INTO INSTANT VALUES (@prdtcode, @prdtmpqy, @prdtmlqy, @prdtmiqy) ";
                            INSTANT instant = new INSTANT(dr["flatString"].ToString().Substring(0, 13),
                                                            dr["flatString"].ToString().Substring(13, 5),
                                                            dr["flatString"].ToString().Substring(18, 7),
                                                            dr["flatString"].ToString().Substring(25, 7));

                            cn.Execute(cmd, instant);
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

        private bool CreatePRODBOX(DataTable table, string file_path)
        {
            bool is_success = false;

            try
            {
                if (System.IO.File.Exists(file_path)) System.IO.File.Delete(file_path);

                using (IDbConnection cn = new SQLiteConnection(@"data source=" + file_path))
                {
                    cn.Open();

                    cn.Execute(@"
                         CREATE TABLE PRODBOX (
                            goo_no    char(13),
                            plu_no    char(13),
                            itf       char(14),
                            cs_qty    char(5),
                            remark1   char(1),
                            remark2   char(1)
                        );
                    ");

                    using (var tran = cn.BeginTransaction())
                    {
                        foreach (DataRow dr in table.Rows)
                        {
                            var cmd = @"
                                INSERT INTO PRODBOX VALUES (@goo_no, @plu_no, @itf, @cs_qty, @remark1, @remark2) ";
                            PRODBOX prodbox = new PRODBOX(dr["flatString"].ToString().Substring(0, 13),
                                                          dr["flatString"].ToString().Substring(13, 13),
                                                          dr["flatString"].ToString().Substring(26, 14),
                                                          dr["flatString"].ToString().Substring(40, 5),
                                                          dr["flatString"].ToString().Substring(45, 1),
                                                          dr["flatString"].ToString().Substring(46, 1));

                            cn.Execute(cmd, prodbox);
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
