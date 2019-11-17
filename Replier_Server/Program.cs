
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

            Console.WriteLine("Iniciando o Servidor...\n");
            Console.WriteLine("Digite o endpoint desejado para realizar o bind: ");
            var ip = Console.ReadLine();

            using (var context = ZmqContext.Create())
            {
                using (var socket = context.CreateSocket(SocketType.REP))
                {
                    var endpoint = $"tcp://{ip}:5000";
                    socket.Bind(endpoint);
                    Console.WriteLine("Realizando o bind no endpoint {0}...\n", endpoint);
                    var remoteFileManagerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "RemoteFileManager");
                    if(!Directory.Exists(remoteFileManagerPath))
                        Directory.CreateDirectory(remoteFileManagerPath);
                    string command = "";

                    while (command != "Exit")
                    {
                        string receivedMessage, replyMessage;
                        ReceivesRequestAndExecutesCommands(socket, remoteFileManagerPath, out receivedMessage, out replyMessage, out command);
                        Console.WriteLine($"Requisição recebida do Cliente: {receivedMessage}");
                        Console.WriteLine($"Enviando Resposta : {replyMessage}{Environment.NewLine}");
                        socket.Send(replyMessage, Encoding.UTF8);
                    }

                    Environment.Exit(1);
                }
            }
        }

        private static void ReceivesRequestAndExecutesCommands(ZmqSocket socket, string remoteFileManagerPath, out string receivedMessage, out string replyMessage, out string command)
        {
            receivedMessage = socket.Receive(Encoding.UTF8);
            var commands = receivedMessage.Split('-');
            command = commands[0];
            var folderPath = "";
            var filePath = "";
            var newFolderPath = "";
            var newFilePath = "";
            replyMessage = "";
            switch (commands[0])
            {
                case "ListAll":
                    replyMessage = ListSubDirectoriesFolder(remoteFileManagerPath);
                    break;
                case "List":
                    replyMessage = ListFilesFolder(remoteFileManagerPath, commands[1]);
                    break;
                case "CreateFolder":
                    folderPath = Path.Combine(remoteFileManagerPath, commands[1]);
                    try
                    {
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                            replyMessage = $"Diretório {commands[1]} criado com sucesso!";
                        }
                        else
                        {
                            replyMessage = $"O diretório {commands[1]} já existe.";
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case "UploadFile":
                    var fileName = Path.GetFileName(commands[2]);
                    filePath = Path.Combine(remoteFileManagerPath, commands[1], fileName);
                    try
                    {   
                        if (!File.Exists(filePath))
                        {
                            File.Copy(commands[2], filePath);
                            replyMessage = $"Upload do arquivo {fileName} realizado com sucesso!";
                        }
                        else
                        {
                            replyMessage = $"O arquivo {fileName} já existe no diretório.";
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case "DownloadFile":
                    fileName = Path.GetFileName(commands[2]);
                    filePath = Path.Combine(remoteFileManagerPath, commands[1], commands[2]);
                    var destionationPath = Path.Combine(commands[3], fileName);
                    try
                    {
                        if (!File.Exists(destionationPath))
                        {
                            File.Copy(filePath, destionationPath);
                            replyMessage = $"Download do arquivo {fileName} realizado com sucesso!";
                        }
                        else
                        {
                            replyMessage = $"O arquivo {fileName} já existe no diretório.";
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case "DeleteFolder":
                    folderPath = Path.Combine(remoteFileManagerPath, commands[1]);
                    try
                    {
                        if (Directory.Exists(folderPath))
                        {
                            Directory.Delete(folderPath);
                            replyMessage = $"Diretório {commands[1]} deletado com sucesso!";
                        }
                        else
                        {
                            replyMessage = $"O diretório {commands[1]} não existe.";
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case "DeleteFile":
                    filePath = Path.Combine(remoteFileManagerPath, commands[1], commands[2]);
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            replyMessage = $"Arquivo {commands[2]} deletado com sucesso!";
                        }
                        else
                        {
                            replyMessage = $"O arquivo {commands[2]} não existe.";
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case "CopyFolder":
                    folderPath = Path.Combine(remoteFileManagerPath, commands[1]);
                    newFolderPath = Path.Combine(remoteFileManagerPath, commands[2]);
                    try
                    {
                        if (Directory.Exists(folderPath))
                        {
                            CopyFolder(folderPath, newFolderPath);
                            replyMessage = $"Diretório {commands[2]} copiado com sucesso!";
                        }
                        else
                        {
                            replyMessage = $"O diretório {commands[1]} não existe.";
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case "CopyFile":
                    filePath = Path.Combine(remoteFileManagerPath, commands[1], commands[2]);
                    newFilePath = Path.Combine(remoteFileManagerPath, commands[3], commands[2]);
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Copy(filePath, newFilePath);
                            replyMessage = $"Arquivo {commands[2]} copiado com sucesso!";
                        }
                        else
                        {
                            replyMessage = $"O arquivo {commands[1]} não existe.";
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case "Exit":
                    Console.WriteLine("Saindo do REMOTE FILE MANAGER...");
                    break;
            }
        }

        private static string ListFilesFolder(string remoteFileManagerPath, string folderParameterPath)
        {
            var folderPath = Path.Combine(remoteFileManagerPath, folderParameterPath);
            var filesPath = Directory.GetFiles(folderPath);
            IList<string> filesName = new List<string>();
            for (int i = 0; i < filesPath.Length; ++i)
            {
                filesName.Add(Path.GetFileName(filesPath[i]));
            }
            return filesName.Aggregate($"\nArquivos do Diretório {folderParameterPath}: ", (before, next) => before + "\n" + next);
        }

        private static string ListSubDirectoriesFolder(string folderPath)
        {
            var folderName = Path.GetFileName(folderPath);
            var directoriesPath = Directory.GetDirectories(folderPath);
            IList<string> directoriesName = new List<string>();
            for (int i = 0; i < directoriesPath.Length; ++i)
            {
                directoriesName.Add(Path.GetFileName(directoriesPath[i]));
            }
            return directoriesName.Aggregate($"\nDiretórios da pasta: {folderName}", (before, next) => before + "\n" + next);

        }

        private static void CopyFolder(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAllDirectoryFiles(diSource, diTarget);
        }

        private static void CopyAllDirectoryFiles(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAllDirectoryFiles(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
