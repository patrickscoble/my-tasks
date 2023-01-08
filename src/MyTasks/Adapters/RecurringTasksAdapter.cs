using Android.Content;
using Android.Provider;
using Android.Views;
using MyTasks.Fragments;
using MyTasks.Models;

namespace MyTasks.Adapters
{
	internal class RecurringTaskAdapter : BaseAdapter<RecurringTask>
	{
		private RecurringTasksFragment _recurringTasksFragment;

		public RecurringTaskAdapter(RecurringTasksFragment recurringTasksFragment, List<RecurringTask> recurringTasks)
			: base(recurringTasks)
		{
			this._recurringTasksFragment = recurringTasksFragment;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LayoutInflater inflater = (LayoutInflater)_recurringTasksFragment.Activity.GetSystemService(Context.LayoutInflaterService);
			View view = inflater.Inflate(Resource.Layout.recurring_task, null);

			RecurringTask recurringTask = Tasks[position];

			view.FindViewById<TextView>(Resource.Id.recurring_task_name).Text = recurringTask.Name;

			TextView lastCompletedDate = view.FindViewById<TextView>(Resource.Id.recurring_task_last_completed_date);
			lastCompletedDate.Text = string.IsNullOrEmpty(recurringTask.LastCompletedDate) ? string.Empty : $"Last Completed {recurringTask.LastCompletedDate}";

			view.Click += delegate
			{
				LayoutInflater layoutInflater = LayoutInflater.From(_recurringTasksFragment.Activity);
				View view = layoutInflater.Inflate(Resource.Layout.create_update_recurring_task, null);

				AlertDialog.Builder builder = new AlertDialog.Builder(_recurringTasksFragment.Activity);
				builder.SetTitle("Update Recurring Task");
				builder.SetView(view);
				builder.SetPositiveButton("Update", _recurringTasksFragment.UpdateRecurringTaskAction);
				builder.SetNeutralButton("Delete", _recurringTasksFragment.DeleteRecurringTaskAction);
				builder.SetNegativeButton("Cancel", _recurringTasksFragment.CancelAction);

				// Prepopulate the fields.
				view.FindViewById<TextView>(Resource.Id.create_update_recurring_task_id).Text = recurringTask.Id.ToString();
				view.FindViewById<TextView>(Resource.Id.create_update_recurring_task_name).Text = recurringTask.Name;
				view.FindViewById<TextView>(Resource.Id.create_update_recurring_task_last_completed_date).Text = recurringTask.LastCompletedDate;

				builder.Show();
			};

			return view;
		}
	}
}
