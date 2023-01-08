using MyTasks.Enums;

namespace MyTasks.Models
{
	public class RecurringTask : Task
	{
		public override TaskTypeEnum TaskType { get { return TaskTypeEnum.Recurring; } }

		public string LastCompletedDate { get; set; }
	}
}
