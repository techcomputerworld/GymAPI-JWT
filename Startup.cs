using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Authorization;
using WebApi.Data.Entities;
using WebApi.Helpers;
using WebApi.Services;
using WebApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // add services to the DI container
        public void ConfigureServices(IServiceCollection services)
        {
            string mySqlConnection = Configuration.GetConnectionString("DefaultConnection");

            //services.AddDbContext<DataContext>();
            services.AddDbContext<WebApiDbContext>(options => options.UseMySql(mySqlConnection, 
                ServerVersion.AutoDetect(mySqlConnection)));
            services.AddCors();
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // configure DI for application services
            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<ITrainingService, TrainingService>();
        }

        // configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, WebApiDbContext context)
        {
            createUserRoot(context);

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            // global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(x => x.MapControllers());
        }
        /// <summary>
        /// CreateUserRoot para testeo de la aplicación
        /// </summary>
        /// <param name="context"></param>
        private void createUserRoot(WebApiDbContext context)
        {
            // add hardcoded test user to db on startup
            //Contraseña test de momento que no estoy en producción.
            bool existAdmin = false;

            var adminUser = new User
            {
                FirstName = "Pepe",
                LastName = "Lopez",
                Username = "Test",
                Country = "Spain",
                City = "Madrid",
                Data = "Usuario",
                Email = "tucorreo@gmail.com",
                CreationDateTime = DateTime.Now,
                LastUpdateDateTime = DateTime.Now,
                PasswordHash = BCryptNet.HashPassword("test")
            };

            var entidad = context.Users.ToListAsync();
            List<User> users = entidad.Result;
            foreach (User user in users)
            {
                if (user.Email == "onzulin@gmail.com")
                {
                    existAdmin = true;
                }
            }
            if (existAdmin == false)
            {
                context.Users.Add(adminUser);
                context.SaveChanges();
            }

            
           
           
            





        }
    }
}
