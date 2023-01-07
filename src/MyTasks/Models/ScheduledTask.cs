using MyTasks.Enums;

namespace MyTasks.Models
{
	public class ScheduledTask : Task
	{
		public override TaskTypeEnum TaskType { get { return TaskTypeEnum.Scheduled; } }

		public string Date { get; set; }
	}
}
