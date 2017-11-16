using System.Collections.ObjectModel;
using Knx.Bus.Common.Configuration;

namespace FalconDemo.Model
{
  /// <summary>
  /// This class represents the configured connections and provides some service 
  /// methods. 
  /// </summary>
  public class ConfiguredConnections 
  {
    private static ObservableCollection<DemoParameter> _connectorParameters = null;

    public static ObservableCollection<DemoParameter> GetConfiguredConnections()
    {
      return _connectorParameters = ConnectionManagerPersister.LoadFalconConnections();
    }

    public static void SetConfiguredConnections(ObservableCollection<DemoParameter> list)
    {
      ConnectionManagerPersister.WriteFalconConnections(list);
    }

    public static ConnectorParameters GetSpecifiedConnection(string name)
    {
      foreach (DemoParameter connection in _connectorParameters)
      {
        if (connection.Name == name)
        {
          return connection;
        }
      }
      return null;
    }
  }
}
