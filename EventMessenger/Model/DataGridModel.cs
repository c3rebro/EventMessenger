using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;

namespace EventMessenger.Model
{
	/// <summary>
	/// Description of DataBlockDataGridModel.
	/// </summary>
	public class DataGridModel
	{
		
		//CustomConverter converter = new CustomConverter();

		private ComboBox Cmbbx;
		
		private string _status;
		private string _eventName;
		private string _eventType;
		private string _reactionName;
		private string _reactionType;
		
		public DataGridModel(string status, string eventName, string eventType, string reactionName, string reactionType)
		{

			Cmbbx = new ComboBox();
			Cmbbx.Items.Add(new ComboBoxItem().Content="blabla");
			
			_status= status;
			_reactionType=reactionType;
			_reactionName=reactionName;
			_eventType=eventType;
			_eventName=eventName;
		}
		
		public string Status {
			get { return _status; }
			set { _status = value;
			}
		}
		
		public string EventName {
			get { return _eventName; }
			set { _eventName = value;
			}
		}
		
		public string EventType {
			get { return _eventName; }
			set { _eventName = value;
			}
		}
		
		public string ReactionName {
			get { return _eventName; }
			set { _eventName = value;
			}
		}
		
		public string ReactionType {
			get { return _eventName; }
			set { _eventName = value;
			}
		}
	}
	
	internal class DataGridModelEventObject
	{
		private DataGridModelEventObject()
		{
			
		}
	}
}
