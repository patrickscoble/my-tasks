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
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Activity.Title = Resources.GetString(Resource.String.title_tasks);
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

		public void UpdateTaskAction(object sender, DialogClickEventArgs e)
		{
			AlertDialog alertDialog = (AlertDialog)sender;

			string id = alertDialog.FindViewById<TextView>(Resource.Id.create_update_task_id).Text;
			string name = alertDialog.FindViewById<EditText>(Resource.Id.create_update_task_name).Text;

			Task task = new Task()
			{
				Id = Convert.ToInt32(id),
				Name = name,
			};

			_dbHelper.UpdateTask(task);
			LoadData();
		}

		public override void LoadData()
		{
			List<Task> tasks = _dbHelper.GetAllTasks().Where(x => x.TaskType == TaskTypeEnum.None)

			List<Task> allTasks = new List<Task>();
			allTasks.AddRange(tasks.Where(x => x.TaskType == TaskTypeEnum.Recurring && (string.IsNullOrEmpty(x.LastCompletedDate) || Convert.ToDateTime(x.LastCompletedDate) < DateTime.Now.Date)));
			allTasks.AddRange(tasks.Where(x => x.TaskType == TaskTypeEnum.Scheduled && Convert.ToDateTime(x.Date) <= DateTime.Now.Date));
			allTasks.AddRange(tasks.Where(x => x.TaskType == TaskTypeEnum.None));

			TaskAdapter taskAdapter = new TaskAdapter(this, allTasks, _dbHelper);
			ListView.Adapter = taskAdapter;
		}
	}
}
