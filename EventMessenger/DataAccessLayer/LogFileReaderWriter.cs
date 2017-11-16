/*
 * Created by SharpDevelop.
 * Date: 02/15/2017
 * Time: 00:35
 * 
 */
using GalaSoft.MvvmLight.Messaging;

using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace EventMessenger
{
	/// <summary>
	/// Description of LogFileReaderWriter.
	/// </summary>
	public class LogFileReaderWriter
	{
		BackgroundWorker bw = new BackgroundWorker();
		
		private readonly string logFilePath;
		private string[][] allParams;
		
		public LogFileReaderWriter(int interval, bool start)
		{
			AutoResetEvent autoEvent = new AutoResetEvent(start);

			// Create the delegate that invokes bw.RunWorkerAsync
			// and create a timer that signals the delegate to invoke
			// bw.RunWorkerAsync after one second, and every 10 seconds
			// thereafter. In DebugMode only raised once
			

			TimerCallback timerDelegate = new TimerCallback(bw.RunWorkerAsync);
			
			System.Threading.Timer stateTimer =
				new System.Threading.Timer(timerDelegate, autoEvent, interval, 10000);
			
//			TimerCallback timerDelegate =
//				new TimerCallback(this.CallMethod);
//
//			Timer stateTimer =
//				new Timer(timerDelegate, autoEvent, interval, Timeout.Infinite);

			bw.RunWorkerAsync();
			logFilePath = Path.GetFullPath("C:\\ProgramData\\SimonsVoss\\LockSysMgr\\config\\event_cmd_exe.txt");
			
			bw.DoWork += new DoWorkEventHandler(ParseLogFile);
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CreateViewModel);
			
		}
		
		#if DEBUG
		private void CallMethod(object stateInfo)
		{
			ParseLogFile(this,null);
		}
		#endif
		
		private void ParseLogFile(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			
			try{
				
				StreamReader sr = new StreamReader(logFilePath, Encoding.Default);
				
				string[] allLines = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
				
				sr.Close();
				
				/* delete old logfile immediatly after 'logfile full' is detected and create a backup of the old log and append everything to a zip file
				 * ensure that lsm can start to create a new logfile */
				if(allLines.Length > 200)
				{
					using (FileStream zipToOpen = new FileStream(Path.Combine(Path.GetDirectoryName(logFilePath),"event_cmd_exe_log_backup.zip"), FileMode.OpenOrCreate))
					{
						using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
						{
							ZipArchiveEntry logEntry = archive.CreateEntry("event_cmd_exe_backup.txt");
							using (StreamWriter writer = new StreamWriter(logEntry.Open()))
							{
								foreach(string line in allLines)
									writer.Write(line);
								writer.Close();
							}
						}
					}
					
					#if !DEBUG
					File.Delete(logFilePath);
					#endif
				}
				
				allParams = new string[allLines.Length][];
				
				for(int lineIterator = 0; lineIterator < allLines.Length / 2; lineIterator++)
				{
					for(int paramInterator = 0; paramInterator<allLines[lineIterator].Length; paramInterator+=2){
						allParams[lineIterator] = allLines[paramInterator].Split(';');
					}
				}
				e.Result = allParams;

			}
			catch (Exception er)
			{
				Console.WriteLine(er.Message);
			}
			


		}
		
		private void CreateViewModel(object sender, RunWorkerCompletedEventArgs e)
		{
			
			allParams = e.Result as string[][];
			Messenger.Default.Send<NotificationMessage<string>>(
				new NotificationMessage<string>(allParams, "LogFileReader", "Arguments"));
		}
	}
}
