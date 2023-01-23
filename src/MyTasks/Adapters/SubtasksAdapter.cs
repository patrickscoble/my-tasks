using Android.Content;
using Android.Views;
using MyTasks.Fragments;
using MyTasks.Models;

namespace MyTasks.Adapters
{
	internal class SubtaskAdapter : BaseAdapter<Subtask>
	{
		private SubtasksFragment _subtasksFragment;

		public SubtaskAdapter(SubtasksFragment subtasksFragment, List<Subtask> tasks)
			: base(tasks)
		{
			this._subtasksFragment = subtasksFragment;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LayoutInflater inflater = (LayoutInflater)_subtasksFragment.Activity.GetSystemService(Context.LayoutInflaterService);
			View view = inflater.Inflate(Resource.Layout.subtask, null);

			Subtask subtask = Tasks[position];

			view.FindViewById<TextView>(Resource.Id.subtask_name).Text = subtask.Name;

			TextView lastCompletedDate = view.FindViewById<TextView>(Resource.Id.subtask_last_completed_date);
			lastCompletedDate.Text = subtask.LastCompletedDate != null ? $"Last Completed {subtask.LastCompletedDate}" : null;
			lastCompletedDate.Visibility = subtask.LastCompletedDate != null ? ViewStates.Visible : ViewStates.Gone;

			view.Click += delegate
			{
				LayoutInflater layoutInflater = LayoutInflater.From(_subtasksFragment.Activity);
				View view = layoutInflater.Inflate(Resource.Layout.create_update_subtask, null);

				AlertDialog.Builder builder = new AlertDialog.Builder(_subtasksFragment.Activity);
				builder.SetTitle("Update Subtask");
				builder.SetView(view);
				builder.SetPositiveButton("Update", _subtasksFragment.UpdateSubtaskAction);
				builder.SetNeutralButton("Delete", _subtasksFragment.DeleteSubtaskAction);
				builder.SetNegativeButton("Cancel", _subtasksFragment.CancelAction);

				// Prepopulate the fields.
				view.FindViewById<TextView>(Resource.Id.create_update_subtask_id).Text = subtask.Id.ToString();
				view.FindViewById<TextView>(Resource.Id.create_update_subtask_name).Text = subtask.Name;

				builder.Show();
			};

			return view;
		}
	}
}
