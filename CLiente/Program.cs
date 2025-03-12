using System;
using System.Net.Sockets;
using System.Text;

class GameClient
{
    static void Main()
    {
        string serverIp = "192.168.56.101";
        int port = 9050;

        try
        {
            while (true) // Bucle infinito para mantener el cliente activo
            {
                using (TcpClient client = new TcpClient(serverIp, port))
                using (NetworkStream stream = client.GetStream())
                {
                    Console.WriteLine("Conectado al servidor. Ingrese una opción:");
                    Console.WriteLine("1 - Registrar");
                    Console.WriteLine("2 - Login");
                    Console.WriteLine("3 - Consultar jugadores");
                    Console.WriteLine("4 - Buscar ID");
                    Console.WriteLine("5 - Ver historial de partidas");
                    Console.WriteLine("6 - Salir");

                    string option = Console.ReadLine();

                    if (option == "6") // Opción para salir
                    {
                        Console.WriteLine("Saliendo del cliente...");
                        break;
                    }

                    string username = "";
                    if ((option != "3")&& (option != "4") && (option != "5")) // No pedir nombre si es la opción 3
                    {
                        Console.Write("Ingrese el nombre de usuario: ");
                        username = Console.ReadLine();
                    }

                    if (option == "4") // No pedir nombre si es la opción 3
                    {
                        Console.Write("Ingrese el nombre de usuario que busca la ID: ");
                        username = Console.ReadLine();
                    }

                    string message = option + "|" + username;
                    byte[] data = Encoding.ASCII.GetBytes(message);
                    stream.Write(data, 0, data.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    Console.WriteLine("Respuesta del servidor: " + response);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }
}