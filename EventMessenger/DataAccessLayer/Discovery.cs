using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
//using FalconDemo.ViewModel;
using Knx.Bus.Common;
using Knx.Bus.Common.Configuration;
using Knx.Bus.Common.KnxIp;
using Knx.Falcon.Sdk;

namespace FalconDemo.Model
{
  /// <summary>
  /// This class explains the usage of the discovery client. 
  /// </summary>
  public class Discovery
  {
    /// <summary>
    /// First a discovery has to be called.
    /// </summary>
    /// <returns></returns>
    public static DiscoveryResult[] Discover()
    {
      DiscoveryClient discoveryClient = new DiscoveryClient(AdapterTypes.All);
      DiscoveryResult[] discoveryResult = discoveryClient.Discover();
      return discoveryResult;
    }


    /// <summary>
    /// Dependent on the discovery result the needed parameters can be collected.
    /// In this case KnxIpRoutingConnectorParameters are needed. 
    /// </summary>
    /// <param name="discoveryResult">The discovery result.</param>
    /// <returns></returns>
    public static ObservableCollection<DemoParameter> GetRoutingParameters(DiscoveryResult[] discoveryResult)
    {
        ObservableCollection<DemoParameter> routingConnectorParameters = new ObservableCollection<DemoParameter>();

        //Get all network adapters 
        NetworkAdapterEnumerator enumerator = new NetworkAdapterEnumerator(AdapterTypes.All);

        foreach (NetworkAdapterInfo networkAdapterInfo in enumerator)
        {
          DiscoveryResult result;
          int count =  GetCount(networkAdapterInfo, discoveryResult, out result);

          if (result != null)
          {
            KnxIpSecureRoutingConnectorParameters parameter = new KnxIpSecureRoutingConnectorParameters(result.MulticastAddress.ToString(),
              result.IndividualAddress, null, result.LocalAddress.ToString());
//            if (result.HasSecuredServiceFamilies(ServiceFamilies.Routing))
//            {
//              parameter.BackboneKey = new EncryptedBuffer(ConnectionManagerViewModel.GetDefaultKey());
//            }
            parameter.Name = string.Format("({0}) {1}", count, networkAdapterInfo.Description);
            routingConnectorParameters.Add(new DemoParameter(parameter, string.Format("({0}) {1}", count, networkAdapterInfo.Description)));
          }
          else
          {
            KnxIpSecureRoutingConnectorParameters parameter = new KnxIpSecureRoutingConnectorParameters("224.0.23.12", "0.0.1", null);
            parameter.Name = string.Format("({0}) {1}", 0, networkAdapterInfo.Description);
            routingConnectorParameters.Add(new DemoParameter(parameter, parameter.Name));
          }
        }
        return routingConnectorParameters;
     }

    private static int GetCount(NetworkAdapterInfo info, DiscoveryResult[] discoveryResult, out DiscoveryResult result)
    {
      List<DiscoveryResult> devices = GetRoutingDevices(discoveryResult);
      int index = 0;
      result = null;

      foreach (DiscoveryResult device in devices)
      {
        if(device.NetworkAdapterInfo.Equals(info))
        {
          index++;
          result = device;
        }
      }
      return index;
    }
    
    private static List<DiscoveryResult> GetRoutingDevices(DiscoveryResult[] discoveryResult)
    {
      List<DiscoveryResult> routingResult = new List<DiscoveryResult>();

      foreach (DiscoveryResult result in discoveryResult)
      {
        if (result.SupportedServiceFamilies.Any(fam => fam.ServiceFamily == ServiceFamilies.Routing))
        {
          if (!routingResult.Contains(result))
          {
            routingResult.Add(result);
          }
        }
      }
      return routingResult;
    }

    /// <summary>
    /// Dependent on the discovery result the needed parameters can be collected.
    /// In this case KnxIpTunnelingConnectorParameters are needed. 
    /// </summary>
    public static ObservableCollection<DemoParameter> GetTunnelingParameters(DiscoveryResult[] discoveryResult)
    {
      if (discoveryResult != null)
      {
        ObservableCollection<DemoParameter> tunnelingConnectorParameters =
          new ObservableCollection<DemoParameter>();
        foreach (DiscoveryResult result in discoveryResult)
        {
          if (result.SupportedServiceFamilies.Any(fam => fam.ServiceFamily == ServiceFamilies.Tunneling))
          {
            KnxIpTunnelingConnectorParameters parameter =
              new KnxIpTunnelingConnectorParameters(result.IpAddress.ToString(),
                                                    result.IpPort, false);

            parameter.Name = result.IsInProgMode ? string.Format("(P) {0}", result.FriendlyName) : result.FriendlyName;
            tunnelingConnectorParameters.Add(new DemoParameter(parameter, result.FriendlyName, result.MacAddress.ToString(), result.IsInProgMode));
          }
        }
        return tunnelingConnectorParameters;
      }
      return null;
    }
  }
}
