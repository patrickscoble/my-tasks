using Android.Content;
using Android.Views;
using MyTasks.Helpers;

using ListFragment = AndroidX.Fragment.App.ListFragment;

namespace MyTasks.Fragments
{
	public class BaseFragment : ListFragment
	{
		protected DbHelper _dbHelper { get { return ((MainActivity)Activity).DbHelper; } }

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
			LoadData();
		}
		
		public void CancelAction(object sender, DialogClickEventArgs e)
		{
		}

		public virtual void LoadData()
		{
		}
	}
}
