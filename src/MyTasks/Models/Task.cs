using MyTasks.Enums;

namespace MyTasks.Models
{
	public class Task
	{
		public virtual TaskTypeEnum TaskType { get { return TaskTypeEnum.None; } }

		public int Id { get; set; }

		public string Name { get; set; }
	}
}
