// <copyright file="Program.cs" company="kdehaan">
// Copyright (c) kdehaan. All rights reserved.
// </copyright>

using System.Diagnostics;
using CloudDesignPatterns.AsyncRequestReply;
using CloudDesignPatterns.BaseComponents;

/// <summary>
/// Run as "server" or "client" with appropriate arguments.
/// </summary>
public class Program
{
    /// <summary>
    /// Main entry thread of the application.
    /// </summary>
    /// <param name="args">process arguments.</param>
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please specify 'server' or 'client' as the first argument, then the port as the second argument. Type 'exit' to quit.");
            string? line;
            while ((line = Console.ReadLine()) != null)
            {
                if (line.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                CreateProcess(line);
            }
        }

        if (args[0].Equals("server", StringComparison.OrdinalIgnoreCase))
        {
            int port = args.Length > 1 ? int.Parse(args[1]) : 5000;
            CreateServer(port);
        }
        else if (args[0].Equals("client", StringComparison.OrdinalIgnoreCase))
        {
            int port = args.Length > 1 ? int.Parse(args[1]) : 5000;
            CreateClient("127.0.0.1", port);
        }
        else
        {
            Console.WriteLine("Invalid argument. Please specify 'server' or 'client'.");
        }
    }

    private static void CreateProcess(string args)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "CloudDesignPatterns.exe",
            Arguments = args,
            UseShellExecute = true,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Normal,
        };
        try
        {
            using (Process? process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    Console.WriteLine("Failed to start process.");
                    throw new InvalidOperationException("Process.Start returned null.");
                }

                Console.WriteLine($"Created Process {process.Id}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start process: {ex.Message}");
        }
    }

    private static void CreateServer(int port = 5000)
    {
        var server = new AsyncRequestServer(port);
        Console.Title = $"Server - Port:{port}";
        server.Start();
    }

    private static void CreateClient(string serverAddress = "127.0.0.1", int port = 5000)
    {
        var client = new BaseClientApp(serverAddress, port);
        Console.Title = $"Client - {serverAddress}:{port}";
        client.Connect();
        Console.WriteLine("Type messages to send. Type 'exit' to quit.");
        string? line;
        while ((line = Console.ReadLine()) != null)
        {
            if (line.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            var args = line.Split('/', 2);
            client.Send(args[0], args[1]);
        }

        client.Disconnect();
    }
}
