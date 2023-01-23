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

		public override void LoadData()
		{
			Activity.Title = Resources.GetString(Resource.String.title_recurring_tasks);

			List<Task> recurringTasks = _dbHelper.GetTasksByTaskType(TaskTypeEnum.Recurring);
			RecurringTaskAdapter recurringTaskAdapter = new RecurringTaskAdapter(this, recurringTasks);
			ListView.Adapter = recurringTaskAdapter;
		}
	}
}
