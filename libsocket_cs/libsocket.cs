using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace libsocket_cs
{
    public class serverSocket
    {
        private Socket listener = null;
        public serverSocket(int port)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.listener.Bind(localEndPoint);
            this.listener.Listen(10);
        }
        public void Close()
        {
            this.listener.Shutdown(SocketShutdown.Both);
            this.listener.Close();
        }
        public connectedSocket Accept()
        {
            Socket sock = listener.Accept();
            connectedSocket ret = new connectedSocket(sock);
            return ret;
        }
    }

    public class clientSocket
    {
        private connectedSocket sock;
        private Boolean error;
        public clientSocket(String host, int port = 80)
        {
            IPAddress ipAddress = IPAddress.Parse(host);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Socket tmpsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tmpsock.Connect(localEndPoint);
            this.sock = new connectedSocket(tmpsock);
        }
        public int Write(byte[] data, int size)
        {
            return this.sock.Write(data, size);
        }
        public int Read(ref byte[] data, int size)
        {
            return this.sock.Read(ref data, size);
        }
        public void Close()
        {
            this.sock.Close();
        }
    }

    public class connectedSocket
    {
        private Socket sock;
        public connectedSocket(Socket s)
        {
            this.sock = s;
        }
        public int Write(byte[] data, int size)
        {
            byte[] buf = new byte[size];
            for (int i = 0; i < size; i++)
                buf[i] = data[i];
            this.sock.SendBufferSize = size;
            return this.sock.Send(buf);
        }
        public int Read(ref byte[] data, int size)
        {
            this.sock.ReceiveBufferSize = size;
            return this.sock.Receive(data);
        }
        public void Close()
        {
            this.sock.Shutdown(SocketShutdown.Both);
            this.sock.Close();
        }
    }
}