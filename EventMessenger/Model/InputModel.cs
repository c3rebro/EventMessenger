/*
 * Created by SharpDevelop.
 * Date: 20.03.2017
 * Time: 15:11
 * 
 */
using System;

namespace EventMessenger.Model
{
	/// <summary>
	/// Description of InputModel.
	/// </summary>
	public class InputModel
	{
		/// <summary>
		/// 
		/// </summary>
		public InputModel()
		{
			
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="addr"></param>
		public InputModel(string name, string type, string addr)
		{
			inputName = name;
			inputTriggerType = type;
			address = addr;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="im"></param>
		public InputModel(InputModel im)
		{
			inputName = im.InputName;
			inputTriggerType = im.InputTriggerType;
			address = im.Address;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string InputName
		{
			get { return inputName; }
			set { inputName = value; }
		} private string inputName;
		
		/// <summary>
		/// 
		/// </summary>
		public string Address
		{
			get { return address; }
			set { address = value; }
		} private string address;
		
		/// <summary>
		/// 
		/// </summary>
		public string InputTriggerType
		{
			get { return inputTriggerType; }
			set	{	inputTriggerType = value;		}
		} private string inputTriggerType;
		
	}
}
