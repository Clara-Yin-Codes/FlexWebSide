using FlexBackend.Admin.Data;
using FlexBackend.UIKit.Rcl;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FlexBackend.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services
                .AddControllersWithViews()
                // 如果先前「找不到版型」，再加這行，明確把 RCL 組件加入
                .AddApplicationPart(typeof(UiKitRclMarker).Assembly);

            //builder.Services.AddControllersWithViews();

            // 取得 RCL 主組件與 Views 組件
            //var rclAssembly = typeof(UiKitRclMarker).Assembly;
            //var rclViewsAssembly = Assembly.Load(new AssemblyName(rclAssembly.GetName().Name + ".Views"));

            //builder.Services
            //    .AddControllersWithViews()
            //    .AddApplicationPart(rclAssembly)       // 程式碼（TagHelper/Controller 等）
            //    .AddApplicationPart(rclViewsAssembly); // ⭐ 這一行很關鍵：把 .cshtml 的 compiled views 載進來

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
