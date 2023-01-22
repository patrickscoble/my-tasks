using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using MyTasks.Enums;

using Task = MyTasks.Models.Task;

namespace MyTasks.Helpers
{
	public class DbHelper : SQLiteOpenHelper
	{
		private static string DB_NAME = "MyTasks";
		private static int DB_VERSION = 1;

		private static string DB_TABLE_TASK = "Task";
		private static string DB_TASK_COLUMN_ID = "Id";
		private static string DB_TASK_COLUMN_NAME = "Name";
		private static string DB_TASK_COLUMN_TASK_TYPE = "TaskType";
		private static string DB_TASK_COLUMN_DATE = "Date";
		private static string DB_TASK_COLUMN_LAST_COMPLETED_DATE = "LastCompletedDate";

		public DbHelper(Context context)
			: base(context, DB_NAME, null, DB_VERSION)
		{
		}

		public override void OnCreate(SQLiteDatabase db)
		{
			string query = $@"
				CREATE TABLE {DB_TABLE_TASK}
				(
					{DB_TASK_COLUMN_ID} INTEGER PRIMARY KEY AUTOINCREMENT,
					{DB_TASK_COLUMN_NAME} TEXT NOT NULL,
					{DB_TASK_COLUMN_TASK_TYPE} TEXT NOT NULL,
					{DB_TASK_COLUMN_DATE} TEXT,
					{DB_TASK_COLUMN_LAST_COMPLETED_DATE} TEXT
				);";

			db.ExecSQL(query);
		}

		public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
		{
			string query = $"DELETE TABLE IF EXISTS {DB_TABLE_TASK}";
			db.ExecSQL(query);

			OnCreate(db);
		}

		#region Task

		public List<Task> GetAllTasks()
		{
			List<Task> tasks = new List<Task>();
			SQLiteDatabase db = this.ReadableDatabase;
			ICursor cursor = db.Query(DB_TABLE_TASK, new string[] { DB_TASK_COLUMN_ID, DB_TASK_COLUMN_NAME, DB_TASK_COLUMN_TASK_TYPE, DB_TASK_COLUMN_DATE, DB_TASK_COLUMN_LAST_COMPLETED_DATE }, null, null, null, null, null);

			while (cursor.MoveToNext())
			{
				int idIndex = cursor.GetColumnIndex(DB_TASK_COLUMN_ID);
				int id = cursor.GetInt(idIndex);

				int nameIndex = cursor.GetColumnIndex(DB_TASK_COLUMN_NAME);
				string name = cursor.GetString(nameIndex);

				int taskTypeIndex = cursor.GetColumnIndex(DB_TASK_COLUMN_TASK_TYPE);
				string taskType = cursor.GetString(taskTypeIndex);

				int dateIndex = cursor.GetColumnIndex(DB_TASK_COLUMN_DATE);
				string date = cursor.GetString(dateIndex);

				int lastCompletedDateIndex = cursor.GetColumnIndex(DB_TASK_COLUMN_LAST_COMPLETED_DATE);
				string lastCompletedDate = cursor.GetString(lastCompletedDateIndex);

				tasks.Add(new Task()
				{
					Id = id,
					Name = name,
					TaskType = Enum.Parse<TaskTypeEnum>(taskType),
					Date = date,
					LastCompletedDate = lastCompletedDate,
				});
			}

			return tasks;
		}

		public List<Task> GetTasksByTaskType(TaskTypeEnum taskType)
		{
			List<Task> tasks = new List<Task>();
			SQLiteDatabase db = this.ReadableDatabase;
			ICursor cursor = db.Query(DB_TABLE_TASK, new string[] { DB_TASK_COLUMN_ID, DB_TASK_COLUMN_NAME, DB_TASK_COLUMN_DATE, DB_TASK_COLUMN_LAST_COMPLETED_DATE }, $"{DB_TASK_COLUMN_TASK_TYPE} = ?", new string[] { taskType.ToString() }, null, null, null, null);

			while (cursor.MoveToNext())
			{
				int idIndex = cursor.GetColumnIndex(DB_TASK_COLUMN_ID);
				int id = cursor.GetInt(idIndex);

				int nameIndex = cursor.GetColumnIndex(DB_TASK_COLUMN_NAME);
				string name = cursor.GetString(nameIndex);

				int dateIndex = cursor.GetColumnIndex(DB_TASK_COLUMN_DATE);
				string date = cursor.GetString(dateIndex);

				int lastCompletedDateIndex = cursor.GetColumnIndex(DB_TASK_COLUMN_LAST_COMPLETED_DATE);
				string lastCompletedDate = cursor.GetString(lastCompletedDateIndex);

				tasks.Add(new Task()
				{
					Id = id,
					Name = name,
					TaskType = taskType,
					Date = date,
					LastCompletedDate = lastCompletedDate,
				});
			}

			return tasks;
		}

		public void CreateTask(Task task)
		{
			SQLiteDatabase db = this.WritableDatabase;
			ContentValues values = new ContentValues();
			values.Put(DB_TASK_COLUMN_NAME, task.Name);
			values.Put(DB_TASK_COLUMN_TASK_TYPE, task.TaskType.ToString());
			values.Put(DB_TASK_COLUMN_DATE, task.Date);
			values.Put(DB_TASK_COLUMN_LAST_COMPLETED_DATE, task.LastCompletedDate);
			db.Insert(DB_TABLE_TASK, null, values);
			db.Close();
		}

		public void UpdateTask(Task task)
		{
			SQLiteDatabase db = this.WritableDatabase;
			ContentValues values = new ContentValues();
			values.Put(DB_TASK_COLUMN_NAME, task.Name);
			values.Put(DB_TASK_COLUMN_DATE, task.Date);
			values.Put(DB_TASK_COLUMN_LAST_COMPLETED_DATE, task.LastCompletedDate);
			db.Update(DB_TABLE_TASK, values, $"{DB_TASK_COLUMN_ID} = ?", new string[] { task.Id.ToString() });
			db.Close();
		}

		public void DeleteTask(int id)
		{
			SQLiteDatabase db = this.WritableDatabase;
			db.Delete(DB_TABLE_TASK, $"{DB_TASK_COLUMN_ID} = ?", new string[] { id.ToString() });
			db.Close();
		}

		#endregion Task
	}
}
