using Android.Content;
using Android.Views;
using MyTasks.Adapters;
using MyTasks.Enums;
using MyTasks.Helpers;
using MyTasks.Models;

using Fragment = AndroidX.Fragment.App.Fragment;
using Task = MyTasks.Models.Task;

namespace MyTasks.Fragments
{
	public class ToDosFragment : Fragment
	{
		private ExpandableListView _expandableListView;

		private DbHelper _dbHelper { get { return ((MainActivity)Activity).DbHelper; } }

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Activity.Title = Resources.GetString(Resource.String.title_to_do);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);

			View view = inflater.Inflate(Resource.Layout.to_dos, container, false);

			_expandableListView = view.FindViewById<ExpandableListView>(Resource.Id.expandable_list_view);

			return view;
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
			LoadData();
		}

		public void CancelAction(object sender, DialogClickEventArgs e)
		{
		}

		public void LoadData()
		{
			List<Task> tasks = _dbHelper.GetAllTasks();

			List<Task> allTasks = new List<Task>();
			allTasks.AddRange(tasks.Where(x => x.TaskType == TaskTypeEnum.Recurring && (string.IsNullOrEmpty(x.LastCompletedDate) || Convert.ToDateTime(x.LastCompletedDate) < DateTime.Now.Date)));
			allTasks.AddRange(tasks.Where(x => x.TaskType == TaskTypeEnum.Scheduled && Convert.ToDateTime(x.Date) <= DateTime.Now.Date));
			allTasks.AddRange(tasks.Where(x => x.TaskType == TaskTypeEnum.None));

			Dictionary<Task, List<Subtask>> tasksWithSubtasks = new Dictionary<Task, List<Subtask>>();

			foreach (Task task in allTasks)
			{
				List<Subtask> subtasks = _dbHelper.GetSubtasksByTaskId(task.Id);
				tasksWithSubtasks.Add(task, subtasks);
			}

			ToDoAdapter toDoAdapter = new ToDoAdapter(this, tasksWithSubtasks, _dbHelper);

			_expandableListView.SetAdapter(toDoAdapter);
		}
	}
}
