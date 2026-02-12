# ReminderTelegramAPI

Sample .NET 8 Web API using Quartz.NET and Dapper to sync reminders between SQL Server and PostgreSQL.

## How to run

1. Extract the ZIP.
2. Edit `appsettings.json` and set your connection strings.
3. Run:
   ```bash
   dotnet restore
   dotnet run
   ```

Jobs:
- EmployeeReminderJob -> Cron: `0 8 */5 * *` (08:00 every 5 days)
- LeaderReminderJob   -> Cron: `0 8 15,20,25,30 * *` (08:00 on 15,20,25,30)

