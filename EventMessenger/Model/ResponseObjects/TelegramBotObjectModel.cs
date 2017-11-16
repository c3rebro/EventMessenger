/*
 * Created by SharpDevelop.
 * Date: 31.07.2017
 * Time: 21:24
 * 
 */
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Xml.Serialization;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;

namespace EventMessenger.Model.ResponseObjects
{
	/// <summary>
	/// Description of TelegramMessageObjectModel.
	/// </summary>
	[XmlRootAttribute("TelegramBotModel", IsNullable = false)]
	public class TelegramBotObjectModel
	{

		private TelegramBotClient Bot;
		private ResourceLoader resMan;
		
		/// <summary>
		/// 
		/// </summary>
		public TelegramBotObjectModel()
		{
			resMan = new ResourceLoader();
			TelegramUsers = new ObservableCollection<TelegramUser>();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_token"></param>
		public TelegramBotObjectModel(string _token)
		{
			resMan = new ResourceLoader();
			TelegramUsers = new ObservableCollection<TelegramUser>();
			
			try {
				
				Token = _token;
				
				Bot = new TelegramBotClient(token);
				
			}
			
			catch(Exception e)
			{
				LogWriter.CreateLogEntry(string.Format("{0};{1};{2}", DateTime.Now, e.Message, e.InnerException != null ? e.InnerException.Message : ""));
				
				throw e;
			}
			
			Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
			Bot.OnMessage += BotOnMessageReceived;
			Bot.OnMessageEdited += BotOnMessageReceived;
			Bot.OnInlineQuery += BotOnInlineQueryReceived;
			Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
			Bot.OnReceiveError += BotOnReceiveError;
		}

		/// <summary>
		/// 
		/// </summary>
		public string messageToSend;
		
		/// <summary>
		/// 
		/// </summary>
		public string Token
		{
			get { return token; }
			set { token = value; }
		} private string token;

		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<TelegramUser> TelegramUsers { get; set;}
		


		/// <summary>
		/// 
		/// </summary>
		public string BotName
		{
			get { return botName; }
			set { botName = value; }
		} private string botName;
		
		/// <summary>
		/// 
		/// </summary>
		public bool IsReceiving
		{
			get { return Bot.IsReceiving; }
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public User CreateBot()
		{
			try {
				
				User me = null;
				
				if(Bot == null)
				{
					Bot = new TelegramBotClient(Token);

					
					Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
					Bot.OnMessage += BotOnMessageReceived;
					Bot.OnMessageEdited += BotOnMessageReceived;
					Bot.OnInlineQuery += BotOnInlineQueryReceived;
					Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
					Bot.OnReceiveError += BotOnReceiveError;
				}
				
				me = Bot.GetMeAsync().Result;
				#if DEBUG
				Trace.WriteLine(me.Username);
				#endif
				Bot.StartReceiving();
				
				
				if (me != null)
				{
					BotName = me.Username;
					return me;
				}

				return new User();
				
			}
			
			catch(Exception e)
			{
				LogWriter.CreateLogEntry(string.Format("{0};{1};{2}", DateTime.Now, e.Message, e.InnerException != null ? e.InnerException.InnerException.Message : ""));
				
				throw e;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public User GetBot()
		{
			return Bot != null ? Bot.GetMeAsync().Result : new User();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public void StopBot()
		{
			if(Bot != null)
				Bot.StopReceiving();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public void StartBot()
		{
			if(Bot != null)
				Bot.StartReceiving();
		}
		
		private void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
		{
			LogWriter.CreateLogEntry(string.Format("error: {0}; {1}",
			                                       receiveErrorEventArgs.ApiRequestException.Message,
			                                       receiveErrorEventArgs.ApiRequestException.InnerException));
		}

		private void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
		{
			Trace.WriteLine("Received choosen inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
		}

		private async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
		{
			InlineQueryResult[] results = {
				new InlineQueryResultLocation
				{
					Id = "1",
					Latitude = 40.7058316f, // displayed result
					Longitude = -74.2581888f,
					Title = "New York",
					InputMessageContent = new InputLocationMessageContent // message if result is selected
					{
						Latitude = 40.7058316f,
						Longitude = -74.2581888f,
					}
				},

				new InlineQueryResultLocation
				{
					Id = "2",
					Longitude = 52.507629f, // displayed result
					Latitude = 13.1449577f,
					Title = "Berlin",
					InputMessageContent = new InputLocationMessageContent // message if result is selected
					{
						Longitude = 52.507629f,
						Latitude = 13.1449577f
					}
				}
			};

			await Bot.AnswerInlineQueryAsync(inlineQueryEventArgs.InlineQuery.Id, results, isPersonal: true, cacheTime: 0);
		}
		
		private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
		{
			var message = messageEventArgs.Message;
			
			if(TelegramUsers.Count == 0)
			{
				TelegramUsers.Add(new TelegramUser(TelegramUserStatus.UserIsAdmin, message.Chat.Id));
				
				await Bot.SendTextMessageAsync(TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Select(x => x.chatID).Single()
				                               , string.Format(resMan.getResource("telegramBotMessageWelcomeAdmin"), message.Chat.Id + ";" + message.Chat.FirstName));
			}

			else if(TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Select(x => x.chatID == message.Chat.Id).Single())
			{
				await Bot.SendTextMessageAsync(TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Select(x => x.chatID).Single()
				                               , string.Format(resMan.getResource("telegramBotMessageWelcomeBackAdmin"), message.Chat.Id + ";" + message.Chat.FirstName));

			}
			
			else if(!
			        (TelegramUsers.Contains(new TelegramUser(TelegramUserStatus.UserIsNotRegistered, message.Chat.Id))
			         || TelegramUsers.Contains(new TelegramUser(TelegramUserStatus.UserIsRegistered, message.Chat.Id))
			         || TelegramUsers.Contains(new TelegramUser(TelegramUserStatus.UserIsAdmin, message.Chat.Id))))
			{
				TelegramUsers.Add(new TelegramUser(TelegramUserStatus.UserIsNotRegistered, message.Chat.Id));
                
				var keyboard = new InlineKeyboardMarkup(new[]
				                                        {
				                                        	new[] // first row
				                                        	{
				                                        		new InlineKeyboardButton(resMan.getResource("telegramBotButtonAddUser"),"userAdd"),
				                                        		new InlineKeyboardButton(resMan.getResource("telegramBotButtonBlockUser"),"userBlock"),
				                                        	}
				                                        });

				await Bot.SendTextMessageAsync(
					TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Select(x => x.chatID).Single()
					, string.Format(resMan.getResource("telegramBotMessageNewUserFound"), message.Chat.Id + ";" + message.Chat.FirstName),
					replyMarkup: keyboard);
				
				await Bot.SendTextMessageAsync(
					TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsNotRegistered).Select(x => x.chatID).Single(),
					resMan.getResource("telegramBotMessageWaitForAdministrator"));
				
			}
			else
			{
				
			}

			if (message == null || message.Type != MessageType.TextMessage) return;

			if (message.Text.StartsWith("/inline")) // send inline keyboard
			{
				await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

				var keyboard = new InlineKeyboardMarkup(new[]
				                                        {
				                                        	new[] // first row
				                                        	{
				                                        		new InlineKeyboardButton("1.1"),
				                                        		new InlineKeyboardButton("1.2"),
				                                        	},
				                                        	new[] // second row
				                                        	{
				                                        		new InlineKeyboardButton("2.1"),
				                                        		new InlineKeyboardButton("2.2"),
				                                        	}
				                                        });

				await Task.Delay(500); // simulate longer running task

				await Bot.SendTextMessageAsync(message.Chat.Id, "Choose",
				                               replyMarkup: keyboard);
			}
			else if (message.Text.StartsWith("/keyboard")) // send custom keyboard
			{
				var keyboard = new ReplyKeyboardMarkup(new[]
				                                       {
				                                       	new [] // first row
				                                       	{
				                                       		new KeyboardButton("1.1"),
				                                       		new KeyboardButton("1.2"),
				                                       	},
				                                       	new [] // last row
				                                       	{
				                                       		new KeyboardButton("2.1"),
				                                       		new KeyboardButton("2.2"),
				                                       	}
				                                       });

				await Bot.SendTextMessageAsync(message.Chat.Id, "Choose",
				                               replyMarkup: keyboard);
			}
			else if (message.Text.StartsWith("/photo")) // send a photo
			{
				await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

				const string file = @"<FilePath>";

				var fileName = file.Split('\\').Last();

				using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var fts = new FileToSend(fileName, fileStream);

					await Bot.SendPhotoAsync(message.Chat.Id, fts, "Nice Picture");
				}
			}
			else if (message.Text.StartsWith("/request")) // request location or contact
			{
				var keyboard = new ReplyKeyboardMarkup(new []
				                                       {
				                                       	new KeyboardButton("Location")
				                                       	{
				                                       		RequestLocation = true
				                                       	},
				                                       	new KeyboardButton("Contact")
				                                       	{
				                                       		RequestContact = true
				                                       	},
				                                       });

				await Bot.SendTextMessageAsync(message.Chat.Id, "Who or Where are you?", replyMarkup: keyboard);
			}
			else
			{

			}
		}

		private async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
		{
			
			switch(callbackQueryEventArgs.CallbackQuery.Data)
			{
				case "userAdd":

					await Bot.AnswerCallbackQueryAsync(
						callbackQueryEventArgs.CallbackQuery.Id,
						string.Format("Received {0}",callbackQueryEventArgs.CallbackQuery.Data)
					);
					
					long offlineUserID = TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsNotRegistered).Single().chatID;
					
					TelegramUsers.Remove(TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsNotRegistered).Single());
					
					TelegramUsers.Add(new TelegramUser(TelegramUserStatus.UserIsRegistered, offlineUserID));
					
					break;
			}
		}
		
		public async void SendTextMessageToChat(string message)
		{
			await Bot.SendTextMessageAsync(TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Single().chatID, message);
			
			foreach(TelegramUser tUser in TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsRegistered))
			{
				await Bot.SendTextMessageAsync(tUser.chatID, message);
			}
			
		}
		
		public async void SendTextMessageToChat(long _chatID, string message)
		{
			await Bot.SendTextMessageAsync(_chatID, message);
		}
	}
}
