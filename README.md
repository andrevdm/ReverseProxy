ReverseProxy
============

A simple C# reverse proxy. It will redirect all TCP traffic from one port to another IP/port destination. Traffic can also be distributed to multiple destinations. In this case each connection is sent to one of the destination in a round-robin fashion.



Usage
-----

Configure the proxies in the app.config. For example

    <ReverseProxiesConfig type="ReverseProxy.ReverseProxiesConfig">
   <Proxies>
		<ReverseProxyConfig>
		  <HostPort>8077</HostPort>
		  <ForwardTo>
			 <PortForwardConfig>
				<ForwardToIp>127.0.0.1</ForwardToIp>
				<ForwardToPort>80</ForwardToPort>
			 </PortForwardConfig>
			 <PortForwardConfig>
				<ForwardToIp>127.0.0.1</ForwardToIp>
				<ForwardToPort>81</ForwardToPort>
			 </PortForwardConfig>
		  </ForwardTo>
		</ReverseProxyConfig>
		<ReverseProxyConfig>
		  <HostPort>8078</HostPort>
		  <ForwardTo>
			 <PortForwardConfig>
				<ForwardToIp>127.0.0.1</ForwardToIp>
				<ForwardToPort>80</ForwardToPort>
			 </PortForwardConfig>
		  </ForwardTo>
		</ReverseProxyConfig>
	 </Proxies>
	</ReverseProxiesConfig>

Here there are two proxies configured.

 1. From port 8077 to port 80 and 81
 2. From port 8078 to port 80
