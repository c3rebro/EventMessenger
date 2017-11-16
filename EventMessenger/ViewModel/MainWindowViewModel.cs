using EventMessenger;
using EventMessenger.EventObjects;
using EventMessenger.Model.ResponseObjects;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Hardcodet.Wpf.TaskbarNotification;

using MvvmDialogs.ViewModels;

using RedCell.Diagnostics.Update;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Controls;


namespace EventMessenger.ViewModel
{
	/// <summary>
	/// The summary is a brief description of the type or type member and will
	/// be displayed in IntelliSense and the Object Browser.
	/// </summary>
	/// <remarks>The remarks element should be used to provide more detailed information
	/// about the type or member such as how it is used, its processing, etc.
	/// 
	/// <para>Remarks will only appear in the help file and can be as long as
	/// necessary.</para></remarks>
	public class MainWindowViewModel : ViewModelBase
	{
		
		#region fields
		
		readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;
		
		private readonly ObservableCollection<MenuItem> rowContextMenuItems;
		private readonly ObservableCollection<MenuItem> emptySpaceContextMenuItems;
		
		private readonly RelayCommand _cmdDeleteEntry;
		private readonly RelayCommand _cmdAddNewEvent;
		private readonly RelayCommand _cmdAddNewResponse;
		
		private readonly ResourceLoader resLoader;
		private SettingsReaderWriter settingsReaderWriter;
		private Updater updater;
		private PipeServer pipeServer;
		private EventObjectHandler eventObjectHandler;
		private DatabaseReaderWriter dbReaderWriter;
		
		private bool firstRun = true;
		private bool runSilent = false;
		private TaskbarIcon tb;
		private MainWindow mw;
		
		private Mutex mutex;
		
		private readonly string assemblyFileName = Process.GetCurrentProcess().MainModule.FileName;
		
		#endregion

		#region events / delegates

		/// <summary>
		/// will raise notifier to inform user about available updates
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void updateReady(object sender, EventArgs e);
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// init main
		/// </summary>
		public MainWindowViewModel()
		{
			
			RunMutex(this,null);
			
			try{
				dialogs = new ObservableCollection<IDialogViewModel>();
				updater = new Updater();

				updater.isUserNotified = Environment.GetCommandLineArgs().Contains<string>("IAmNotified_DoUpdate");
				
				runSilent = Environment.GetCommandLineArgs().Contains<string>("runSilent");
			}
			
			catch(Exception e){
				LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
				return;
			}
			
			if(!updater.isUserNotified){
				settingsReaderWriter = new SettingsReaderWriter();
				dbReaderWriter = new DatabaseReaderWriter();
			}
			else{
				updater.allowUpdate = true;
				firstRun = false;
				
				updater.StartMonitoring();
			}
			
			try{
				resLoader = new ResourceLoader();
				pipeServer = new PipeServer();
				eventObjectHandler = new EventObjectHandler();
				
				tb = (TaskbarIcon) Application.Current.FindResource("TrayNotifyIcon");
			}
			catch(Exception e){
				LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
			}

			mainWindowIcon = new BitmapImage();
			mainWindowIcon.BeginInit();
			mainWindowIcon.UriSource = new Uri("pack://application:,,,/EventMessenger;component/Resources/logo.ico");

			mainWindowIcon.EndInit();
			
			tb.IconSource = mainWindowIcon;
			
			//logReader = new LogFileReaderWriter(10000, true); // just for testing purposes

			_cmdDeleteEntry = new RelayCommand(DeleteRow);
			_cmdAddNewEvent = new RelayCommand(AddNewEvent);
			_cmdAddNewResponse = new RelayCommand(AddNewResponse);
			
			rowContextMenuItems = new ObservableCollection<MenuItem>();
			emptySpaceContextMenuItems = new ObservableCollection<MenuItem>();
			
			emptySpaceContextMenuItems.Add(new MenuItem(){
			                               	Header = resLoader.getResource("contextMenuItemAddNewEvent"),
			                               	Command = _cmdAddNewEvent
			                               });
			
			rowContextMenuItems.Add(new MenuItem(){
			                        	Header = resLoader.getResource("contextMenuItemAddOrEditResponse"),
			                        	Command = _cmdAddNewResponse
			                        });
			rowContextMenuItems.Add(new MenuItem(){
			                        	Header = resLoader.getResource("contextMenuItemEditEvent"),
			                        	//ToolTip = "this will be the helptext",
			                        	Command = _cmdAddNewEvent
			                        });

			rowContextMenuItems.Add(new MenuItem(){
			                        	Header = resLoader.getResource("contextMenuItemDeleteSelectedItem"),
			                        	Command = _cmdDeleteEntry
			                        });

			//dataGridSource = new ObservableCollection<EventObjectModel>();

			Application.Current.MainWindow.Closing += new CancelEventHandler(CloseThreads);
			Application.Current.MainWindow.Activated += new EventHandler(LoadCompleted);
			updater.newVersionAvailable += new EventHandler(AskForUpdateNow);
			tb.TrayLeftMouseDown += new RoutedEventHandler(RestoreMainWindow);
			tb.MouseDown += new MouseButtonEventHandler(RestoreMainWindow);
			//any dialog boxes added in the constructor won't appear until DialogBehavior.DialogViewModels gets bound to the Dialogs collection.
		}
		
