using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace ReminderTelegramAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuartzController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public QuartzController(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        // ✅ Endpoint manual trigger job
        [HttpPost("run-now")]
        public async Task<IActionResult> RunNow()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.TriggerJob(new JobKey("EmployeeReminderJob"));
            return Ok("✅ Job EmployeeReminderJob dijalankan manual!");
        }

        //
        [HttpPost("run-now-leader")]
        public async Task<IActionResult> RunNowLeader()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.TriggerJob(new JobKey("LeaderReminderJob"));
            return Ok("✅ Job LeaderReminderJob dijalankan manual!");
        }
    }
}
