using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.Delegates;

namespace DbComparator.App.ViewModels
{
	class AboutViewModel : ModelBase, IAboutVM
    {
		public event NotifyDelegate CloseHandler;

		private string _aboutProgram;

		private string _technicalInfo;

		public string AboutProgram
		{
			get => _aboutProgram; 
			set => SetProperty(ref _aboutProgram, value, "AboutProgram");
		}

		public string TechnicalInfo
		{
			get => _technicalInfo; 
			set => SetProperty(ref _technicalInfo, value, "TechnicalInfo");
		}

		public AboutViewModel()
		{
			_aboutProgram = GetAboutProgramString();
			_technicalInfo = GetTechnicalInfoString();
		}

		private RellayCommand _closeCommand;

		public RellayCommand CloseCommand =>
			 _closeCommand = new RellayCommand(c => CloseHandler?.Invoke());

		/// <summary>
		/// Returns a string with technical information
		/// </summary>
		/// <returns>Line with technical information</returns>
		private string GetTechnicalInfoString()
		{
			string version = "1.0.0";
			return string.Format($"Version: {version}\n\r");
		}

		/// <summary>
		/// Returns a string with information about the program
		/// </summary>
		/// <returns>A string with information about the program</returns>
		private string GetAboutProgramString()
		{
			return "     Desktop applications with a graphical interface built with using WPF and the MVVM application " +
				   "architecture design template. Application task: connecting to databases(MsSql, SyBase, MySql) " +
				   "and collecting data about their structure. With visualization of changes and the ability to " +
				   "automatic reconciliation of the specified databases.";
		}
	}
}
