using Dapper;
using Quartz;
using ReminderTelegramAPI.Configs;

namespace ReminderTelegramAPI.Jobs;

public class LeaderReminderJob : IJob
{
    // private readonly DapperContext _context;
    private readonly ILogger<LeaderReminderJob> _logger;

    public LeaderReminderJob(ILogger<LeaderReminderJob> logger)
    {
        // _context = context;
        _logger = logger;
    }

    // public async Task Execute(IJobExecutionContext context)
    // {
    //     _logger.LogInformation("LeaderReminderJob started at {Time}", DateTimeOffset.Now);

    //     using var sql = _context.CreateSqlConnection();
    //     using var pg = _context.CreatePostgresConnection();

    //     // ambil dept yang memiliki karyawan belum mcu (contoh)
    //     var depts = await pg.QueryAsync<string>(@"
    //         SELECT DISTINCT department
    //         FROM mcu_data
    //         WHERE status != 'selesai'");

    //     foreach (var dept in depts)
    //     {
    //         await sql.ExecuteAsync(@"
    //             INSERT INTO mcu_reminder_log (NIK, Nama, Department, ReminderDate, ReminderType)
    //             VALUES ('-', @Nama, @Dept, GETDATE(), 'Pimpinan')",
    //             new { Nama = $"Dept/Bagian {dept}", Dept = dept });
    //     }

    //     _logger.LogInformation("LeaderReminderJob finished at {Time}", DateTimeOffset.Now);
    // }
    public async Task Execute(IJobExecutionContext context)
    {
        var now = DateTime.Now;
        Console.WriteLine($"🟢 LeaderReminderJob RUNNING at {now}");
        _logger.LogInformation("LeaderReminderJob started at {Time}", DateTimeOffset.Now);

        // ... isi job kamu di sini

        Console.WriteLine($"✅ LeaderReminderJob FINISHED at {DateTime.Now}");
        _logger.LogInformation("LeaderReminderJob finished at {Time}", DateTimeOffset.Now);
    }

}
