using Android.Content;
using Android.Views;
using AndroidX.Core.View;
using MyTasks.Adapters;
using MyTasks.Models;

using static Android.App.DatePickerDialog;

namespace MyTasks.Fragments
{
	public class ScheduledTasksFragment : BaseFragment, IOnDateSetListener, IMenuProvider
	{
		internal EditText DateEditText;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Activity.Title = Resources.GetString(Resource.String.title_scheduled_tasks);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);

			Activity.AddMenuProvider(this);

			return inflater.Inflate(Resource.Layout.tasks, container, false);
		}

		public override void OnDestroyView()
		{
			Activity.RemoveMenuProvider(this);
			base.OnDestroy();
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

			ScheduledTask scheduledTask = new ScheduledTask()
			{
				Name = name,
				Date = date,
			};

			_dbHelper.CreateScheduledTask(scheduledTask);
			LoadData();
		}

		public void UpdateScheduledTaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			string id = alertDialog.FindViewById<TextView>(Resource.Id.create_update_scheduled_task_id).Text;
			string name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_scheduled_task_name).Text;
			string date = DateEditText.Text;

			ScheduledTask scheduledTask = new ScheduledTask()
			{
				Id = Convert.ToInt32(id),
				Name = name,
				Date = date,
			};

			_dbHelper.UpdateScheduledTask(scheduledTask);
			LoadData();
		}

		public void DeleteScheduledTaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			string id = alertDialog.FindViewById<TextView>(Resource.Id.create_update_scheduled_task_id).Text;
			string name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_scheduled_task_name).Text;

			_dbHelper.DeleteScheduledTask(Convert.ToInt32(id));
			LoadData();

			string text = $"{name} has been deleted";
			Toast.MakeText(Activity.Application, text, ToastLength.Short).Show();
		}

		public override void LoadData()
		{
			List<ScheduledTask> scheduledTasks = _dbHelper.GetAllScheduledTasks();
			ScheduledTaskAdapter scheduledTaskAdapter = new ScheduledTaskAdapter(this, scheduledTasks);
			ListView.Adapter = scheduledTaskAdapter;
		}
	}
}
