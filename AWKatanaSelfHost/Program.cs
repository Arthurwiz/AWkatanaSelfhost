﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;


//Need to  Install-Package Microsoft.Owin.Host.HttpListener
namespace AWKatanaSelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUri = "http://localhost:8000";

            Console.WriteLine("Starting web Server...");
            WebApp.Start<Startup>(baseUri);
            Console.WriteLine("Server running at {0} - press Enter to quit. ", baseUri);
            Console.ReadLine();
        }
    }
}
