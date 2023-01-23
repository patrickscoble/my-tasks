using Android.Views;
using AndroidX.AppCompat.App;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;
using MyTasks.Fragments;
using MyTasks.Helpers;

namespace MyTasks
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, Icon = "@drawable/task_48")]
	public class MainActivity : AppCompatActivity, NavigationBarView.IOnItemSelectedListener
	{
		internal DbHelper DbHelper;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SupportFragmentManager.BeginTransaction().Add(Resource.Id.container, new ToDosFragment(), "toDosFragment").Commit();

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);

			DbHelper = new DbHelper(this);

			NavigationBarView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
			navigation.SetOnItemSelectedListener(this);
		}

		public bool OnNavigationItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.navigation_to_do:
				{
					SupportFragmentManager.BeginTransaction().Replace(Resource.Id.container, new ToDosFragment(), "toDosFragment").Commit();
					return true;
				}

				case Resource.Id.navigation_tasks:
				{
					SupportFragmentManager.BeginTransaction().Replace(Resource.Id.container, new TasksFragment(), "tasksFragment").Commit();
					return true;
				}
				case Resource.Id.navigation_scheduled_tasks:
				{
					SupportFragmentManager.BeginTransaction().Replace(Resource.Id.container, new ScheduledTasksFragment(), "scheduledTasksFragment").Commit();
					return true;
				}
			case Resource.Id.navigation_recurring_tasks:
				{
					SupportFragmentManager.BeginTransaction().Replace(Resource.Id.container, new RecurringTasksFragment(), "recurringTasksFragment").Commit();
					return true;
				}
			}

			return false;
		}
	}
}
