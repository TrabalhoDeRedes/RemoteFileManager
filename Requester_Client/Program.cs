﻿
namespace Req
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using CommandLine;
    using ZeroMQ;

    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            options.connectEndPoints = new List<string>();
            options.connectEndPoints.Add("tcp://127.0.0.1:5000");
            options.alterMessages = new List<string>();
            options.alterMessages.Add("World");
            /*var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
                Environment.Exit(1);*/

            using (var context = ZmqContext.Create())
            {
                using (var socket = context.CreateSocket(SocketType.REQ))
                {
                    foreach (var connectEndpoint in options.connectEndPoints)
                        socket.Connect(connectEndpoint);
                    long msgCptr = 0;
                    int msgIndex = 0;
                    while (true)
                    {
                        if (msgCptr == long.MaxValue)
                            msgCptr = 0;
                        msgCptr++;
                        if (options.maxMessage >= 0)
                            if (msgCptr > options.maxMessage)
                                break;
                        if (msgIndex == options.alterMessages.Count())
                            msgIndex = 0;
                        var reqMsg = options.alterMessages[msgIndex++].Replace("#nb#", msgCptr.ToString("d2"));
                        Thread.Sleep(options.delay);
                        Console.WriteLine("Sending : " + reqMsg);
                        socket.Send(reqMsg, Encoding.UTF8);
                        var replyMsg = socket.Receive(Encoding.UTF8);
                        Console.WriteLine("Received: " + replyMsg + Environment.NewLine);
                    }
                }
            }
        }
    }
}
