using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using MyTasks.Enums;
using MyTasks.Models;

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

		private static string DB_TABLE_SUBTASK = "Subtask";
		private static string DB_SUBTASK_COLUMN_ID = "Id";
		private static string DB_SUBTASK_COLUMN_TASK_ID = "TaskId";
		private static string DB_SUBTASK_COLUMN_NAME = "Name";
		private static string DB_SUBTASK_COLUMN_LAST_COMPLETED_DATE = "LastCompletedDate";
		private static string DB_SUBTASK_COLUMN_DONE = "Done";

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

			query = $@"
				CREATE TABLE {DB_TABLE_SUBTASK}
				(
					{DB_SUBTASK_COLUMN_ID} INTEGER PRIMARY KEY AUTOINCREMENT,
					{DB_SUBTASK_COLUMN_TASK_ID} INTEGER NOT NULL,
					{DB_SUBTASK_COLUMN_NAME} TEXT NOT NULL,
					{DB_SUBTASK_COLUMN_LAST_COMPLETED_DATE} TEXT,
					{DB_SUBTASK_COLUMN_DONE} BOOLEAN NOT NULL,
					CONSTRAINT FK_TaskId FOREIGN KEY ({DB_SUBTASK_COLUMN_TASK_ID}) REFERENCES {DB_TABLE_TASK}({DB_TASK_COLUMN_ID})
				);";

			db.ExecSQL(query);
		}

		public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
		{
			string query = $"DELETE TABLE IF EXISTS {DB_TABLE_SUBTASK}";
			db.ExecSQL(query);

			query = $"DELETE TABLE IF EXISTS {DB_TABLE_TASK}";
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
			List<Subtask> subtasks = GetSubtasksByTaskId(id);
			foreach (Subtask subtask in subtasks)
			{
				DeleteSubtask(subtask.Id);
			}

			SQLiteDatabase db = this.WritableDatabase;
			db.Delete(DB_TABLE_TASK, $"{DB_TASK_COLUMN_ID} = ?", new string[] { id.ToString() });
			db.Close();
		}

		#endregion Task

		#region Subtask

		public List<Subtask> GetSubtasksByTaskId(int taskId)
		{
			List<Subtask> subtasks = new List<Subtask>();
			SQLiteDatabase db = this.ReadableDatabase;
			ICursor cursor = db.Query(DB_TABLE_SUBTASK, new string[] { DB_SUBTASK_COLUMN_ID, DB_SUBTASK_COLUMN_NAME, DB_SUBTASK_COLUMN_LAST_COMPLETED_DATE, DB_SUBTASK_COLUMN_DONE }, $"{DB_SUBTASK_COLUMN_TASK_ID} = ?", new string[] { taskId.ToString() }, null, null, null, null);

			while (cursor.MoveToNext())
			{
				int idIndex = cursor.GetColumnIndex(DB_SUBTASK_COLUMN_ID);
				int id = cursor.GetInt(idIndex);

				int nameIndex = cursor.GetColumnIndex(DB_SUBTASK_COLUMN_NAME);
				string name = cursor.GetString(nameIndex);

				int lastCompletedDateIndex = cursor.GetColumnIndex(DB_SUBTASK_COLUMN_LAST_COMPLETED_DATE);
				string lastCompletedDate = cursor.GetString(lastCompletedDateIndex);

				int doneIndex = cursor.GetColumnIndex(DB_SUBTASK_COLUMN_DONE);
				bool done = cursor.GetInt(doneIndex) > 0;

				subtasks.Add(new Subtask()
				{
					Id = id,
					Name = name,
					TaskId = taskId,
					LastCompletedDate = lastCompletedDate,
					Done = done,
				});
			}

			return subtasks;
		}
		public void CreateSubtask(Subtask subtask)
		{
			SQLiteDatabase db = this.WritableDatabase;
			ContentValues values = new ContentValues();
			values.Put(DB_SUBTASK_COLUMN_TASK_ID, subtask.TaskId);
			values.Put(DB_SUBTASK_COLUMN_NAME, subtask.Name);
			values.Put(DB_SUBTASK_COLUMN_LAST_COMPLETED_DATE, subtask.LastCompletedDate);
			values.Put(DB_SUBTASK_COLUMN_DONE, subtask.Done);
			db.Insert(DB_TABLE_SUBTASK, null, values);
			db.Close();
		}

		public void UpdateSubtask(Subtask subtask)
		{
			SQLiteDatabase db = this.WritableDatabase;
			ContentValues values = new ContentValues();
			values.Put(DB_SUBTASK_COLUMN_NAME, subtask.Name);
			values.Put(DB_SUBTASK_COLUMN_LAST_COMPLETED_DATE, subtask.LastCompletedDate);
			values.Put(DB_SUBTASK_COLUMN_DONE, subtask.Done);
			db.Update(DB_TABLE_SUBTASK, values, $"{DB_SUBTASK_COLUMN_ID} = ?", new string[] { subtask.Id.ToString() });
			db.Close();
		}

		public void DeleteSubtask(int id)
		{
			SQLiteDatabase db = this.WritableDatabase;
			db.Delete(DB_TABLE_SUBTASK, $"{DB_SUBTASK_COLUMN_ID} = ?", new string[] { id.ToString() });
			db.Close();
		}

		#endregion Subtask
	}
}
