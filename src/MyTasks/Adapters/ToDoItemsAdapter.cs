using Android.Content;
using Android.Views;
using MyTasks.Enums;
using MyTasks.Fragments;
using MyTasks.Helpers;
using MyTasks.Models;

using Task = MyTasks.Models.Task;

namespace MyTasks.Adapters
{
	internal class ToDoAdapter : BaseExpandableListAdapter
	{
		private ToDosFragment _toDosFragment;
		private Dictionary<Task, List<Subtask>> _tasks;
		private DbHelper _dbHelper;

		public override int GroupCount { get { return _tasks.Count; } }

		public override bool HasStableIds { get { return false; } }

		public ToDoAdapter(ToDosFragment toDosFragment, Dictionary<Task, List<Subtask>> tasks, DbHelper dbHelper)
		{
			this._toDosFragment = toDosFragment;
			this._tasks = tasks;
			this._dbHelper = dbHelper;
		}

		public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override long GetChildId(int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override int GetChildrenCount(int groupPosition)
		{
			return _tasks.ElementAt(groupPosition).Value.Count;
		}

		public override Java.Lang.Object GetGroup(int groupPosition)
		{
			return groupPosition;
		}

		public override bool IsChildSelectable(int groupPosition, int childPosition)
		{
			return true;
		}

		public override long GetGroupId(int groupPosition)
		{
			return groupPosition;
		}

		public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			LayoutInflater inflater = (LayoutInflater)_toDosFragment.Activity.GetSystemService(Context.LayoutInflaterService);
			View view = inflater.Inflate(Resource.Layout.to_do_task, null);
			Task task = _tasks.ElementAt(groupPosition).Key;

			view.FindViewById<TextView>(Resource.Id.to_do_name).Text = task.Name;

			// Show/hide the subtasks indicator.
			ImageView subtasks = view.FindViewById<ImageView>(Resource.Id.to_do_task_subtasks);
			subtasks.Visibility = _tasks[task].Count > 0 ? ViewStates.Visible : ViewStates.Gone;

			// Set the image for the task type.
			ImageView imageView = view.FindViewById<ImageView>(Resource.Id.to_do_task_task_type);
			int resId = GetItemTypeImage(task.TaskType);
			imageView.SetImageResource(resId);

			Button button = view.FindViewById<Button>(Resource.Id.to_do_task_done);
			button.Click += delegate
			{
				int unfinishedSubtasksCount = _tasks[task].Count(x => !x.Done);
				if (unfinishedSubtasksCount > 0)
				{
					AlertDialog.Builder builder = new AlertDialog.Builder(_toDosFragment.Activity);
					builder.SetMessage($"Task {task.Name} cannot be completed because it has {unfinishedSubtasksCount} unfinished Subtask(s).");

					builder.Show();
					return;
				}

				if (task.TaskType == TaskTypeEnum.None)
				{
					_dbHelper.DeleteTask(task.Id);
				}
				else if (task.TaskType == TaskTypeEnum.Scheduled)
				{
					_dbHelper.DeleteTask(task.Id);
				}
				else if (task.TaskType == TaskTypeEnum.Recurring)
				{
					task.LastCompletedDate = DateTime.Now.ToShortDateString();
					_dbHelper.UpdateTask(task);
				}

				_toDosFragment.LoadData();

				string text = $"{task.Name} has been completed";
				Toast.MakeText(_toDosFragment.Activity.Application, text, ToastLength.Short).Show();
			};

			view.Click += delegate
			{
				if (isExpanded)
				{
					((ExpandableListView)parent).CollapseGroup(groupPosition);
				}
				else
				{
					((ExpandableListView)parent).ExpandGroup(groupPosition);
				}
			};

			return view;
		}

		public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
			LayoutInflater inflater = (LayoutInflater)_toDosFragment.Activity.GetSystemService(Context.LayoutInflaterService);
			View view = inflater.Inflate(Resource.Layout.to_do_subtask, null);
			Task task = _tasks.ElementAt(groupPosition).Key;
			Subtask subtask = _tasks[task].ElementAt(childPosition);

			TextView name = view.FindViewById<TextView>(Resource.Id.to_do_subtask_name);
			name.Text = subtask.Name;
			name.PaintFlags = subtask.Done ? name.PaintFlags | Android.Graphics.PaintFlags.StrikeThruText : name.PaintFlags;

			// If the Subtask's parent is a Recurring Task and the Subtask was marked as complete prior to today, we need to reset the Done flag.
			if (task.TaskType == TaskTypeEnum.Recurring && subtask.Done)
			{
				if (string.IsNullOrEmpty(subtask.LastCompletedDate) || Convert.ToDateTime(subtask.LastCompletedDate) < DateTime.Now.Date)
				{
					subtask.Done = !subtask.Done;

					_dbHelper.UpdateSubtask(subtask);
				}
			}

			CheckBox checkBox = view.FindViewById<CheckBox>(Resource.Id.to_do_subtask_done);
			checkBox.Checked = subtask.Done;
			checkBox.Click += delegate
			{
				subtask.Done = !subtask.Done;

				if (subtask.Done)
				{
					subtask.LastCompletedDate = task.TaskType == TaskTypeEnum.Recurring ? DateTime.Now.ToShortDateString() : null;
				}

				_dbHelper.UpdateSubtask(subtask);

				((ExpandableListView)parent).CollapseGroup(groupPosition);
				((ExpandableListView)parent).ExpandGroup(groupPosition);
			};

			return view;
		}

		private int GetItemTypeImage(TaskTypeEnum taskType)
		{
			if (taskType == TaskTypeEnum.Scheduled) return Resource.Drawable.schedule_24;
			if (taskType == TaskTypeEnum.Recurring) return Resource.Drawable.repeat_24;
			if (taskType == TaskTypeEnum.None) return Resource.Drawable.task_24;

			return 0;
		}
	}
}
