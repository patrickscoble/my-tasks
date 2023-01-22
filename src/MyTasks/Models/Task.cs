using MyTasks.Enums;

namespace MyTasks.Models
{
	public class Task
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public TaskTypeEnum TaskType { get; set; }

		public string Date { get; set; }

		public string LastCompletedDate { get; set; }
	}
}
