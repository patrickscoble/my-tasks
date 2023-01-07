namespace MyTasks.Adapters
{
	public abstract class BaseAdapter<T> : BaseAdapter
	{
		protected List<T> Tasks;

		public override int Count { get { return Tasks.Count; } }

		public BaseAdapter(List<T> tasks)
		{
			this.Tasks = tasks;
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return position;
		}

		public override long GetItemId(int position)
		{
			return position;
		}
	}
}
