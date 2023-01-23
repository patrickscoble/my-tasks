namespace MyTasks.Models
{
	public class Subtask
	{
		public int Id { get; set; }

		public int TaskId { get; set; }

		public string Name { get; set; }

		public bool Done { get; set; }

		public string LastCompletedDate { get; set; }
	}
}
