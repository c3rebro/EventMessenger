﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Resources;
using System.Windows.Markup;

namespace EventMessenger
{
	/// <summary>
	/// Description of ResourceLoaderViewModel.
	/// </summary>
	
	/// <summary>
	/// Enables Binding even if target is not part of visual or logical tree. Thanks to:
	/// https://www.thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/
	/// </summary>
	public class BindingProxy : Freezable
	{
		#region Overrides of Freezable
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override Freezable CreateInstanceCore()
		{
			return new BindingProxy();
		}
		
		#endregion
		
		public object Data
		{
			get { return (object)GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}
		
		// Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
	}
	
	/// <summary>
	/// 
	/// </summary>
	public sealed class EnumerateExtension : MarkupExtension
	{
		private readonly SettingsReaderWriter settings;
		private readonly CultureInfo cultureInfo;
		private readonly ResourceManager resManager;
		
		/// <summary>
		/// 
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		public EnumerateExtension(Type type)
		{
			this.Type = type;
			
			settings = new SettingsReaderWriter();
			resManager = new ResourceManager("EventMessenger.Resources.Manifest", System.Reflection.Assembly.GetExecutingAssembly());
			settings.ReadSettings();
			
			cultureInfo = (settings.DefaultLanguage == "german") ? new CultureInfo("de") : new CultureInfo("en");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceProvider"></param>
		/// <returns></returns>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			string[] names = Enum.GetNames(Type);
			string[] values = new string[names.Length];

			for (int i = 0; i < names.Length; i++)
			{ values[i] = resManager.GetString(string.Format("ENUM.{0}",names[i]), cultureInfo); }

			return values;
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	public sealed class ResourceLoader : IValueConverter, IDisposable
	{
		
		private readonly SettingsReaderWriter settings;
		private readonly CultureInfo cultureInfo;
		private readonly ResourceManager resManager;
		
		/// <summary>
		/// 
		/// </summary>
		public ResourceLoader()
		{
			settings = new SettingsReaderWriter();
			resManager = new ResourceManager("EventMessenger.Resources.Manifest", System.Reflection.Assembly.GetExecutingAssembly());
			settings.ReadSettings();
			
			cultureInfo = (settings.DefaultLanguage == "german") ? new CultureInfo("de") : new CultureInfo("en");
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo language)
		{
			try{
				if(parameter is string)
					return resManager.GetString((parameter as string), cultureInfo);
				else if (value != null && value.GetType() == typeof(ObservableCollection<string>))
				{
					var collection = new ObservableCollection<string>();
					
					foreach(string s in value as ObservableCollection<string>)
					{
						collection.Add(resManager.GetString(string.Format("ENUM.{0}",s), cultureInfo));
					}
					return collection;
				}
				else if (value != null && !(value is string))
					return resManager.GetString(string.Format("ENUM.{0}",Enum.GetName(value.GetType(), value)), cultureInfo );
				else if (value is string)
					return resManager.GetString(string.Format("ENUM.{0}",value), cultureInfo);
				else
					return "Ressource not Found";
			}
			catch(Exception e)
			{
				LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
				
				throw new Exception(
					string.Format("parameter:{0}\nvalue:{1}",
					              parameter ?? "no param" ,
					              value ?? "no value"));
			}

		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
		{
			if(value != null)
			{
				string[] names = Enum.GetNames(parameter as Type);
				string[] values = new string[names.Length];

				for (int i = 0; i < names.Length; i++)
				{
					values[i] = resManager.GetString(string.Format("ENUM.{0}",names[i]), cultureInfo);
					if(	(string)value == values[i])
						return names[i];
				}
				
				throw new ArgumentException(null, "value");
			}
			
			return null;

		}
		
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="resName"></param>
		/// <returns></returns>
		public string getResource(string resName){
			return resManager.GetString(resName, cultureInfo);
		}
		

	}
}
