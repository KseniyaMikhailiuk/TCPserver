﻿using System;
using System.Text;
using System.Net.Sockets;

namespace ChatServer
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string UserName;
        TcpClient Client;
        ServerObject server;

        public ClientObject(TcpClient tcpclient, ServerObject serverobject)
        {
            Id = Guid.NewGuid().ToString();
            Client = tcpclient;
            server = serverobject;
            server.AddConection(this);
        }

        public void Process()
        {
            try
            {
                Stream = Client.GetStream();
                string message = GetMessage();
                UserName = message;

                message = UserName + " вошел в чат";
                server.BroadcastMessage(message, Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0} : {1}", UserName, message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, Id);
                    }
                    catch
                    {
                        message = String.Format("{0} : покинул чат", UserName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        protected internal void Close()
        {
            if (Stream != null)
            {
                Stream.Close();
            }
            if (Client != null)
            {
                Client.Close();
            }
        }

        //читает поток байтов из Stream, преобразует полученный массив байтов в строку
        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (Stream.DataAvailable);
            return builder.ToString();
        }
    }
}
