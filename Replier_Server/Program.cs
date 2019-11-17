
namespace Rep
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using CommandLine;
    using ZeroMQ;

    class Program
    {
        static void Main(string[] args)
        {
            /*var options = new Options();
            options.bindEndPoints = new List<string>();
            options.bindEndPoints.Add("tcp://127.0.0.1:5000");
            options.replyMessage = "Hello";
            var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
                Environment.Exit(1);*/

            Console.WriteLine("Iniciando o Servidor...");
            Console.WriteLine("Digite o endpoint desejado para realizar o bind: ");
            var ip = Console.ReadLine();

            using (var context = ZmqContext.Create())
            {
                using (var socket = context.CreateSocket(SocketType.REP))
                {
                    var endpoint = $"tcp://{ip}:5000";
                    socket.Bind(endpoint);
                    Console.WriteLine("Realizando o bind no endpoint {0}...", endpoint);
                    var remoteFileManagerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "RemoteFileManager");
                    if(!Directory.Exists(remoteFileManagerPath))
                        Directory.CreateDirectory(remoteFileManagerPath);

                    while (true)
                    {
                        var receivedMessage = socket.Receive(Encoding.UTF8);
                        var commands = receivedMessage.Split('-');

                        var folderPath = Path.Combine(remoteFileManagerPath, commands[1]);
                        Console.WriteLine("Requisição recebida: " + receivedMessage);
                        if(commands[0].Equals("create"))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        /*var replyMessage = options.replyMessage.Replace("#msg#", receivedMessage);
                        Console.WriteLine("Sending : " + replyMessage + Environment.NewLine);                        
                        socket.Send(replyMessage, Encoding.UTF8);*/
                    }
                }
            }
        }     
    }
}
