using System.Text;
using Dapper;
using Newtonsoft.Json;
using Quartz;
using ReminderTelegramAPI.Configs;
using ReminderTelegramAPI.Models;

namespace ReminderTelegramAPI.Jobs;

public class EmployeeReminderJob : IJob
{
    private readonly DapperContext _context;
    private readonly ILogger<EmployeeReminderJob> _logger;

    public EmployeeReminderJob(DapperContext context, ILogger<EmployeeReminderJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var stKary = context.MergedJobDataMap.GetString("st_kary") ?? "KK"; // default KK
        var now = DateTime.Now;
        Console.WriteLine($"🟢 EmployeeReminderJob ({stKary}) RUNNING at {now}");
        _logger.LogInformation("EmployeeReminderJob ({StKary}) started at {Time}", stKary, DateTimeOffset.Now);

        try
        {
            using var httpClient = new HttpClient();
            var apiUrl = "http://192.168.12.168:5070/apiklinik/MCU/get_tk_filter_for_mcu";

            var requestBody = new
            {
                bulan_mcu = DateTime.Now.Month,
                st_kary = stKary,
                sort_by = "nama",
                ascending = true
            };

            var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(apiUrl, content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        dynamic? result = JsonConvert.DeserializeObject(responseContent);
        var listTK = result?.listTKTerdaftar ?? new object[] { };

        using var sql = _context.CreateSqlConnection();

        var insertQuery = @"
            INSERT INTO TelegramOutbox
            (DataFrom, ToTelegramID, FirstName, Messages, SendingTime, PendingStatus)
            VALUES (@notif_from, @telegram_id, @name, @message, CURRENT_TIMESTAMP, 1);
        ";

        var message = """
<b>Pengingat Pelaksanaan MCU Berkala</b>

Bapak/Ibu,
Telah memasuki periode pelaksanaan <b>Medical Check-Up (MCU)</b> tahunan.
Mohon untuk segera melakukan pendaftaran MCU melalui <b>admin Departemen/Bagian</b> masing-masing.

Terima kasih atas perhatian dan kerja sama Bapak/Ibu.

<i>Catatan: Pesan ini akan dikirim setiap 5 (lima) hari sekali hingga pelaksanaan MCU dilakukan.</i>
""";

        int insertedCount = 0;
        foreach (var tk in listTK)
        {
            long? telegramId = tk.telegramid;
            string nama = tk.nama;
            if (telegramId == null || telegramId == 0)
                continue;

            var param = new
            {
                notif_from = "EKLINIK",
                telegram_id = telegramId,
                name = nama,
                message
            };

            insertedCount += await sql.ExecuteAsync(insertQuery, param);
        }

        Console.WriteLine($"✅ [{stKary}] Insert sukses: {insertedCount} row(s)");
        _logger.LogInformation("[{StKary}] Insert success: {Count} rows at {Time}", stKary, insertedCount, DateTimeOffset.Now);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ [{stKary}] Error: {ex.Message}");
        _logger.LogError(ex, "[{StKary}] Error saat insert ke TelegramOutbox", stKary);
    }

    Console.WriteLine($"✅ EmployeeReminderJob ({stKary}) FINISHED at {DateTime.Now}");
    _logger.LogInformation("EmployeeReminderJob ({StKary}) finished at {Time}", stKary, DateTimeOffset.Now);
}


    // public async Task Execute(IJobExecutionContext context)
    // {
    //     var now = DateTime.Now;
    //     Console.WriteLine($"🟢 EmployeeReminderJob RUNNING at {now}");
    //     _logger.LogInformation("EmployeeReminderJob started at {Time}", DateTimeOffset.Now);

    //     try
    //     {
    //         using var sql = _context.CreateSqlConnection();

    //         var insertQuery = @"
    //             INSERT INTO TelegramOutbox
    //             (DataFrom, ToTelegramID, FirstName, Messages, SendingTime, PendingStatus)
    //             VALUES (@notif_from, @telegram_id, @name, @message, CURRENT_TIMESTAMP, 1);
    //         ";

    //         var message = """
    // <b>Pengingat Pelaksanaan MCU Berkala</b>

    // Bapak/Ibu telah memasuki periode jatuh tempo Medical Check-Up (MCU) tahunan.  
    // Mohon untuk segera melakukan pendaftaran MCU melalui admin Departemen/Bagian masing-masing.

    // Terima kasih atas perhatian dan kerja sama Bapak/Ibu.

    // <i>Catatan: Pesan ini akan dikirim setiap 5 (lima) hari sekali hingga pelaksanaan MCU dilakukan.</i>
    // """;

    //         var param = new
    //         {
    //             notif_from = "EKLINIK",
    //             telegram_id = "883654574", // contoh ID Telegram
    //             name = "MAULANA SAROWIS",
    //             message
    //         };

    //         var rows = await sql.ExecuteAsync(insertQuery, param);

    //         Console.WriteLine($"✅ Insert sukses: {rows} row(s) ditambahkan ke TelegramOutbox");
    //         _logger.LogInformation("Insert success at {Time}", DateTimeOffset.Now);
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"❌ Gagal insert ke TelegramOutbox: {ex.Message}");
    //         _logger.LogError(ex, "Error saat insert ke TelegramOutbox");
    //     }

    //     Console.WriteLine($"✅ EmployeeReminderJob FINISHED at {DateTime.Now}");
    //     _logger.LogInformation("EmployeeReminderJob finished at {Time}", DateTimeOffset.Now);
    // }

}
