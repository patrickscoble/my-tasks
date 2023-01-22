using Android.Content;
using Android.Views;
using MyTasks.Fragments;

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
				LayoutInflater layoutInflater = LayoutInflater.From(_scheduledTasksFragment.Activity);
				View view = layoutInflater.Inflate(Resource.Layout.create_update_scheduled_task, null);

				_scheduledTasksFragment.DateEditText = view.FindViewById<EditText>(Resource.Id.create_update_scheduled_task_date);
				_scheduledTasksFragment.DateEditText.Text = DateTime.Now.ToShortDateString();
				_scheduledTasksFragment.DateEditText.Click += delegate
				{
					_scheduledTasksFragment.OnClickDateEditText();
				};

				AlertDialog.Builder builder = new AlertDialog.Builder(_scheduledTasksFragment.Activity);
				builder.SetTitle("Update Scheduled Task");
				builder.SetView(view);
				builder.SetPositiveButton("Update", _scheduledTasksFragment.UpdateScheduledTaskAction);
				builder.SetNeutralButton("Delete", _scheduledTasksFragment.DeleteScheduledTaskAction);
				builder.SetNegativeButton("Cancel", _scheduledTasksFragment.CancelAction);

				// Prepopulate the fields.
				view.FindViewById<TextView>(Resource.Id.create_update_scheduled_task_id).Text = scheduledTask.Id.ToString();
				view.FindViewById<TextView>(Resource.Id.create_update_scheduled_task_name).Text = scheduledTask.Name;
				view.FindViewById<TextView>(Resource.Id.create_update_scheduled_task_date).Text = scheduledTask.Date.ToString();

				builder.Show();
			};

			return view;
		}
	}
}
