using Android.Content;
using Android.Views;
using AndroidX.Core.View;
using MyTasks.Adapters;
using MyTasks.Enums;

using Task = MyTasks.Models.Task;

namespace MyTasks.Fragments
{
	public class TasksFragment : BaseFragment, IMenuProvider
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
			inflater.Inflate(Resource.Menu.tasks_menu, menu);
		}

		public bool OnMenuItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.action_add:
					{
						LayoutInflater layoutInflater = LayoutInflater.From(Activity);
						View view = layoutInflater.Inflate(Resource.Layout.create_update_task, null);

						AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
						builder.SetTitle("Create Task");
						builder.SetView(view);
						builder.SetPositiveButton("Create", CreateTaskAction);
						builder.SetNegativeButton("Cancel", CancelAction);

						builder.Show();
						return true;
					}
			}

			return false;
		}

		private void CreateTaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			string name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_task_name).Text;

			Task task = new Task()
			{
				Name = name,
				TaskType = TaskTypeEnum.None,
			};

			_dbHelper.CreateTask(task);
			LoadData();
		}

		public override void LoadData()
		{
			Activity.Title = Resources.GetString(Resource.String.title_tasks);

			List<Task> tasks = _dbHelper.GetAllTasks().Where(x => x.TaskType == TaskTypeEnum.None).ToList();
			TaskAdapter taskAdapter = new TaskAdapter(this, tasks);
			ListView.Adapter = taskAdapter;
		}
	}
}