		#endregion
		
		#region Collections
		
		/// <summary>
		/// Expose Dialogs
		/// </summary>
		public ObservableCollection<IDialogViewModel> Dialogs {
			get { return dialogs; }
		} private ObservableCollection<IDialogViewModel> dialogs;
		
		/// <summary>
		/// expose contextmenu on row click
		/// </summary>
		public ObservableCollection<MenuItem> RowContextMenu {
			get {
				return rowContextMenuItems;
			}
		}

		/// <summary>
		/// expose context menu on blank area click
		/// </summary>
		public ObservableCollection<MenuItem> EmptySpaceContextMenu {
			get {
				return emptySpaceContextMenuItems;
			}
		}

		/// <summary>
		/// expose row content
		/// </summary>
		public ObservableCollection<EventObjectModel> DataGridSource {
			get {
				return eventObjectHandler.StaticEvents; }
			set {
				eventObjectHandler.StaticEvents = value;
				RaisePropertyChanged("DataGridSource");
			}
		}
		
		#endregion
		
		#region Localization

		/// <summary>
		/// Expose translated strings from ResourceLoader
		/// </summary>
		public string LocalizationResourceSet { get; set; }
		
		#endregion
		
		#region Commands
		
		private void AddNewResponse()
		{
			this.Dialogs.Add(new ResponseEditorDialogViewModel(eventObjectHandler.StaticEvents, SelectedDataGridItem)
			                 {
			                 	OnOk = (sender) =>
			                 	{
			                 		if(sender.ResponseCollection != null && sender.ResponseCollection.Count > 0)
			                 		{
			                 			SelectedDataGridItem.ResponseCollection = sender.ResponseCollection;
			                 			SelectedResponse = SelectedDataGridItem.ResponseCollection[0];
			                 			
			                 			RaisePropertyChanged("DataGridSource");
			                 			RaisePropertyChanged("SelectedResponse");
			                 		}
			                 		
			                 		
			                 		sender.Close();

			                 	},

			                 	OnCancel = (sender) =>
			                 	{
			                 		sender.Close();
			                 	},
			                 	
			                 	OnCloseRequest = (sender) => {
			                 		sender.Close();
			                 	}
			                 });
		}
		
