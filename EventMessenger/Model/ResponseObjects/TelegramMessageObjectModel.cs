/*
 * Created by SharpDevelop.
 * User: Werkstatt
 * Date: 03.03.2017
 * Time: 07:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Xml.Serialization;

namespace EventMessenger.Model.ResponseObjects
{
	
	
	/// <summary>
	/// Description of TelegramMessageObjectModel.
	/// </summary>
	[XmlRootAttribute("TelegramMassageObjectModel", IsNullable = false)]
	public class TelegramMessageObjectModel
	{
		
		/// <summary>
		/// 
		/// </summary>
		public TelegramMessageObjectModel()
		{
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_bot"></param>
		public TelegramMessageObjectModel(TelegramBotObjectModel _bot)
		{
			Bot2Use = _bot;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="_token"></param>
		/// <param name="_chatID"></param>
		public TelegramMessageObjectModel(string _token, long _chatID)
		{

		}
		
		/// <summary>
		/// 
		/// </summary>
		public TelegramBotObjectModel Bot2Use
		{
			get { return bot; }
			set { bot = value; }
		}
		private TelegramBotObjectModel bot;
		
		/// <summary>
		/// 
		/// </summary>
		public string MessageToSend
		{
			get { return messageToSend; }
			set { messageToSend = value; }
		}
		private string messageToSend;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="_bot"></param>
		public async void SendTextMessageToChat(string message, TelegramBotObjectModel _bot)
		{
			_bot.SendTextMessageToChat(message);
		}
		
	}
}
