
namespace Req
{
    using System;
    using System.Text;
    using System.IO;
    using ZeroMQ;

    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Iniciando o Cliente...\n");
            Console.WriteLine("Digite o endpoint desejado para realizar a conexão: ");
            var ip = Console.ReadLine();
            
            using (var context = ZmqContext.Create())
            {
                using (var socket = context.CreateSocket(SocketType.REQ))
                {
                    var endpoint = $"tcp://{ip}:5000";
                    socket.Connect(endpoint);
                    Console.WriteLine("Conectando com o endpoint {0}...\n", endpoint);
                    var menuOption = "";
                    while (menuOption != "10")
                    {
                        Console.Clear();
                        ShowMainMenu();
                        string requestMessage = "";
                        SelectsOptionAndDefinesRequest(out menuOption, out requestMessage);
                        //Console.WriteLine($"\nEnviando requisição {requestMessage} para o servidor...");
                        socket.Send(requestMessage, Encoding.UTF8);
                        var replyMessage = socket.Receive(Encoding.UTF8);
                        Console.WriteLine($"Resposta do Servidor:\n {replyMessage}{Environment.NewLine}");
                        Console.WriteLine("Pressione [ENTER] para voltar ao menu principal...");
                        Console.ReadLine();
                    }
                    Environment.Exit(1);
                }
            }
        }

        private static void SelectsOptionAndDefinesRequest(out string menuOption, out string requestMessage)
        {
            menuOption = Console.ReadLine();
            requestMessage = "";
            var folderName = "";
            var fileName = "";
            var newFolderName = "";
            Console.Clear();
            
            switch (menuOption)
            {
                case "1":
                    Console.WriteLine("**Listar todos os diretórios do servidor**\n");
                    requestMessage = "ListAll";
                    break;
                case "2":
                    Console.WriteLine("**Listar arquivos de um diretório**\n");
                    Console.WriteLine("Digite o nome do diretório para listar os arquivos: ");
                    folderName = Console.ReadLine();
                    requestMessage = $"List-{folderName}";
                    break;
                case "3":
                    Console.WriteLine("**Criar um novo diretório**\n");
                    Console.WriteLine("Digite o nome do diretório que deseja criar: ");
                    folderName = Console.ReadLine();
                    requestMessage = $"CreateFolder-{folderName}";
                    break;
                case "4":
                    Console.WriteLine("**Fazer upload de um arquivo**\n");
                    Console.WriteLine("Digite o diretório de destino do upload: ");
                    folderName = Console.ReadLine();
                    Console.WriteLine("Digite caminho do arquivo que será carregado: ");
                    fileName = Console.ReadLine();
                    requestMessage = $"UploadFile-{folderName}-{fileName}";
                    break;
                case "5":
                    Console.WriteLine("**Fazer download de um arquivo**\n");
                    Console.WriteLine("Digite o diretório do arquivo de download: ");
                    folderName = Console.ReadLine();
                    Console.WriteLine("Digite o nome do arquivo de download: ");
                    fileName = Console.ReadLine();
                    Console.WriteLine("Digite o caminho de destino download: ");
                    var downloadPath = Console.ReadLine();
                    requestMessage = $"DownloadFile-{folderName}-{fileName}-{downloadPath}";
                    break;
                case "6":
                    Console.WriteLine("**Deletar um diretório**\n");
                    Console.WriteLine("Digite o nome do diretório que deseja deletar: ");
                    folderName = Console.ReadLine();
                    requestMessage = $"DeleteFolder-{folderName}";
                    break;
                case "7":
                    Console.WriteLine("**Deletar um arquivo**\n");
                    Console.WriteLine("Digite o nome do diretório do arquivo que será deletado: ");
                    folderName = Console.ReadLine();
                    Console.WriteLine("Digite o nome do arquivo que deseja deletar: ");
                    fileName = Console.ReadLine();
                    requestMessage = $"DeleteFile-{folderName}-{fileName}";
                    break;
                case "8":
                    Console.WriteLine("**Copiar um diretório**\n");
                    Console.WriteLine("Digite o nome do diretório que deseja copiar: ");
                    folderName = Console.ReadLine();
                    Console.WriteLine("Digite o nome do novo diretório: ");
                    newFolderName = Console.ReadLine();
                    requestMessage = $"CopyFolder-{folderName}-{newFolderName}";
                    break;
                case "9":
                    Console.WriteLine("**Copiar um arquivo**\n");
                    Console.WriteLine("Digite o nome do diretório do arquivo que será copiado: ");
                    folderName = Console.ReadLine();
                    Console.WriteLine("Digite o nome do arquivo que deseja copiar: ");
                    fileName = Console.ReadLine();
                    Console.WriteLine("Digite o nome do diretório de destino: ");
                    var folderDestination = Console.ReadLine();
                    requestMessage = $"CopyFile-{folderName}-{fileName}-{folderDestination}";
                    break;
                case "10":
                    requestMessage = "Exit";
                    Console.WriteLine("Saindo do REMOTE FILE MANAGER...");
                    break;
            }
        }

        private static void ShowMainMenu()
        {
            Console.WriteLine("\nREMOTE FILE MANAGER\n");
            Console.WriteLine("1 - Listar diretórios do servidor");
            Console.WriteLine("2 - Listar arquivos de um diretório");
            Console.WriteLine("3 - Criar um novo diretório");
            Console.WriteLine("4 - Fazer upload de um arquivo");
            Console.WriteLine("5 - Fazer download de um arquivo");
            Console.WriteLine("6 - Deletar um diretório");
            Console.WriteLine("7 - Deletar um arquivo");
            Console.WriteLine("8 - Copiar um diretório");
            Console.WriteLine("9 - Copiar um arquivo");
            Console.WriteLine("10 - Sair do REMOTE FILE MANAGER");
            Console.WriteLine("\nSelecione uma opção: ");
        }
    }
}
