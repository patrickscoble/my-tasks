using Android.Content;
using Android.Views;
using MyTasks.Fragments;
using Newtonsoft.Json;
using Task = MyTasks.Models.Task;

namespace MyTasks.Adapters
{
	internal class TaskAdapter : BaseAdapter<Task>
	{
		private TasksFragment _tasksFragment;

		public TaskAdapter(TasksFragment tasksFragment, List<Task> tasks)
			: base(tasks)
		{
			this._tasksFragment = tasksFragment;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LayoutInflater inflater = (LayoutInflater)_tasksFragment.Activity.GetSystemService(Context.LayoutInflaterService);
			View view = inflater.Inflate(Resource.Layout.task, null);

			Task task = Tasks[position];

			view.FindViewById<TextView>(Resource.Id.task_name).Text = task.Name;

			view.Click += delegate
			{
				SubtasksFragment subtasksFragment = new SubtasksFragment();
				Bundle bundle = new Bundle();
				bundle.PutString("@string/task", JsonConvert.SerializeObject(task));
				subtasksFragment.Arguments = bundle;

				_tasksFragment.Activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.container, subtasksFragment, "subtasksFragment").AddToBackStack("tasksFragment").Commit();
			};

			return view;
		}
	}
}
