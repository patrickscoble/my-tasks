using Android.Content;
using Android.Views;
using MyTasks.Fragments;
using Newtonsoft.Json;

using Task = MyTasks.Models.Task;

namespace MyTasks.Adapters
{
	internal class RecurringTaskAdapter : BaseAdapter<Task>
	{
		private RecurringTasksFragment _recurringTasksFragment;

		public RecurringTaskAdapter(RecurringTasksFragment recurringTasksFragment, List<Task> recurringTasks)
			: base(recurringTasks)
		{
			this._recurringTasksFragment = recurringTasksFragment;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LayoutInflater inflater = (LayoutInflater)_recurringTasksFragment.Activity.GetSystemService(Context.LayoutInflaterService);
			View view = inflater.Inflate(Resource.Layout.recurring_task, null);

			Task recurringTask = Tasks[position];

			view.FindViewById<TextView>(Resource.Id.recurring_task_name).Text = recurringTask.Name;

			TextView lastCompletedDate = view.FindViewById<TextView>(Resource.Id.recurring_task_last_completed_date);
			lastCompletedDate.Text = string.IsNullOrEmpty(recurringTask.LastCompletedDate) ? string.Empty : $"Last Completed {recurringTask.LastCompletedDate}";

			view.Click += delegate
			{
				SubtasksFragment subtasksFragment = new SubtasksFragment();
				Bundle bundle = new Bundle();
				bundle.PutString("@string/task", JsonConvert.SerializeObject(recurringTask));
				subtasksFragment.Arguments = bundle;

				_recurringTasksFragment.Activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.container, subtasksFragment, "subtasksFragment").AddToBackStack("recurringTasksFragment").Commit();
			};

			return view;
		}
	}
}
