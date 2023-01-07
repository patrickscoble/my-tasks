using Android.Content;
using Android.Views;
using MyTasks.Fragments;
using MyTasks.Helpers;

using Task = MyTasks.Models.Task;

namespace MyTasks.Adapters
{
	internal class TaskAdapter : BaseAdapter
	{
		private TasksFragment _tasksFragment;
		private DbHelper _dbHelper;
		private List<Task> _tasks;

		public override int Count { get { return _tasks.Count; } }

		public TaskAdapter(TasksFragment tasksFragment, List<Task> tasks, DbHelper dbHelper)
		{
			this._tasksFragment = tasksFragment;
			this._dbHelper = dbHelper;
			this._tasks = tasks;
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return position;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LayoutInflater inflater = (LayoutInflater)_tasksFragment.Activity.GetSystemService(Context.LayoutInflaterService);
			View view = inflater.Inflate(Resource.Layout.task, null);

			Task task = _tasks[position];

			view.FindViewById<TextView>(Resource.Id.task_name).Text = task.Name;

			CheckBox checkBox = view.FindViewById<CheckBox>(Resource.Id.task_done);
			checkBox.Click += delegate
			{
				_dbHelper.DeleteTask(task.Id);
				_tasksFragment.LoadData();
			};

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

			return view;
		}
	}
}
