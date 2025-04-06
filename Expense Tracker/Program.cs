using Expense_Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
                "Mgo+DSMBMAY9C3t2XFhhQlJHfVpdWXxLflFzVWVTfVl6dVBWESFaRnZdR11nSXpTcEFkXX1ZdHNUTWJV"
            );

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure SQL Server connection
            var connectionString = builder.Configuration.GetConnectionString("DevConnection");
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(connectionString)
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Enable static files
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=DashBoard}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}

