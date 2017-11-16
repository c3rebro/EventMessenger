/*
 * Created by SharpDevelop.
 * Date: 17.02.2017
 * Time: 15:33
 * 
 */
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using System;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Text;

using System.Runtime.InteropServices;

namespace EventMessenger
{
	public class PipeServer
	{
		private bool showDemoConsole;
		private SettingsReaderWriter settingsReader = new SettingsReaderWriter();
		
		[DllImport("Kernel32")]
		public static extern void AllocConsole();

		[DllImport("Kernel32")]
		public static extern void FreeConsole();

		BackgroundWorker bw = new BackgroundWorker();
		
		public PipeServer()
		{
			settingsReader.ReadSettings();
			
			bw.DoWork += new DoWorkEventHandler(ServerStart);
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ServerFinished);
			
			bw.WorkerSupportsCancellation = true;
			bw.RunWorkerAsync();
		}

		public PipeServer(bool _showDemoConsole)
		{
			showDemoConsole = _showDemoConsole;
			
			bw.DoWork += new DoWorkEventHandler(ServerStart);
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ServerFinished);
			
			bw.WorkerSupportsCancellation = true;
			bw.RunWorkerAsync();
		}
		
		private void ServerStart(object sender, DoWorkEventArgs arg)
		{
			try{
				
				if(settingsReader.HasDebugConsole)
				{
					AllocConsole();
					
					ShowConsole();
				}
				
				System.Diagnostics.Trace.WriteLine("Waiting for client connect...\n");
				
				string tempCmdLineArgs = "Error:";
				
				NamedPipeServerStream pipeServer =
					new NamedPipeServerStream(@"\\.\EA2pipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.None);

				// Wait for a client to connect
				pipeServer.WaitForConnection();
				
				System.Diagnostics.Trace.WriteLine(string.Format("Client connected."));
				try
				{
					// Read the request from the client. Once the client has
					// written to the pipe its security token will be available.

					StreamString ss = new StreamString(pipeServer);

					// Verify our identity to the connected client using a
					// string that the client anticipates.

					ss.WriteString("6jEBb4uZ2yLmx39lDESGrhucp6WTUVJKbVdiqySOr4tanvDgGHrBM8OFEFKjDkHmLgTDqYyl0R57RIX0Y61HpjtTJhalzubI4UZhF07HydUckfxGrN5HOk2oLeH62SQj" + '\0');
					arg.Result = ss.ReadString();
					tempCmdLineArgs = arg.Result as string;

					// Display the name of the user we are impersonating.
					System.Diagnostics.Trace.WriteLine(string.Format("Recieved String: {0} as user: {1}.",
					                                                 tempCmdLineArgs, pipeServer.GetImpersonationUserName()));
				}
				// Catch the IOException that is raised if the pipe is broken
				// or disconnected.
				catch (IOException e)
				{
					LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
					Environment.Exit(0);
				}
				pipeServer.Close();
				
			}
			
			catch(Exception e)
			{
				LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
				Environment.Exit(0);
			}
		}
		
		private async void ShowConsole()
		{
			while(true)
			{
				Console.WriteLine("DEBUG Console:\n\nSelect one of the following Options to send a DEMO Response:\n"+
				                  "NOTE: \nDemo Transponder will always be: \"Vorname Nachname\" with SerialNumber \"01UHE7N\"\n"+
				                  "Demo Lock will always be: \"Tuerbezeichnung\" with Serialnumber \"009P7BL\"\n"+
				                  "WaveNet Address will always be: \"0x0026\"\n\n"+
				                  "1 = Unlocking Event: Access Granted\n"+
				                  "2 = Unlocking Event: Access Denied\n\n"+
				                  "3 = Input Event: Input 1 Rising Edge\n"+
				                  "4 = Input Event: Input 1 Falling Edge\n"+
				                  "5 = Input Event: Input 2 Rising Edge\n"+
				                  "6 = Input Event: Input 2 Falling Edge\n"+
				                  "7 = Input Event: Input 3 Rising Edge\n"+
				                  "8 = Input Event: Input 3 Falling Edge\n\n"+
				                  "9 = Door Monitoring Event - Door Opened\n"+
				                  "10 = Door Monitoring Event - Door Closed\n"+
				                  "11 = Door Monitoring Event - Door Stays Open Too Long\n"+
				                  "12 = Door Monitoring Event - Door Locked\n"+
				                  "13 = Door Monitoring Event - Door Secured\n");
				
				string consoleInput = Console.ReadLine();
				string tempInput;
				
				switch(consoleInput)
				{
						
					case "1":
						tempInput = "event_cmd.exe 1;"+
							"9;"+
							"2017.02.20-19:11:30;"+
							"0x0000;"+
							"009P7BL;"+
							"9229;"+
							"3200;"+
							"0;"+
							"\"Event Test\";"+
							"\"Tuerbezeichnung / 009P7BL\";"+
							"\"Nachname Vorname / 01UHE7N\";\n\n";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "2":
						tempInput = "event_cmd.exe 1;"+
							"9;"+
							"2017.02.20-19:11:30;"+
							"0x0000;"+
							"009P7BL;"+
							"9229;"+
							"3200;"+
							"1;"+
							"\"Event Test\";"+
							"\"Tuerbezeichnung / 009P7BL\";"+
							"\"Nachname Vorname / 01UHE7N\";\n\n";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "3":
						tempInput = "event_cmd.exe 1;"+
							"1;"+
							"2017.03.17-20:55:15;"+
							"0xffff0026;"+
							"0000000;"+
							"1;"+
							"0;"+
							"0;"+
							"\"\";"+
							"\"\";"+
							"\"\";";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "4":
						tempInput = "event_cmd.exe 1;"+
							"1;"+
							"2017.03.17-20:55:15;"+
							"0xffff0026;"+
							"0000000;"+
							"2;"+
							"0;"+
							"0;"+
							"\"\";"+
							"\"\";"+
							"\"\";";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "5":
						tempInput = "event_cmd.exe 1;"+
							"1;"+
							"2017.03.17-20:55:15;"+
							"0xffff0026;"+
							"0000000;"+
							"0;"+
							"1;"+
							"0;"+
							"\"\";"+
							"\"\";"+
							"\"\";";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "6":
						tempInput = "event_cmd.exe 1;"+
							"1;"+
							"2017.03.17-20:55:15;"+
							"0xffff0026;"+
							"0000000;"+
							"0;"+
							"2;"+
							"0;"+
							"\"\";"+
							"\"\";"+
							"\"\";";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "7":
						tempInput = "event_cmd.exe 1;"+
							"1;"+
							"2017.03.17-20:55:15;"+
							"0xffff0026;"+
							"0000000;"+
							"0;"+
							"0;"+
							"1;"+
							"\"\";"+
							"\"\";"+
							"\"\";";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "8":
						tempInput = "event_cmd.exe 1;"+
							"1;"+
							"2017.03.17-20:55:15;"+
							"0xffff0026;"+
							"0000000;"+
							"0;"+
							"0;"+
							"2;"+
							"\"\";"+
							"\"\";"+
							"\"\";";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "9":
						tempInput = "event_cmd.exe 1;"+
							"2;"+
							"2017.03.19-14:02:16;"+
							"0x0000;"+
							"0000041;"+
							"0;"+
							"0;"+
							"0;"+
							"\"Event Test\";"+
							"\"Tuerbezeichnung / 009P7BL\";"+
							"\"\";";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "10":
						tempInput = "event_cmd.exe 1;"+
							"4;"+
							"2017.03.19-14:02:16;"+
							"0x0000;"+
							"0000041;"+
							"0;"+
							"0;"+
							"0;"+
							"\"Event Test\";"+
							"\"Tuerbezeichnung / 009P7BL\";"+
							"\"\";";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "11":
						tempInput = "event_cmd.exe 1;"+
							"3;"+
							"2017.03.19-14:02:16;"+
							"0x0000;"+
							"0000041;"+
							"0;"+
							"0;"+
							"0;"+
							"\"Event Test\";"+
							"\"Tuerbezeichnung / 009P7BL\";"+
							"\"\";";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "12":
						tempInput = "event_cmd.exe 1;"+
							"5;"+
							"2017.03.19-14:02:16;"+
							"0x0000;"+
							"0000041;"+
							"0;"+
							"0;"+
							"0;"+
							"\"Event Test\";"+
							"\"Tuerbezeichnung / 009P7BL\";"+
							"\"\";";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "13":
						tempInput = "event_cmd.exe 1;"+
							"6;"+
							"2017.03.19-14:02:16;"+
							"0x0000;"+
							"0000041;"+
							"0;"+
							"0;"+
							"0;"+
							"\"Event Test\";"+
							"\"Tuerbezeichnung / 009P7BL\";"+
							"\"\";";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
						
					case "G1" :
					case "g1" :
						tempInput =
							"event_cmd.exe 1;"+
							"9;"+
							"2017.10.07-21:13:39;"+
							"0x0000;"+
							"00FR7LP;"+
							"15507;"+
							"20;"+
							"0;"+
							"\"Eichenstraße 21\";"+
							"\"Haustür_00020 / 00FR7LP\";"+
							"\" / \";\n\n";
						
						Console.WriteLine(tempInput);
						Messenger.Default.Send<NotificationMessage<string>>(
							new NotificationMessage<string>(this, "PipeServerMessage", tempInput as string));
						
						break;
				}
			}
		}
		
		private void ServerFinished(object sender, RunWorkerCompletedEventArgs e)
		{
			bw.RunWorkerAsync();
			
			Messenger.Default.Send<NotificationMessage<string>>(
				new NotificationMessage<string>(this, "PipeServerMessage", e.Result as string)
			);
		}
		
		public void StopAllWorkers()
		{
			bw.CancelAsync();
			bw.Dispose();
		}
	}

	// Defines the data protocol for reading and writing strings on our stream
	public class StreamString
	{
		private Stream ioStream;
		private Encoding streamEncoding;

		public StreamString(Stream ioStream)
		{
			this.ioStream = ioStream;
			streamEncoding = new UnicodeEncoding();
		}

		public string ReadString()
		{
			try {
				int len = 0, unicodeDummy;
				len = ioStream.ReadByte();
				unicodeDummy = ioStream.ReadByte();
				len += (256 * (int)ioStream.ReadByte());
				unicodeDummy = ioStream.ReadByte();
				
				byte[] inBuffer = new byte[len];
				ioStream.Read(inBuffer, 0, len);

				return streamEncoding.GetString(inBuffer);
			}
			catch (Exception)
			{
				return "Error";
			}

		}

		public int WriteString(string outString)
		{
			try{
				byte[] outBuffer = streamEncoding.GetBytes(outString);
				int len = outBuffer.Length;
				if (len > UInt16.MaxValue)
				{
					len = (int)UInt16.MaxValue;
				}
				ioStream.WriteByte((byte)(len / 256));
				ioStream.WriteByte((byte)(len & 255));
				ioStream.Write(outBuffer, 0, len);
				//ioStream.Flush();

				return outBuffer.Length;
			}
			catch (Exception)
			{
				return 1;
			}

		}
	}
}
