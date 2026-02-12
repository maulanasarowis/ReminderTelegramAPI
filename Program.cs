using Quartz;
using ReminderTelegramAPI.Configs;
using ReminderTelegramAPI.Jobs;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"Server local time: {DateTime.Now}");
Console.WriteLine($"Server UTC time  : {DateTime.UtcNow}");
Console.WriteLine($"TimeZone         : {TimeZoneInfo.Local.Id}");

// ✅ Kestrel binding manual
builder.WebHost.ConfigureKestrel(options =>
{
    // Listen ke IP LAN (misalnya IP server kamu)
    options.Listen(System.Net.IPAddress.Parse("192.168.12.73"), 8080);
    // Jika mau juga HTTPS (opsional)
    // options.Listen(System.Net.IPAddress.Parse("192.168.12.73"), 7000, listenOptions => listenOptions.UseHttps());
});

// register configuration & DapperContext
builder.Services.AddSingleton<DapperContext>();

// Quartz configuration
builder.Services.AddQuartz(q =>
{
    // Default sudah pakai Microsoft DI, tidak perlu panggil ini lagi
    // q.UseMicrosoftDependencyInjectionJobFactory();
    

    // Employee reminder (every 5 days at 08:00)
    var employeeJobKeyKK = new JobKey("EmployeeReminderJob_KK");
    q.AddJob<EmployeeReminderJob>(opts => opts.WithIdentity(employeeJobKeyKK)
        .UsingJobData("st_kary", "KK"));
    q.AddTrigger(opts => opts
        .ForJob(employeeJobKeyKK)
        .WithIdentity("EmployeeReminderTrigger_KK")
        .WithCronSchedule("0 15 8 */5 * ?")); // jam 8:00

    var employeeJobKeyKT = new JobKey("EmployeeReminderJob_KT");
    q.AddJob<EmployeeReminderJob>(opts => opts.WithIdentity(employeeJobKeyKT)
        .UsingJobData("st_kary", "KT"));
    q.AddTrigger(opts => opts
        .ForJob(employeeJobKeyKT)
        .WithIdentity("EmployeeReminderTrigger_KT")
        .WithCronSchedule("0 30 8 */5 * ?")); // jam 8:30

    var employeeJobKeyB = new JobKey("EmployeeReminderJob_B");
    q.AddJob<EmployeeReminderJob>(opts => opts.WithIdentity(employeeJobKeyB)
        .UsingJobData("st_kary", "B"));
    q.AddTrigger(opts => opts
        .ForJob(employeeJobKeyB)
        .WithIdentity("EmployeeReminderTrigger_B")
        .WithCronSchedule("0 0 9 */5 * ?")); // jam 9:00


    // Leader reminder (15,20,25,30 at 08:00)
    // var leaderJobKey = new JobKey("LeaderReminderJob");
    // q.AddJob<LeaderReminderJob>(opts => opts.WithIdentity(leaderJobKey));
    // q.AddTrigger(opts => opts
    //     .ForJob(leaderJobKey)
    //     .WithIdentity("LeaderReminderTrigger")
    //     .WithCronSchedule("0 0 9 15,20,25,30 * ?")
    //     // .WithCronSchedule("0 0/1 * * * ?")
    // );
});

builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

// controllers & swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reminder API v1");
            c.RoutePrefix = string.Empty; // Biar swagger tampil langsung di root
        });

}

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
