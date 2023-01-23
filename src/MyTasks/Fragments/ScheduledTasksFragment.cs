using Android.Content;
using Android.Views;
using AndroidX.Core.View;
using MyTasks.Adapters;
using MyTasks.Enums;

using Task = MyTasks.Models.Task;

using static Android.App.DatePickerDialog;

namespace MyTasks.Fragments
{
	public class ScheduledTasksFragment : BaseFragment, IOnDateSetListener, IMenuProvider
	{
		internal EditText DateEditText;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);

			Activity.AddMenuProvider(this);

			return inflater.Inflate(Resource.Layout.tasks, container, false);
		}

		public override void OnDestroyView()
		{
			Activity.RemoveMenuProvider(this);
			base.OnDestroyView();
		}

		internal void OnClickDateEditText()
		{
			DateTime dateTimeNow = DateTime.Now;
			Android.App.DatePickerDialog datePicker = new DatePickerDialog(Activity, this, dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day);
			datePicker.UpdateDate(dateTimeNow);
			datePicker.Show();
		}

		public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
		{
			DateEditText.Text = new DateTime(year, month + 1, dayOfMonth).ToShortDateString();
		}

		public void OnCreateMenu(IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate(Resource.Menu.scheduled_tasks_menu, menu);
		}

		public bool OnMenuItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.action_add:
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
						builder.SetTitle("Create Scheduled Task");
						builder.SetView(view);
						builder.SetPositiveButton("Create", CreateScheduledTaskAction);
						builder.SetNegativeButton("Cancel", CancelAction);

						builder.Show();
						return true;
					}
			}

			return false;
		}

		private void CreateScheduledTaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			string name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_scheduled_task_name).Text;
			string date = DateEditText.Text;

			Task scheduledTask = new Task()
			{
				Name = name,
				TaskType = TaskTypeEnum.Scheduled,
				Date = date,
			};

			_dbHelper.CreateTask(scheduledTask);
			LoadData();
		}

		public override void LoadData()
		{
			Activity.Title = Resources.GetString(Resource.String.title_scheduled_tasks);

			List<Task> scheduledTasks = _dbHelper.GetTasksByTaskType(TaskTypeEnum.Scheduled).OrderBy(x => Convert.ToDateTime(x.Date)).ToList();
			ScheduledTaskAdapter scheduledTaskAdapter = new ScheduledTaskAdapter(this, scheduledTasks);
			ListView.Adapter = scheduledTaskAdapter;
		}
	}
}