		private void AddNewEvent()
		{
			this.Dialogs.Add(new EventEditorDialogViewModel(DataGridSource, SelectedDataGridItem)
			                 {
			                 	OnOk = (sender) =>
			                 	{
			                 		
			                 		//TODO: Test for Duplicate entrys / Overwrite or create new?
			                 		
			                 		if(sender.HasConfiguration && sender.ObjectModel.EventType == sender.SelectedGeneralEventType)
			                 		{
			                 			if(selectedDataGridItem == null)
			                 			{
			                 				if(DataGridSource.Contains(sender.ObjectModel))
			                 				{
			                 					if (new MessageBoxViewModel {
			                 					    	Caption = resLoader.getResource("messageBoxEventAlreadyExistCaption"),
			                 					    	Message = resLoader.getResource("messageBoxEventAlreadyExistMessage"),
			                 					    	Buttons = MessageBoxButton.OK,
			                 					    	Image = MessageBoxImage.Information
			                 					    }
			                 					    .Show(this.Dialogs) == MessageBoxResult.OK)
			                 						return;
			                 				}
			                 				else
			                 				{
			                 					DataGridSource.Add(sender.ObjectModel);
			                 				}

			                 			}
			                 			
			                 			else{
			                 				SelectedDataGridItem.EventName = sender.EventName;
			                 				SelectedDataGridItem.EventDescription = sender.EventDescription;
			                 			}
			                 			sender.Close();
			                 		}

			                 		else{
			                 			new MessageBoxViewModel {
			                 				Caption = resLoader.getResource("messageBoxMissingConfigCaption"),
			                 				Message = resLoader.getResource("messageBoxMissingConfigMessage"),
			                 				Buttons = MessageBoxButton.OK,
			                 				Image = MessageBoxImage.Information
			                 			}
			                 			.Show(this.Dialogs);
			                 		}

			                 		RaisePropertyChanged("DataGridSource");
			                 	},

			                 	OnCancel = (sender) =>
			                 	{
			                 		sender.Close();
			                 	},
			                 	
			                 	OnCloseRequest = (sender) => {
			                 		sender.Close();
			                 	}
			                 });
		}
		
		private void DeleteRow()
		{
			DataGridSource.Remove(selectedDataGridItem);
		}
		
		/// <summary>
		/// Expose Command to Save As Menu Item
		/// </summary>
		public ICommand SaveToProjectFileDialogCommand { get { return new RelayCommand(OnNewSaveToProjectFileCommand); } }
		private void OnNewSaveToProjectFileCommand()
		{
			var dlg = new SaveFileDialogViewModel {
				Title = "Select a file (I won't actually do anything with it)",
				Filter = "All files (*.xml)|*.xml",
			};
			
			if (dlg.Show(this.Dialogs) && dlg.FileName != null)
			{
				dbReaderWriter.WriteDatabase(eventObjectHandler, dlg.FileName);
			}
		}
		
		/// <summary>
		/// Expose Command to Save Menu Item
		/// </summary>
		public ICommand SaveToDatabaseCommand { get { return new RelayCommand(OnSaveToDatabaseCommand); } }
		private void OnSaveToDatabaseCommand()
		{
			dbReaderWriter.WriteDatabase(eventObjectHandler);
		}
		
		/// <summary>
		/// Expose Command to Exit App Command
		/// </summary>
		public ICommand CloseAllCommand { get { return new RelayCommand(OnCloseAll); } }
		private void OnCloseAll()
		{
			this.Dialogs.Clear();
		}
		
