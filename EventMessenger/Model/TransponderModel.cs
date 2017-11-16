/*
 * Created by SharpDevelop.
 * Date: 03/01/2017
 * Time: 23:58
 * 
 */
using System;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace EventMessenger.Model
{
	/// <summary>
	/// Description of TransponderModel.
	/// </summary>
	[XmlRootAttribute("TransponderModel", IsNullable = false)]
	public class TransponderModel
	{
		/// <summary>
		/// 
		/// </summary>
		public TransponderModel()
		{
			TransponderName = "";
			TransponderPHI = "";
			
			var locksAsModels = new ObservableCollection<LockModel>();
			
			if(locksAsModels != null)
				lockModels = new ObservableCollection<LockModel>(locksAsModels);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="traName"></param>
		/// <param name="traPHI"></param>
		/// <param name="locksAsModels"></param>
		public TransponderModel(string traName, string traPHI, ObservableCollection<LockModel> locksAsModels)
		{
			if(locksAsModels != null)
				lockModels = new ObservableCollection<LockModel>(locksAsModels);
			
			TransponderName = traName;
			TransponderPHI = traPHI;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="traName"></param>
		/// <param name="traPHI"></param>
		/// <param name="lockAsModel"></param>
		public TransponderModel(string traName, string traPHI, LockModel lockAsModel)
		{
			lockModels = new ObservableCollection<LockModel>();
			
			if(lockAsModel != null)
				lockModels.Add(lockAsModel);

			TransponderName = traName;
			TransponderPHI = traPHI;
		}
		
		private string selectedTransponderTriggerType;
		public string SelectedTransponderTriggerType
		{
			get {return selectedTransponderTriggerType; }
			set {selectedTransponderTriggerType = value; }
		}
		
		private string selectedLockName;
		public string SelectedLockName
		{
			get { return selectedLockName; }
			set { selectedLockName = value; }
		}
		
		private ObservableCollection<LockModel> lockModels;
		public ObservableCollection<LockModel> LockModels
		{
			get { return lockModels; }
			set { lockModels = value; }
		}
		
//		public ObservableCollection<string> LockModelNames
//		{
//			get {
//				using(null){
//					ObservableCollection<string> lmNames = new ObservableCollection<string>();
//					
//					foreach(LockModel lm in LockModels)
//					{
//						lmNames.Add(lm.LockNames);
//					}
//					
//					return lmNames;
//				}
//			}
//		}
		
		/// <summary>
		/// 
		/// </summary>
		public string TransponderPHI { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public string TransponderName { get; set; }
		
		public object MakeDeepCopy()
		{
			TransponderModel tmCopy = new TransponderModel();
			tmCopy = this.MemberwiseClone() as TransponderModel;
			
			tmCopy.LockModels = new ObservableCollection<LockModel>();
			
			foreach(LockModel lm in this.LockModels)
			{
				tmCopy.LockModels.Add((LockModel) lm.MakeDeepCopy());
			}
			
			return tmCopy;
		}
	}
}
