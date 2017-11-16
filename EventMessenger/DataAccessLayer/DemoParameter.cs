using System;
using System.Runtime.Serialization;
using Knx.Bus.Common;
using Knx.Bus.Common.Configuration;

namespace FalconDemo.Model
{
  /// <summary>
  /// Represents the specialized connector parameter for this demo application 
  /// </summary>
  [Serializable]
  public class DemoParameter : ConnectorParameters, ISerializable   
  {
    private readonly ConnectorParameters _parameter;
    private string _macAddress;
    private readonly bool _isInProgMode;
    private string _displayName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DemoParameter"/> class.
    /// </summary>
    public DemoParameter()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DemoParameter"/> class.
    /// </summary>
    /// <param name="parameter">The Falcon Connector Parameters.</param>
    /// <param name="displayName">The display name.</param>
    public DemoParameter(ConnectorParameters parameter, string displayName)
    {
      _parameter = parameter;
      _displayName = displayName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DemoParameter"/> class.
    /// </summary>
    /// <param name="parameter">The  Falcon Connector Parameters.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="macAddress">The MAC address for tunneling connections.</param>
    /// <param name="isInProgMode">if set to <c>true</c> the device [is in programming mode].</param>
    public DemoParameter(ConnectorParameters parameter, string displayName, string macAddress, bool isInProgMode)
    {
      _parameter = parameter;
      _macAddress = macAddress;
      _isInProgMode = isInProgMode;
      _displayName = displayName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DemoParameter"/> class.
    /// </summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    public DemoParameter(SerializationInfo info, StreamingContext context)
    {
      string type = info.GetString("Type");
      if (type == "KnxIpRouting")
      {
        _parameter = new KnxIpSecureRoutingConnectorParameters(info, context);
      }
      else if (type == "KnxIpTunneling")
      {
        _parameter = new KnxIpTunnelingConnectorParameters(info, context);
      }
      else if (type == "USB")
      {
        _parameter = new UsbConnectorParameters(info, context);
      }

      _macAddress = info.GetString("MacAddress");
      _isInProgMode = info.GetBoolean("isInProgMode"); ;
      _displayName = info.GetString("DisplayName"); ;
    }
    
    /// <summary>
    /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param><param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
    public new void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      switch (Type)
      {
          case ConnectorTypes.USB:
          {
            UsbConnectorParameters usbConnectorParameters = _parameter as UsbConnectorParameters;
            if (usbConnectorParameters != null)
            {
              usbConnectorParameters.GetObjectData(info, context);
              info.AddValue("Type", ConnectorTypes.USB.ToString());
            }
            break;
          }
          case ConnectorTypes.KnxIpRouting:
          {
            var knxIpRoutingConnectorParameters = _parameter as KnxIpSecureRoutingConnectorParameters;
            if (knxIpRoutingConnectorParameters != null)
            {
              knxIpRoutingConnectorParameters.GetObjectData(info, context);
              info.AddValue("Type", ConnectorTypes.KnxIpRouting.ToString());
            }
            break;
          }
          case ConnectorTypes.KnxIpTunneling:
          {
            KnxIpTunnelingConnectorParameters knxIpTunnelingConnectorParameters = _parameter as KnxIpTunnelingConnectorParameters;
            if (knxIpTunnelingConnectorParameters != null)
            {
              knxIpTunnelingConnectorParameters.GetObjectData(info, context);
              info.AddValue("Type", ConnectorTypes.KnxIpTunneling.ToString());
            }
            break;
          }
      }
      info.AddValue("DisplayName", _displayName);
      info.AddValue("MacAddress", _macAddress);
      info.AddValue("isInProgMode", _isInProgMode);
    }

    protected override bool InternalEquals(ConnectorParameters other)
    {
      if( other.Equals(_parameter))
      {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Returns the type of the connector for which parameters are provided.
    /// </summary>
    public override ConnectorTypes Type
    {
      get
      {
        if(_parameter != null)
        {
          return _parameter.Type;
        }
        return ConnectorTypes.Unknown;
      }
    }

    /// <summary>
    /// Returns or sets the name of the interface. 
    /// May be null. The name is set by Falcon when enumerating devices. It has no relevance when opening a connection.
    /// </summary>
    public new string Name
    {
      get
      {
        if (_parameter != null)
        {
          return _parameter.Name;
        }
        return "";
      }
    }

    /// <summary>
    /// Gets or sets the MAC address.
    /// </summary>
    /// <value>
    /// The MAC address.
    /// </value>
    public string MacAddress
    {
      get
      {
        return _macAddress;
      }
      set
      {
        _macAddress = value;
      }
    }


    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    public string DisplayName
    {
      get
      {
        if(String.IsNullOrEmpty(_displayName))
        {
          _displayName = Name;
        }
        return _displayName;
      }
      set
      {
        _displayName = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is in prog mode.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if the device is in programming mode; otherwise, <c>false</c>.
    /// </value>
    public bool IsInProgMode
    {
      get
      {
        return _isInProgMode;
      }
    }

    /// <summary>
    /// Gets the Falcon ConnectorParameter.
    /// </summary>
    public ConnectorParameters Parameter
    {
      get
      {
        return _parameter;
      }
    }
  }
}