		/// <summary>
		/// Expose Command to switch language command
		/// </summary>
		public ICommand SwitchLanguageToGerman { get { return new RelayCommand(SetGermanLanguage); } }
		private void SetGermanLanguage()
		{
			if (settingsReaderWriter.DefaultLanguage != "german") {
				settingsReaderWriter.DefaultLanguage = "german";
				this.RestartRequiredDialog();
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public ICommand SwitchLanguageToEnglish { get { return new RelayCommand(SetEnglishLanguage); } }
		private void SetEnglishLanguage()
		{
			if (settingsReaderWriter.DefaultLanguage != "english") {
				settingsReaderWriter.DefaultLanguage = "english";
				this.RestartRequiredDialog();
			}
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		public ICommand SwitchAutoLoadDB { get { return new RelayCommand(ToggleAutoLoadDB); } }
		private void ToggleAutoLoadDB()
		{
			if (!settingsReaderWriter.AutoLoadDB)
				settingsReaderWriter.AutoLoadDB = true;
			else
				settingsReaderWriter.AutoLoadDB = false;
			
			RaisePropertyChanged("IsAutoLoadDBActive");
		}
		
		/// <summary>
		/// 
		/// </summary>
		public ICommand SwitchAutoLoadOnStartup { get { return new RelayCommand(ToggleAutoLoadOnStartup); } }
		private void ToggleAutoLoadOnStartup()
		{
			DirectoryInfo assemblyRootDir = new DirectoryInfo(assemblyFileName);
			
			string autoRunCreatorPath = Path.Combine(assemblyRootDir.Parent.FullName,"arc.exe");
			
			if (!settingsReaderWriter.AutoLoadOnStartup)
			{
				settingsReaderWriter.AutoLoadOnStartup = true;
				
				if(File.Exists(autoRunCreatorPath))
				{
					ProcessStartInfo startInfo = new ProcessStartInfo(autoRunCreatorPath);
					startInfo.Verb = "runas";
					startInfo.Arguments = "set";
					Process.Start(startInfo);
				}
			}

			else
			{
				settingsReaderWriter.AutoLoadOnStartup = false;
				
				if(File.Exists(autoRunCreatorPath))
				{
					ProcessStartInfo startInfo = new ProcessStartInfo(autoRunCreatorPath);
					startInfo.Verb = "runas";
					Process.Start(startInfo);
				}
			}

			RaisePropertyChanged("IsAutoLoadOnStartupActive");
		}
		


		/// <summary>
		/// 
		/// </summary>
		public ICommand ShowHelpCommand { get { return new RelayCommand(OnNewShowHelpCommand); }}
		private void OnNewShowHelpCommand()
		{
			var q = from p in Process.GetProcesses() where ContainsAny(p.MainWindowTitle, new string[] {"EventMessenger Help", "EventMessenger Hilfe"}) select p;

			if(!q.Any())
			{
				string pathToHelpFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingsReaderWriter.DefaultLanguage == "german" ? "eventmessenger_de.chm" : "eventmessenger_en.chm");
				if(File.Exists(pathToHelpFile))
				{
					ProcessStartInfo startInfo = new ProcessStartInfo(pathToHelpFile);
					Process.Start(startInfo);
				}

			}

		}
		
		/// <summary>
		/// 
		/// </summary>
		public ICommand NewOpenFileDialogCommand { get { return new RelayCommand(OnNewOpenFileDialog); } }
		private void OnNewOpenFileDialog()
		{
			var dlg = new OpenFileDialogViewModel {
				Title = "Select a file (I won't actually do anything with it)",
				Filter = "All files (*.xml)|*.xml",
				Multiselect = false
			};
			
			if (dlg.Show(this.Dialogs) && dlg.FileName != null)
			{
				if(DataGridSource != null && DataGridSource.Count > 0)
					DataGridSource.Clear();
				
				dbReaderWriter.ReadDatabase(dlg.FileName);
				
				RestoreFromDB();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ICommand CloseApplication { get { return new RelayCommand(OnCloseRequest); } }
		private void OnCloseRequest()
		{
			Environment.Exit(0);
		}
		
		#endregion
		
		#region Dependency Properties
		
		/// <summary>
		/// 
		/// </summary>
		public ResponseObjectModel SelectedResponse {
			get { return selectedResponse; }
			set {
				selectedResponse = value;
				RaisePropertyChanged("SelectedResponse");
			}
		} private ResponseObjectModel selectedResponse;
		
		/// <summary>
		/// 
		/// </summary>
		public EventObjectModel SelectedDataGridItem {
			get { return selectedDataGridItem; }
			set {selectedDataGridItem = value;
				RaisePropertyChanged("SelectedDataGridItem");
				RaisePropertyChanged("ContextMenu");
			}
		} private EventObjectModel selectedDataGridItem;
		
		/// <summary>
		/// 
		/// </summary>
		public BitmapImage MainWindowIcon //../resources/envelopeopensmall.ico
		{
			get { return mainWindowIcon; }
			set { mainWindowIcon = value; }
		} private BitmapImage mainWindowIcon;
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsSelected {
			get; set;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string ToolTipVisibility
		{
			get { return toolTipVisibility; }
			set {
				toolTipVisibility = value;
				RaisePropertyChanged("ToolTipVisibility");
			}
		} private string toolTipVisibility;
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsCheckForUpdatesChecked {
			get { return settingsReaderWriter != null ? settingsReaderWriter.AutoCheckForUpdates : false; }
			set {
				if (value)
					updater.StartMonitoring();

				else
					updater.StopMonitoring();
				
				
				settingsReaderWriter.AutoCheckForUpdates = value;
				RaisePropertyChanged("IsCheckForUpdatesChecked");

			}
		}


		/// <summary>
		/// 
		/// </summary>
		public bool IsDebugWindowVisible {
			get { return settingsReaderWriter != null ? settingsReaderWriter.HasDebugConsole : false; }
			set {
				settingsReaderWriter.HasDebugConsole = value;
				RestartRequiredDialog();
				RaisePropertyChanged("IsDebugWindowVisible");

			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsAutoLoadDBActive {
			get { return settingsReaderWriter != null ? settingsReaderWriter.AutoLoadDB : false; }
			set {
				settingsReaderWriter.AutoLoadDB = value;
				RaisePropertyChanged("IsAutoLoadDBActive");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsAutoLoadOnStartupActive {
			get { return settingsReaderWriter != null ? settingsReaderWriter.AutoLoadOnStartup : false; }
			set {
				settingsReaderWriter.AutoLoadOnStartup = value;
				RaisePropertyChanged("IsAutoLoadOnStartupActive");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool RadioButtonGermanLanguageSelectedState {
			get { return settingsReaderWriter != null ? (settingsReaderWriter.DefaultLanguage == "german" ? true : false) : false; }
			set {
				if (settingsReaderWriter.DefaultLanguage == "english")
					value = false;
				RaisePropertyChanged("RadioButtonGermanLanguageSelectedState");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public bool RadioButtonEnglishLanguageSelectedState {
			get { return settingsReaderWriter != null ? (settingsReaderWriter.DefaultLanguage != "german" ? true : false) : true;	}
			set {
				if (settingsReaderWriter.DefaultLanguage == "german")
					value = false;
				RaisePropertyChanged("RadioButtonEnglishLanguageSelectedState");
			}
		}
		

		#endregion
		
		#region Extensions
		
		//Only one instance is allowed due to the pipeserver listening for event_cmd.exe
		private void RunMutex(object sender, StartupEventArgs e)
		{
			bool aIsNewInstance = false;
			mutex = new Mutex(true, "App", out aIsNewInstance);
			
			if(!aIsNewInstance)
			{
				Environment.Exit(0);
			}
			
		}

		private void RestartRequiredDialog()
		{
			this.Dialogs.Add(new CustomDialogViewModel {
			                 	Message = resLoader.getResource("messageBoxRestartRequiredMessage"),
			                 	Caption = resLoader.getResource("messageBoxRestartRequiredCaption"),

			                 	OnOk = (sender) => {
			                 		sender.Close();
			                 		App.Current.Shutdown();
			                 	},

			                 	OnCancel = (sender) => {
			                 		sender.Close();

			                 	},

			                 	OnCloseRequest = (sender) => {
			                 		sender.Close();
			                 	}
			                 });
		}

		private void RestoreMainWindow(object sender, RoutedEventArgs e)
		{
			mw.Show();
		}
		
		private void CloseThreads(object sender, CancelEventArgs e)
		{
			if (new MessageBoxViewModel
			    {
			    	Caption = resLoader.getResource("messageBoxAskCloseOrTrayCaption"),
			    	Message = resLoader.getResource("messageBoxAskCloseOrTrayMessage"),
			    	Buttons = MessageBoxButton.YesNo,
			    	Image = MessageBoxImage.Question
			    }.Show(this.Dialogs) == MessageBoxResult.Yes)
			{
				tb.Visibility=Visibility.Visible;
				tb.ShowBalloonTip(resLoader.getResource("taskBarIconBalloonHeaderStillHere"), resLoader.getResource("taskBarIconBalloonMessageStillHere"), BalloonIcon.Info);
				e.Cancel = true;
				mw.Hide();
			}

			else
			{
				pipeServer.StopAllWorkers();
				Environment.Exit(0);
			}
		}
		
		private void LoadCompleted(object sender, EventArgs e) {
			
			mw = (MainWindow) Application.Current.MainWindow;
			mw.Title= string.Format("EventMessenger {0}.{1}.{2} {3}", Version.Major, Version.Minor, Version.Build , Version.Major == 0 ? "beta" : "");

			if(firstRun)
			{
				firstRun = false;
				eventObjectHandler.Register();
				
				if(settingsReaderWriter.AutoCheckForUpdates)
					updater.StartMonitoring();
				
				if(runSilent)
					mw.Hide();
				
				if(IsAutoLoadDBActive && !updater.isUserNotified)
				{
					if(!dbReaderWriter.ReadDatabase())
					{
						//TODO: decide if asking user before restoring or just restore without bothering
//						if (new MessageBoxViewModel
//						    {
//						    	Caption = resLoader.getResource("messageBoxLoadDefaultDatabaseCaption"),
//						    	Message = resLoader.getResource("messageBoxLoadDefaultDatabaseMessage"),
//						    	Buttons = MessageBoxButton.YesNo,
//						    	Image = MessageBoxImage.Question
//						    }.Show(this.Dialogs) == MessageBoxResult.Yes)
//						{
//							RestoreFromDB();
//						}
//
//						else
//						{
//							dbReaderWriter.eventObjects.Clear();
//						}
						RestoreFromDB();
					}

				}
			}

		}
		
		private void RestoreFromDB()
		{
			EventObjectModel tempModel = new EventObjectModel();
			
			foreach(EventObjectModel eo in dbReaderWriter.eventObjects)
			{
				foreach(ResponseObjectModel rom in eo.ResponseCollection)
				{
					if(rom.ResponseObject is TelegramMessageObjectModel)
					{
						var botListQuery = DataGridSource.SelectMany(
							u => u.ResponseCollection.Where(
								v => v.ResponseObject is TelegramMessageObjectModel).Select(
								w => w.ResponseObject as TelegramMessageObjectModel).Select(x => x.Bot2Use).ToList()
						).ToList();

						var selectedBotQuery = DataGridSource.SelectMany(
							u => u.ResponseCollection.Where(
								v => v.ResponseObject is TelegramMessageObjectModel).Select(
								w => w.ResponseObject as TelegramMessageObjectModel).Select(
								x => x.Bot2Use)
						).ToList();
						
						if((rom.ResponseObject is TelegramMessageObjectModel) &&
						   !(botListQuery.Select(y => y.Token).Contains((rom.ResponseObject as TelegramMessageObjectModel).Bot2Use.Token)))
						{
							try{
								(rom.ResponseObject as TelegramMessageObjectModel).Bot2Use.CreateBot();
							}
							catch(Exception e)
							{
								if(e.InnerException.GetType().ToString() == "System.Net.Http.HttpRequestException")
								{
									LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
									
									this.Dialogs.Add(new CustomDialogViewModel {
									                 	Message = e.InnerException.InnerException.Message,
									                 	Caption = "Error",

									                 	OnOk = (sender2) => {
									                 		sender2.Close();
									                 	},

									                 	OnCancel = (sender2) => {
									                 		sender2.Close();
									                 	},

									                 	OnCloseRequest = (sender2) => {
									                 		sender2.Close();
									                 	}
									                 });
								}
							}
						}
						
						else if(botListQuery.Select(y => y.Token).Contains((rom.ResponseObject as TelegramMessageObjectModel).Bot2Use.Token))
						{
							(rom.ResponseObject as TelegramMessageObjectModel).Bot2Use = selectedBotQuery.Where(z => z.Token == (rom.ResponseObject as TelegramMessageObjectModel).Bot2Use.Token).FirstOrDefault();
						}
					}
					
					else if (rom.ResponseObject is EMailResponseObjectModel)
					{

					}

				}
				
				DataGridSource.Add(eo);
				SelectedDataGridItem = eo;
				SelectedResponse = SelectedDataGridItem.ResponseCollection.FirstOrDefault();
			}
		}
		
		private void AskForUpdateNow(object sender, EventArgs e)
		{
			if (new MessageBoxViewModel
			    {
			    	Caption = resLoader.getResource("messageBoxUpdateAvailableCaption"),
			    	Message = resLoader.getResource("messageBoxUpdateAvailableMessage"),
			    	Buttons = MessageBoxButton.YesNo,
			    	Image = MessageBoxImage.Question
			    }.Show(this.Dialogs) == MessageBoxResult.Yes)
				(sender as Updater).allowUpdate = true;
			else
			{
				(sender as Updater).allowUpdate = false;
			}

		}
		
		private static bool ContainsAny(string haystack, params string[] needles) { return needles.Any(haystack.Contains); }
		
		#endregion
	}
}


/*
		public ICommand NewModalDialogCommand { get { return new RelayCommand(OnNewModalDialog); } }
		public void OnNewModalDialog()
		{
			this.Dialogs.Add(new CustomDialogViewModel {
			                 	Message = "Hello World!",
			                 	Caption = "Modal Dialog Box",

			                 	OnOk = (sender) => {
			                 		sender.Close();
			                 		new MessageBoxViewModel("You pressed ok!", "Bye bye!").Show(this.Dialogs);
			                 	},

			                 	OnCancel = (sender) => {
			                 		sender.Close();
			                 		new MessageBoxViewModel("You pressed cancel!", "Bye bye!").Show(this.Dialogs);
			                 	},

			                 	OnCloseRequest = (sender) => {
			                 		sender.Close();
			                 		new MessageBoxViewModel("You clicked the caption bar close button!", "Bye bye!").Show(this.Dialogs);
			                 	}
			                 });
		}

 */

/*
		public ICommand NewModelessDialogCommand { get { return new RelayCommand(OnNewModelessDialog); } }
		public void OnNewModelessDialog()
		{
			var confirmClose = new Action<CustomDialogViewModel>((sender) => {
			                                                     	if (new MessageBoxViewModel {
			                                                     	    	Caption = "Closing",
			                                                     	    	Message = "Are you sure you want to close this window?",
			                                                     	    	Buttons = MessageBoxButton.YesNo,
			                                                     	    	Image = MessageBoxImage.Question
			                                                     	    }
			                                                     	    .Show(this.Dialogs) == MessageBoxResult.Yes)
			                                                     		sender.Close();
			                                                     });

			new CustomDialogViewModel(false) {
				Message = "Hello World!",
				Caption = "Modeless Dialog Box",
				OnOk = confirmClose,
				OnCancel = confirmClose,
				OnCloseRequest = confirmClose
			}.Show(this.Dialogs);
		}

 */

/*
		public ICommand NewMessageBoxCommand { get { return new RelayCommand(OnNewMessageBox); } }
		public void OnNewMessageBox()
		{
			new MessageBoxViewModel {
				Caption = "Message Box",
				Message = "This is a message box!",
				Image = MessageBoxImage.Information
			}.Show(this.Dialogs);
		}
 */