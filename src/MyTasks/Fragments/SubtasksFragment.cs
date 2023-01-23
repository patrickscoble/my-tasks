using Android.Content;
using Android.Views;
using AndroidX.Core.View;
using MyTasks.Adapters;
using MyTasks.Enums;
using MyTasks.Models;
using Newtonsoft.Json;

using static Android.App.DatePickerDialog;
using Task = MyTasks.Models.Task;

namespace MyTasks.Fragments
{
	public class SubtasksFragment : BaseFragment, IOnDateSetListener, IMenuProvider
	{
		private Task _task;	
		private EditText DateEditText;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);

			if (Arguments != null)
			{
				_task = JsonConvert.DeserializeObject<Task>(Arguments.GetString("@string/task"));
			}

			Activity.AddMenuProvider(this);

			return inflater.Inflate(Resource.Layout.subtasks, container, false);
		}

		public override void OnDestroyView()
		{
			Activity.RemoveMenuProvider(this);
			base.OnDestroyView();
		}

		internal void OnClickDateEditText()
		{
			DateTime dateTimeNow = DateTime.Now;
			DatePickerDialog datePicker = new DatePickerDialog(Activity, this, dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day);
			datePicker.UpdateDate(dateTimeNow);
			datePicker.Show();
		}

		public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
		{
			DateEditText.Text = new DateTime(year, month + 1, dayOfMonth).ToShortDateString();
		}

		public void OnCreateMenu(IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate(Resource.Menu.subtasks_menu, menu);
		}

		public bool OnMenuItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.action_edit:
					{
						if (_task.TaskType == TaskTypeEnum.None)
						{
							LayoutInflater layoutInflater = LayoutInflater.From(Activity);
							View view = layoutInflater.Inflate(Resource.Layout.create_update_task, null);

							AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
							builder.SetTitle("Update Task");
							builder.SetView(view);
							builder.SetPositiveButton("Update", UpdateTaskAction);
							builder.SetNeutralButton("Delete", DeleteTaskAction);
							builder.SetNegativeButton("Cancel", CancelAction);

							// Prepopulate the fields.
							view.FindViewById<TextView>(Resource.Id.create_update_task_id).Text = _task.Id.ToString();
							view.FindViewById<TextView>(Resource.Id.create_update_task_name).Text = _task.Name;

							builder.Show();
							return true;
						}
						else if (_task.TaskType == TaskTypeEnum.Scheduled)
						{
							LayoutInflater layoutInflater = LayoutInflater.From(Activity);
							View view = layoutInflater.Inflate(Resource.Layout.create_update_scheduled_task, null);

							DateEditText = view.FindViewById<EditText>(Resource.Id.create_update_scheduled_task_date);
							DateEditText.Text = DateTime.Now.ToShortDateString();
							DateEditText.Click += delegate
							{
								OnClickDateEditText();
							};

							AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
							builder.SetTitle("Update Scheduled Task");
							builder.SetView(view);
							builder.SetPositiveButton("Update", UpdateScheduledTaskAction);
							builder.SetNeutralButton("Delete", DeleteScheduledTaskAction);
							builder.SetNegativeButton("Cancel", CancelAction);

							// Prepopulate the fields.
							view.FindViewById<TextView>(Resource.Id.create_update_scheduled_task_id).Text = _task.Id.ToString();
							view.FindViewById<TextView>(Resource.Id.create_update_scheduled_task_name).Text = _task.Name;
							view.FindViewById<TextView>(Resource.Id.create_update_scheduled_task_date).Text = _task.Date.ToString();

							builder.Show();
						}
						else if (_task.TaskType == TaskTypeEnum.Recurring)
						{
							LayoutInflater layoutInflater = LayoutInflater.From(Activity);
							View view = layoutInflater.Inflate(Resource.Layout.create_update_recurring_task, null);

							AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
							builder.SetTitle("Update Recurring Task");
							builder.SetView(view);
							builder.SetPositiveButton("Update", UpdateRecurringTaskAction);
							builder.SetNeutralButton("Delete", DeleteRecurringTaskAction);
							builder.SetNegativeButton("Cancel", CancelAction);

							// Prepopulate the fields.
							view.FindViewById<TextView>(Resource.Id.create_update_recurring_task_id).Text = _task.Id.ToString();
							view.FindViewById<TextView>(Resource.Id.create_update_recurring_task_name).Text = _task.Name;
							view.FindViewById<TextView>(Resource.Id.create_update_recurring_task_last_completed_date).Text = _task.LastCompletedDate;

							builder.Show();
						}

						return false;
					}
				case Resource.Id.action_add:
					{
						LayoutInflater layoutInflater = LayoutInflater.From(Activity);
						View view = layoutInflater.Inflate(Resource.Layout.create_update_subtask, null);

						AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
						builder.SetTitle("Create Subtask");
						builder.SetView(view);
						builder.SetPositiveButton("Create", CreateSubtaskAction);
						builder.SetNegativeButton("Cancel", CancelAction);

						builder.Show();
						return true;
					}
			}

			return false;
		}

		private void UpdateTaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			_task.Name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_task_name).Text;

			_dbHelper.UpdateTask(_task);
			LoadData();
		}

		private void DeleteTaskAction(object sender, DialogClickEventArgs e)
		{
			_dbHelper.DeleteTask(_task.Id);
			LoadData();

			string text = $"{_task.Name} has been deleted";
			Toast.MakeText(Activity.Application, text, ToastLength.Short).Show();

			Activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.container, new TasksFragment(), "tasksFragment").Commit();
		}

		private void UpdateScheduledTaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			_task.Name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_scheduled_task_name).Text;
			_task.Date = DateEditText.Text;

			_dbHelper.UpdateTask(_task);
			LoadData();
		}

		private void DeleteScheduledTaskAction(object sender, DialogClickEventArgs e)
		{
			_dbHelper.DeleteTask(_task.Id);
			LoadData();

			string text = $"{_task.Name} has been deleted";
			Toast.MakeText(Activity.Application, text, ToastLength.Short).Show();

			Activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.container, new ScheduledTasksFragment(), "scheduledTasksFragment").Commit();
		}

		private void UpdateRecurringTaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			_task.Name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_recurring_task_name).Text;

			_dbHelper.UpdateTask(_task);
			LoadData();
		}

		private void DeleteRecurringTaskAction(object sender, DialogClickEventArgs e)
		{
			_dbHelper.DeleteTask(_task.Id);
			LoadData();

			string text = $"{_task.Name} has been deleted";
			Toast.MakeText(Activity.Application, text, ToastLength.Short).Show();

			Activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.container, new RecurringTasksFragment(), "recurringTasksFragment").Commit();
		}

		private void CreateSubtaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			string name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_subtask_name).Text;

			Subtask subtask = new Subtask()
			{
				TaskId = _task.Id,
				Name = name,
				Done = false,
			};

			_dbHelper.CreateSubtask(subtask);
			LoadData();
		}

		public void UpdateSubtaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			string id = alertDialog.FindViewById<TextView>(Resource.Id.create_update_subtask_id).Text;
			string name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_subtask_name).Text;
			bool done = alertDialog.FindViewById<CheckBox>(Resource.Id.create_update_subtask_done).Checked;

			Subtask subtask = new Subtask()
			{
				Id = Convert.ToInt32(id),
				Name = name,
				Done = done,
			};

			_dbHelper.UpdateSubtask(subtask);
			LoadData();
		}

		public void DeleteSubtaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			string id = alertDialog.FindViewById<TextView>(Resource.Id.create_update_subtask_id).Text;
			string name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_subtask_name).Text;

			_dbHelper.DeleteSubtask(Convert.ToInt32(id));
			LoadData();

			string text = $"{name} has been deleted";
			Toast.MakeText(Activity.Application, text, ToastLength.Short).Show();
		}

		public override void LoadData()
		{
			Activity.Title = _task.Name;

			List<Subtask> subtasks = _dbHelper.GetSubtasksByTaskId(_task.Id).ToList();
			SubtaskAdapter taskAdapter = new SubtaskAdapter(this, subtasks);
			ListView.Adapter = taskAdapter;
		}
	}
}
