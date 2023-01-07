using Android.Content;
using Android.Database;
using Android.Database.Sqlite;

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

		public DbHelper(Context context)
			: base(context, DB_NAME, null, DB_VERSION)
		{
		}

		public override void OnCreate(SQLiteDatabase db)
		{
			string query = $@"CREATE TABLE {DB_TABLE_TASK} ({DB_TASK_COLUMN_ID} INTEGER PRIMARY KEY AUTOINCREMENT, {DB_TASK_COLUMN_NAME} TEXT NOT NULL);";
			db.ExecSQL(query);
		}

		public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
		{
			string query = $"DELETE TABLE IF EXISTS {DB_TABLE_TASK}";
			db.ExecSQL(query);

			OnCreate(db);
		}

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
	}
}
