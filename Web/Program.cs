using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Application.Services;
using Application.Interfaces;
using FluentValidation.AspNetCore;
using Application.Validation;
using FluentValidation;
using WebUI.Commons;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // 1. Add Context 
            builder.Services.AddDbContext<ToDoListWebContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionStringDB")));

            //// 2. Dependency Injection: Repository + Serviec
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>(); // ✅ Correct 

            //3. Register Validator 
            builder.Services.AddFluentValidationAutoValidation(); // Kích hoạt validation tự động
            builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>(); // Đăng ký validator 

            // DDnag ky user tool 
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<UserToolsCommon>();

            //Category
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            //4. Cấu Hình để luu Trữ côokie nguoi dung 
            builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
    });


            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            } 



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
              name: "default",
              pattern: "{controller=Account}/{action=LoginAccount}");


            app.Run();
        }
    }
}
