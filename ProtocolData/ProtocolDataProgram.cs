using System;
using System.Net.Sockets;
using System.Text;

namespace ProtocolData
{
    public enum action {PublishGame}

    public class ProtocolDataProgram
    {
        private const int ProtocolFixedSize = 4;

        
        private static void HandleData(string message)
        {
            string[] data = message.Split(Environment.NewLine);
            string actionType = data[0];
            string cmd = data[1];
            string[] info = data[2].Split("/-");

            if (actionType == "REQ")
            {
                
            }
            else
            {


            }

        }

        public void SendHeader(Socket socket, Header header)
        {
            //asi? o que manda el header
            string messageToSend = header.action;
            Send(socket, messageToSend);
        }

        public void SendData(Socket socket, string data)
        {
            Send(socket, data);
        }

        public void Send(Socket socket, string message)
        {    
                //2 Codifico el mensaje a bytes
                byte[] data = Encoding.UTF8.GetBytes(message);
                //3 Leo el largo del mensaje codificado
                //    (largo codificado <> largo mensaje)
                int length = data.Length;
                //4 Codifico dicho largo a bytes
                byte[] dataLength = BitConverter.GetBytes(length);

                //5 Envío el largo en bytes
                SendBatch(socket, dataLength);

                //6 Envío el mensaje
                SendBatch(socket, data);
            
        }

        public static void Listen(Socket socket, int size)
        {

            int totalReceivedBytes = 0;

            while (iBytesRecibidos > 0)
            {   //1 Creo la parte fija del protocolo
                byte[] dataLength = new byte[ProtocolFixedSize];
                //2 Recibo los datos fijos
                socket.Receive(dataLength);
                //3 Interpreto dichos bytes para obtener cuanto serán los datos variables
                int length = BitConverter.ToInt32(dataLength);
                //4 Creo que el buffer del tamaño exacto de el mensaje que va a venir
                byte[] data = new byte[length];
                //5 Recibo el mensaje (largo variable, que ahora se que es length)
                socket.Receive(data);
                //6 Convierto los datos a string
                string message = Encoding.UTF8.GetString(data);
                //7 Uso los datos
                HandleData(message);

            }
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public byte[] Receive(int size)
        {
            int totalReceivedBytes = 0;
            var data = new byte[size];
            while (totalReceivedBytes < size)
            {
                int receivedBytes = _socket.Receive(
                    data,
                    totalReceivedBytes,
                    size - totalReceivedBytes,
                    SocketFlags.None);
                if (receivedBytes == 0)
                {
                    throw new SocketException();
                }
                totalReceivedBytes += receivedBytes;
            }
            return data;
        }

        private static void SendBatch(Socket socket, byte[] data)
        {
            int totalDataSent = 0;

            while (totalDataSent < data.Length)
            {
                int sent = socket.Send(data, totalDataSent, data.Length - totalDataSent, SocketFlags.None);
                if (sent == 0)
                {
                    throw new SocketException();
                }
                totalDataSent += sent;
            }
        }
    }
}