using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Knx.Bus.Common.Configuration;

namespace FalconDemo.Model
{
  /// <summary>
  /// This class reads an writes the configured connections from respectively into a XML file. 
  /// </summary>
  public class ConnectionManagerPersister
  {
    private static readonly string _falconConnectionsCommonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"KNX\Falcon");
    private static readonly string _xmlConfigPath = Path.Combine(_falconConnectionsCommonPath, @"ConnectionManagerPublicDemo.xml");

    /// <summary>
    /// Loads the falcon connections.
    /// </summary>
    /// <returns></returns>
    public static ObservableCollection<DemoParameter> LoadFalconConnections()
    {
      ObservableCollection<DemoParameter> result = new ObservableCollection<DemoParameter>();
      if (File.Exists(_xmlConfigPath))
      {
        try
        {
          using (XmlReader reader = XmlReader.Create(_xmlConfigPath))
          {
            DataContractSerializer serializer = new DataContractSerializer(typeof (ObservableCollection<DemoParameter>),
              new[]
              {
                typeof (UsbConnectorParameters),
                typeof (KnxIpSecureRoutingConnectorParameters),
                typeof (KnxIpTunnelingConnectorParameters)
              });

            ObservableCollection<DemoParameter> list =
              (ObservableCollection<DemoParameter>) serializer.ReadObject(reader);
            return list;
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          File.Delete(_xmlConfigPath);
          return new ObservableCollection<DemoParameter>();
        }
      }
      return result;
    }

    /// <summary>
    /// Writes the configured connections to the XML file.
    /// </summary>
    /// <param name="connectionList">The connection list.</param>
    public static void WriteFalconConnections(ObservableCollection<DemoParameter> connectionList)
    {
      if (!Directory.Exists(Path.GetDirectoryName(_xmlConfigPath)))
      {
        Directory.CreateDirectory(Path.GetDirectoryName(_xmlConfigPath));
      }
      using (XmlWriter writer = XmlWriter.Create(_xmlConfigPath,
                                                 new XmlWriterSettings
                                                   {
                                                     Indent = true,
                                                     IndentChars = "  ",
                                                     Encoding = Encoding.UTF8,
                                                     CloseOutput = true
                                                   }))
      {
        DataContractSerializer serializer = new DataContractSerializer(typeof(ObservableCollection<DemoParameter>),
                                                                       new[] 
                                                                         {
                                                                           typeof (UsbConnectorParameters),
                                                                           typeof (KnxIpSecureRoutingConnectorParameters),
                                                                           typeof (KnxIpTunnelingConnectorParameters)
                                                                         });
        serializer.WriteObject(writer, connectionList.ToList());
      }
    }
  }
}
