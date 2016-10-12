using System;
using System.Net;
using System.Net.Sockets;

public class BBOSCReceiverAsync {} // bogus declaration to meet with unity's strict name = filename scheme


namespace OSC.NET
{
	/// <summary>
	/// OSCReceiver
	/// </summary>
	public class OSCReceiverAsync
	{
		
		private class UdpState
        {
            /// <summary>
            /// Gets the associated client.
            /// </summary>
            public UdpClient Client
            {
                get
                {
                    return mClient;
                }
            }

            /// <summary>
            /// Gets the associted end point.
            /// </summary>
            public IPEndPoint IPEndPoint
            {
                get
                {
                    return mIPEndPoint;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="UdpState"/> class.
            /// </summary>
            /// <param name="client">The associated client.</param>
            /// <param name="ipEndPoint">The associated end point.</param>
            public UdpState(UdpClient client, IPEndPoint ipEndPoint)
            {
                mClient = client;
                mIPEndPoint = ipEndPoint;
            }

            private UdpClient mClient;
            private IPEndPoint mIPEndPoint;
        }
		
		
		protected UdpClient udpClient;
		protected int localPort;
		protected AsyncCallback mAsyncCallback;
		public delegate void OSCReceiveCallback(OSCPacket packet);
		OSCReceiveCallback cb;
		
		public OSCReceiverAsync(int localPort)
		{
			this.localPort = localPort;
			Connect();
		}

		public void Connect()
		{
			if(this.udpClient != null) Close();
			this.udpClient = new UdpClient( this.localPort);
		}
		
		public void Start(OSCReceiveCallback callback){
			cb += callback;
			IPEndPoint ip 		= null;
			UdpState udpState 	= new UdpState(udpClient, ip);
			mAsyncCallback 		= new AsyncCallback(EndReceive);
	        udpClient.BeginReceive(mAsyncCallback, udpState);
		}
		
		
		/// <summary>
        /// EndReceive paired call.
        /// </summary>
        /// <param name="asyncResult">Paired result object from the BeginReceive call.</param>
        private void EndReceive(IAsyncResult asyncResult)
        {
            try
            {
                UdpState udpState = (UdpState)asyncResult.AsyncState;
                UdpClient udpClient = udpState.Client;
                IPEndPoint ipEndPoint = udpState.IPEndPoint;

                byte[] data = udpClient.EndReceive(asyncResult, ref ipEndPoint);
                if (data != null && data.Length > 0)
                {
					if(cb != null){
                    	cb(OSCPacket.Unpack(data));
					}
                }
                udpClient.BeginReceive(mAsyncCallback, udpState);
            }
            catch (ObjectDisposedException)
            {
                // Suppress error
            }
        }
		
		public void Close()
		{
			if (this.udpClient!=null) this.udpClient.Close();
			this.udpClient = null;
		}

		/*public OSCPacket Receive()
		{
            try
            {
                IPEndPoint ip = null;
				UdpState udpState = new UdpState(udpClient, ip);
	            mUdpClient.BeginReceive(mAsynCallback, udpState);
                byte[] bytes = this.udpClient.Receive(ref ip);
                if (bytes != null && bytes.Length > 0)
                    return OSCPacket.Unpack(bytes);

            } catch (Exception e) { 
                Console.WriteLine(e.Message);
                return null;
            }

			return null;
		}*/
	}
}
