using Android.Content;
using Android.Views;
using MyTasks.Fragments;
using Newtonsoft.Json;

using Task = MyTasks.Models.Task;

namespace MyTasks.Adapters
{
	internal class ScheduledTaskAdapter : BaseAdapter<Task>
	{
		private ScheduledTasksFragment _scheduledTasksFragment;

		public ScheduledTaskAdapter(ScheduledTasksFragment scheduledTasksFragment, List<Task> scheduledTasks)
			: base(scheduledTasks)
		{
			this._scheduledTasksFragment = scheduledTasksFragment;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LayoutInflater inflater = (LayoutInflater)_scheduledTasksFragment.Activity.GetSystemService(Context.LayoutInflaterService);
			View view = inflater.Inflate(Resource.Layout.scheduled_task, null);

			Task scheduledTask = Tasks[position];

			view.FindViewById<TextView>(Resource.Id.scheduled_task_name).Text = scheduledTask.Name;
			view.FindViewById<TextView>(Resource.Id.scheduled_task_date).Text = scheduledTask.Date;

			view.Click += delegate
			{
				SubtasksFragment subtasksfragment = new SubtasksFragment();
				Bundle bundle = new Bundle();
				bundle.PutString("@string/task", JsonConvert.SerializeObject(scheduledTask));
				subtasksfragment.Arguments = bundle;

				_scheduledTasksFragment.Activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.container, subtasksfragment, "subtasksFragment").AddToBackStack("scheduledTasksFragment").Commit();
			};

			return view;
		}
	}
}
