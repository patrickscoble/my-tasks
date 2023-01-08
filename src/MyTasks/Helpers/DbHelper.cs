using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
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

		private static string DB_TABLE_SCHEDULED_TASK = "ScheduledTask";
		private static string DB_SCHEDULED_TASK_COLUMN_ID = "Id";
		private static string DB_SCHEDULED_TASK_COLUMN_NAME = "Name";
		private static string DB_SCHEDULED_TASK_COLUMN_DATE = "Date";

		private static string DB_TABLE_RECURRING_TASK = "RecurringTask";
		private static string DB_RECURRING_TASK_COLUMN_ID = "Id";
		private static string DB_RECURRING_TASK_COLUMN_NAME = "Name";
		private static string DB_RECURRING_TASK_COLUMN_LAST_COMPLETED_DATE = "LastCompletedDate";

		public DbHelper(Context context)
			: base(context, DB_NAME, null, DB_VERSION)
		{
		}

		public override void OnCreate(SQLiteDatabase db)
		{
			string query = $@"CREATE TABLE {DB_TABLE_TASK} ({DB_TASK_COLUMN_ID} INTEGER PRIMARY KEY AUTOINCREMENT, {DB_TASK_COLUMN_NAME} TEXT NOT NULL);";
			db.ExecSQL(query);

			query = $@"CREATE TABLE {DB_TABLE_SCHEDULED_TASK} ({DB_SCHEDULED_TASK_COLUMN_ID} INTEGER PRIMARY KEY AUTOINCREMENT, {DB_SCHEDULED_TASK_COLUMN_NAME} TEXT NOT NULL, {DB_SCHEDULED_TASK_COLUMN_DATE} TEXT NOT NULL);";
			db.ExecSQL(query);

			query = $@"CREATE TABLE {DB_TABLE_RECURRING_TASK} ({DB_RECURRING_TASK_COLUMN_ID} INTEGER PRIMARY KEY AUTOINCREMENT, {DB_RECURRING_TASK_COLUMN_NAME} TEXT NOT NULL, {DB_RECURRING_TASK_COLUMN_LAST_COMPLETED_DATE} TEXT NOT NULL);";
			db.ExecSQL(query);
		}

		public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
		{
			string query = $"DELETE TABLE IF EXISTS {DB_TABLE_TASK}";
			db.ExecSQL(query);

			query = $"DELETE TABLE IF EXISTS {DB_TABLE_SCHEDULED_TASK}";
			db.ExecSQL(query);

			query = $"DELETE TABLE IF EXISTS {DB_TABLE_RECURRING_TASK}";
			db.ExecSQL(query);

			OnCreate(db);
		}

		#region Task

		public List<Task> GetAllTasks()
		{
			List<Task> tasks = new List<Task>();
			SQLiteDatabase db = this.ReadableDatabase;
			ICursor cursor = db.Query(DB_TABLE_TASK, new string[] { DB_TASK_COLUMN_ID, DB_TASK_COLUMN_NAME }, null, null, null, null, null);

			while (cursor.MoveToNext())
			{
				int idIndex = cursor.GetColumnIndex(DB_TASK_COLUMN_ID);
				int id = cursor.GetInt(idIndex);

				int nameIndex = cursor.GetColumnIndex(DB_TASK_COLUMN_NAME);
				string name = cursor.GetString(nameIndex) ?? string.Empty;

				tasks.Add(new Task()
				{
					Id = id,
					Name = name,
				});
			}

			return tasks;
		}

		public void CreateTask(Task task)
		{
			SQLiteDatabase db = this.WritableDatabase;
			ContentValues values = new ContentValues();
			values.Put(DB_TASK_COLUMN_NAME, task.Name);
			db.Insert(DB_TABLE_TASK, null, values);
			db.Close();
		}

		public void UpdateTask(Task task)
		{
			SQLiteDatabase db = this.WritableDatabase;
			ContentValues values = new ContentValues();
			values.Put(DB_TASK_COLUMN_NAME, task.Name);
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
		
		#region Scheduled Task

		public List<ScheduledTask> GetAllScheduledTasks()
		{
			List<ScheduledTask> scheduledTasks = new List<ScheduledTask>();
			SQLiteDatabase db = this.ReadableDatabase;
			ICursor cursor = db.Query(DB_TABLE_SCHEDULED_TASK, new string[] { DB_SCHEDULED_TASK_COLUMN_ID, DB_SCHEDULED_TASK_COLUMN_NAME, DB_SCHEDULED_TASK_COLUMN_DATE }, null, null, null, null, null);

			while (cursor.MoveToNext())
			{
				int idIndex = cursor.GetColumnIndex(DB_SCHEDULED_TASK_COLUMN_ID);
				int id = cursor.GetInt(idIndex);

				int nameIndex = cursor.GetColumnIndex(DB_SCHEDULED_TASK_COLUMN_NAME);
				string name = cursor.GetString(nameIndex) ?? string.Empty;

				int dateIndex = cursor.GetColumnIndex(DB_SCHEDULED_TASK_COLUMN_DATE);
				string date = cursor.GetString(dateIndex);

				scheduledTasks.Add(new ScheduledTask()
				{
					Id = id,
					Name = name,
					Date = date,
				});
			}

			return scheduledTasks;
		}

		public void CreateScheduledTask(ScheduledTask scheduledTask)
		{
			SQLiteDatabase db = this.WritableDatabase;
			ContentValues values = new ContentValues();
			values.Put(DB_SCHEDULED_TASK_COLUMN_NAME, scheduledTask.Name);
			values.Put(DB_SCHEDULED_TASK_COLUMN_DATE, scheduledTask.Date);
			db.Insert(DB_TABLE_SCHEDULED_TASK, null, values);
			db.Close();
		}

		public void UpdateScheduledTask(ScheduledTask scheduledTask)
		{
			SQLiteDatabase db = this.WritableDatabase;
			ContentValues values = new ContentValues();
			values.Put(DB_SCHEDULED_TASK_COLUMN_NAME, scheduledTask.Name);
			values.Put(DB_SCHEDULED_TASK_COLUMN_DATE, scheduledTask.Date);
			db.Update(DB_TABLE_SCHEDULED_TASK, values, $"{DB_SCHEDULED_TASK_COLUMN_ID} = ?", new string[] { scheduledTask.Id.ToString() });
			db.Close();
		}

		public void DeleteScheduledTask(int id)
		{
			SQLiteDatabase db = this.WritableDatabase;
			db.Delete(DB_TABLE_SCHEDULED_TASK, $"{DB_SCHEDULED_TASK_COLUMN_ID} = ?", new string[] { id.ToString() });
			db.Close();
		}

		#endregion Scheduled Task

		#region Recurring Task

		public List<RecurringTask> GetAllRecurringTasks()
		{
			List<RecurringTask> recurringTasks = new List<RecurringTask>();
			SQLiteDatabase db = this.ReadableDatabase;
			ICursor cursor = db.Query(DB_TABLE_RECURRING_TASK, new string[] { DB_RECURRING_TASK_COLUMN_ID, DB_RECURRING_TASK_COLUMN_NAME, DB_RECURRING_TASK_COLUMN_LAST_COMPLETED_DATE }, null, null, null, null, null);

			while (cursor.MoveToNext())
			{
				int idIndex = cursor.GetColumnIndex(DB_RECURRING_TASK_COLUMN_ID);
				int id = cursor.GetInt(idIndex);

				int nameIndex = cursor.GetColumnIndex(DB_RECURRING_TASK_COLUMN_NAME);
				string name = cursor.GetString(nameIndex) ?? string.Empty;

				int lastCompletedDateIndex = cursor.GetColumnIndex(DB_RECURRING_TASK_COLUMN_LAST_COMPLETED_DATE);
				string lastCompletedDate = cursor.GetString(lastCompletedDateIndex);

				recurringTasks.Add(new RecurringTask()
				{
					Id = id,
					Name = name,
					LastCompletedDate = lastCompletedDate,
				});
			}

			return recurringTasks;
		}

		public void CreateRecurringTask(RecurringTask recurringTask)
		{
			SQLiteDatabase db = this.WritableDatabase;
			ContentValues values = new ContentValues();
			values.Put(DB_RECURRING_TASK_COLUMN_NAME, recurringTask.Name);
			values.Put(DB_RECURRING_TASK_COLUMN_LAST_COMPLETED_DATE, string.Empty);
			db.Insert(DB_TABLE_RECURRING_TASK, null, values);
			db.Close();
		}

		public void UpdateRecurringTask(RecurringTask recurringTask)
		{
			SQLiteDatabase db = this.WritableDatabase;
			ContentValues values = new ContentValues();
			values.Put(DB_RECURRING_TASK_COLUMN_NAME, recurringTask.Name);
			values.Put(DB_RECURRING_TASK_COLUMN_LAST_COMPLETED_DATE, recurringTask.LastCompletedDate);
			db.Update(DB_TABLE_RECURRING_TASK, values, $"{DB_RECURRING_TASK_COLUMN_ID} = ?", new string[] { recurringTask.Id.ToString() });
			db.Close();
		}

		public void DeleteRecurringTask(int id)
		{
			SQLiteDatabase db = this.WritableDatabase;
			db.Delete(DB_TABLE_RECURRING_TASK, $"{DB_RECURRING_TASK_COLUMN_ID} = ?", new string[] { id.ToString() });
			db.Close();
		}

		#endregion Recurring Task
	}
}
