using Android.Content;
using Android.Views;
using MyTasks.Enums;
using MyTasks.Fragments;
using MyTasks.Helpers;
using MyTasks.Models;
using Task = MyTasks.Models.Task;

namespace MyTasks.Adapters
{
	internal class TaskAdapter : BaseAdapter<Task>
	{
		private TasksFragment _tasksFragment;
		private DbHelper _dbHelper;

		public TaskAdapter(TasksFragment tasksFragment, List<Task> tasks, DbHelper dbHelper)
			: base(tasks)
		{
			this._tasksFragment = tasksFragment;
			this._dbHelper = dbHelper;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LayoutInflater inflater = (LayoutInflater)_tasksFragment.Activity.GetSystemService(Context.LayoutInflaterService);
			View view = inflater.Inflate(Resource.Layout.task, null);

			Task task = Tasks[position];

			view.FindViewById<TextView>(Resource.Id.task_name).Text = task.Name;

			// Set the image for the task type.
			ImageView imageView = view.FindViewById<ImageView>(Resource.Id.task_task_type);
			int resId = GetItemTypeImage(task.TaskType);
			imageView.SetImageResource(resId);

			Button button = view.FindViewById<Button>(Resource.Id.task_done);
			button.Click += delegate
			{
				if (task.TaskType == TaskTypeEnum.None)
				{
					_dbHelper.DeleteTask(task.Id);
				}
				else if (task.TaskType == TaskTypeEnum.Scheduled)
				{
					_dbHelper.DeleteScheduledTask(task.Id);
				}
				else if (task.TaskType == TaskTypeEnum.Recurring)
				{
					RecurringTask recurringtask = (RecurringTask)task;
					recurringtask.LastCompletedDate = DateTime.Now.ToShortDateString();
					_dbHelper.UpdateRecurringTask(recurringtask);
				}

				_tasksFragment.LoadData();

				string text = $"{task.Name} has been completed";
				Toast.MakeText(_tasksFragment.Activity.Application, text, ToastLength.Short).Show();
			};

			// Only Tasks should be editable on this screen.
			if (task.TaskType == TaskTypeEnum.None)
			{
				view.Click += delegate
				{
					LayoutInflater layoutInflater = LayoutInflater.From(_tasksFragment.Activity);
					View view = layoutInflater.Inflate(Resource.Layout.create_update_task, null);

					AlertDialog.Builder builder = new AlertDialog.Builder(_tasksFragment.Activity);
					builder.SetTitle("Update Task");
					builder.SetView(view);
					builder.SetPositiveButton("Update", _tasksFragment.UpdateTaskAction);
					builder.SetNegativeButton("Cancel", _tasksFragment.CancelAction);

					// Prepopulate the fields.
					view.FindViewById<TextView>(Resource.Id.create_update_task_id).Text = task.Id.ToString();
					view.FindViewById<TextView>(Resource.Id.create_update_task_name).Text = task.Name;

					builder.Show();
				};
			}

			return view;
		}

		private int GetItemTypeImage(TaskTypeEnum taskType)
		{
			if (taskType == TaskTypeEnum.Scheduled) return Resource.Drawable.schedule_24;
			if (taskType == TaskTypeEnum.Recurring) return Resource.Drawable.repeat_24;

			return 0;
		}
	}
}
