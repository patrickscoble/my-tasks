using Android.Content;
using Android.Views;
using AndroidX.Core.View;
using MyTasks.Adapters;
using MyTasks.Enums;

using Task = MyTasks.Models.Task;

namespace MyTasks.Fragments
{
	public class RecurringTasksFragment : BaseFragment, IMenuProvider
	{
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Activity.Title = Resources.GetString(Resource.String.title_recurring_tasks);
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

		public void OnCreateMenu(IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate(Resource.Menu.recurring_tasks_menu, menu);
		}

		public bool OnMenuItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.action_add:
					{
						LayoutInflater layoutInflater = LayoutInflater.From(Activity);
						View view = layoutInflater.Inflate(Resource.Layout.create_update_recurring_task, null);

						AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
						builder.SetTitle("Create Recurring Task");
						builder.SetView(view);
						builder.SetPositiveButton("Create", CreateRecurringTaskAction);
						builder.SetNegativeButton("Cancel", CancelAction);

						builder.Show();
						return true;
					}
			}

			return false;
		}

		private void CreateRecurringTaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			string name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_recurring_task_name).Text;

			Task recurringTask = new Task()
			{
				Name = name,
				TaskType = TaskTypeEnum.Recurring,
				LastCompletedDate = string.Empty,
			};

			_dbHelper.CreateTask(recurringTask);
			LoadData();
		}

		public void UpdateRecurringTaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			string id = alertDialog.FindViewById<TextView>(Resource.Id.create_update_recurring_task_id).Text;
			string name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_recurring_task_name).Text;
			string lastCompletedDate = alertDialog.FindViewById<TextView>(Resource.Id.create_update_recurring_task_last_completed_date).Text;

			Task recurringTask = new Task()
			{
				Id = Convert.ToInt32(id),
				Name = name,
				LastCompletedDate = lastCompletedDate,
			};

			_dbHelper.UpdateTask(recurringTask);
			LoadData();
		}

		public void DeleteRecurringTaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			string id = alertDialog.FindViewById<TextView>(Resource.Id.create_update_recurring_task_id).Text;
			string name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_recurring_task_name).Text;

			_dbHelper.DeleteTask(Convert.ToInt32(id));
			LoadData();

			string text = $"{name} has been deleted";
			Toast.MakeText(Activity.Application, text, ToastLength.Short).Show();
		}

		public override void LoadData()
		{
			List<Task> recurringTasks = _dbHelper.GetTasksByTaskType(TaskTypeEnum.Recurring);
			RecurringTaskAdapter recurringTaskAdapter = new RecurringTaskAdapter(this, recurringTasks);
			ListView.Adapter = recurringTaskAdapter;
		}
	}
}
