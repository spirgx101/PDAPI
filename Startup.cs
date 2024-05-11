using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PDAPI.DataRepositories;
using PDAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PDAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var connectionDict = new Dictionary<DbConnectionName, string>
            {
                { DbConnectionName.CSC, this.Configuration.GetConnectionString("CSC") },
                { DbConnectionName.EEPDC, this.Configuration.GetConnectionString("EEPDC")},
                { DbConnectionName.SMD, this.Configuration.GetConnectionString("SMD")},
                { DbConnectionName.POP, this.Configuration.GetConnectionString("POP")}
            };

            services.AddSingleton<IDictionary<DbConnectionName, string>>(connectionDict);

            services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();
            

            services.AddScoped<IDbConnection, SqlConnection>(serviceProvider => {
                SqlConnection conn = new SqlConnection();
                //指派連線字串
                conn.ConnectionString = Configuration.GetConnectionString("CSC");
                return conn;
            });

            
            services.AddDirectoryBrowser();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //引用自定义文件路径
            var path = Configuration.GetValue<string>("Setting:DownloadPath");

            if (!Directory.Exists(path))  Directory.CreateDirectory(path);
            //var path = @"D:\Temp\wwwroot\";
            var staticFile = new StaticFileOptions();
            staticFile.FileProvider = new PhysicalFileProvider(path);
            app.UseStaticFiles(staticFile);
               

            //显示静态文件路径下的所有文件
            var staticBrowser = new DirectoryBrowserOptions();
            staticBrowser.FileProvider = new PhysicalFileProvider(path);
            app.UseDirectoryBrowser(staticBrowser);

            //app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}


/*
 　　　　　　public void ConfigureServices(IServiceCollection services)
　　　　　　{
　　　　　　　　services.AddDirectoryBrowser();
　　　　　　　　services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
　　　　　　}



　　　　　public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //引用自定义文件路径
            var path = @"C:\Users\LIKUI\Desktop\试点项目\";
            var staticFile = new StaticFileOptions();
            staticFile.FileProvider = new PhysicalFileProvider(path);
            app.UseStaticFiles(staticFile);

            //显示静态文件路径下的所有文件
            var staticBrowser = new DirectoryBrowserOptions();
            staticBrowser.FileProvider = new PhysicalFileProvider(path);
            app.UseDirectoryBrowser(staticBrowser);

            app.UseHttpsRedirection();
            app.UseMvc();
        }
 
 
 
*/