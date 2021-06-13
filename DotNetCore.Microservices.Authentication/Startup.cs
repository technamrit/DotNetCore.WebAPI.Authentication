using DotNetCore.Microservices.Authentication.Models;
using DotNetCore.Microservices.Authentication.DataAccessLayer;
using DotNetCore.Microservices.Authentication.Repository;
using DotNetCore.Microservices.Authentication.Utilities;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog.AspNetCore;
using Serilog.Sinks.File;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace DotNetCore.Microservices.Authentication
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

            //Add database for Entity Framework
            AddDatabases(services);

            //Add Identity
            AddIdentity(services);

            //Add Options for IOptions pattern
            AddOptions(services);

            //Add Authentication
            AddAuthentication(services);

            //Add Loggers
            AddLoggers(services);

            //Add data repository classes
            AddRepositories(services);

            //Add other implementations
            AddImplementations(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddRepositories(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
        }

        private void AddDatabases(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnectionString")));
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => 
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:IssuerSigningKey"])),
                };
            });
        }

        private void AddIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDBContext>()
                    .AddDefaultTokenProviders();
        }

        private void AddOptions(IServiceCollection services)
        {
            services.AddOptions<ApplicationConfiguration>()
                    .Configure<IConfiguration>((applicationConfiguration, configuration) =>
                    {
                        configuration.Bind(applicationConfiguration);
                    });
        }

        private void AddLoggers(IServiceCollection services)
        {
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .WriteTo.Debug(outputTemplate: DateTime.Now.ToString())
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Seq("http://localhost:5341/")
                .WriteTo.MSSqlServer(
                         connectionString: Configuration.GetConnectionString("DatabaseConnectionString"),
                         sinkOptions: new MSSqlServerSinkOptions
                         {
                             TableName = "DotNetCoreWebAPILogs",
                             SchemaName = "dbo",
                             AutoCreateSqlTable = true
                         },
                         appConfiguration: Configuration,
                         columnOptionsSection: Configuration.GetSection("Serilog:ColumnOptions"),
                         columnOptions: GetColumnOptions()).CreateLogger();
            services.AddLogging(log => log.AddSerilog(logger));
        }

        private void AddImplementations(IServiceCollection services)
        {
            services.AddSingleton<ITokenHelper, TokenHelper>();
        }

        private LoggerConfiguration GetLoggerConfiguration()
        {
            return new LoggerConfiguration().Enrich.FromLogContext();
        }

        private static ColumnOptions GetColumnOptions()
        {
            var columnOptions = new ColumnOptions
            {
                Store = new Collection<StandardColumn>(),
                TimeStamp =
                {
                    ConvertToUtc = true,
                    ColumnName = "Created"
                },
                AdditionalColumns = new Collection<SqlColumn>
                {
                    new SqlColumn
                    {
                        DataType = SqlDbType.DateTimeOffset,
                        ColumnName = "CreatedAt",
                        AllowNull = false
                    },
                }
            };
            return columnOptions;
        }

    }
}
