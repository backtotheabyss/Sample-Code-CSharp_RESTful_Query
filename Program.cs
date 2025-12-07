using CSharp_Net8_RESTful_Query.Classes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using static DTO.DTO;

namespace CSharp_Net8_RESTful_Query
{
    public class Program
    {
        const int CONSOLEMAXLINES = 5000;

        static void Main(string[] args)
        {
            string tErrorMessage = string.Empty;

            ConsoleKeyInfo keyInfo;
            Console.BufferHeight = CONSOLEMAXLINES;

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());  // Ruta actual
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddEnvironmentVariables();                     // opcional
                })
                .ConfigureServices((ctx, services) =>
                {
                    // services.AddSingleton<Settings>();
                    services.AddSingleton<Settings>(sp => new Settings(sp.GetRequiredService<IConfiguration>()));
                    services.AddTransient<Customers>();
                })
                .Build();

            Customers customersObj = host.Services.GetRequiredService<Customers>();
            Response<Customer> customers = customersObj.customersRetrieve(10);            

            if (!customers.Success)
            {
                Console.WriteLine("customersRetrieve returned an error.");
                tErrorMessage = customers.ErrorMessage;

                Console.WriteLine($"{tErrorMessage}");
            }
            else if (customers.Success && (customers.Results == null || customers.Results.Count == 0))
            {
                Console.WriteLine("No customers have been found.");
                tErrorMessage = customers.ErrorMessage;

                Console.WriteLine($"{tErrorMessage}");
            }
            else
            {
                /* customers - retrieve */
                foreach (var customer in customers.Results)
                    Console.WriteLine($"{customer.CustomerId} - {customer.CompanyName} - {customer.Country}");

                /* console output - continue */
                Console.WriteLine("-------------------------");
                Console.WriteLine("Press any key to continue ...");
                Console.ReadKey();

                /* customers - by country */
                int maxRows = 50;
                string Country = "Italy";

                customers = customersObj.customersByCountry(maxRows, Country);

                if (!customers.Success)
                {
                    Console.WriteLine("customersByCountry returned an error.");
                    tErrorMessage = customers.ErrorMessage;

                    Console.WriteLine($"{tErrorMessage}");
                }
                else if (customers.Success && (customers.Results == null || customers.Results.Count == 0))
                {
                    Console.WriteLine("customersByCountry returned an error.");
                    tErrorMessage = customers.ErrorMessage;

                    Console.WriteLine($"{tErrorMessage}");
                }
                else
                {
                    Console.WriteLine(Country);
                    Console.WriteLine("-------------");

                    foreach (var customer in customers.Results)
                        Console.WriteLine($"CustomerID: {customer.CustomerId}, CompanyName: {customer.CompanyName}");
                }

                /* console output - continue */
                Console.WriteLine("-------------------------");
                Console.WriteLine("Press any key to continue ...");
                Console.ReadKey();

                // customersObj.customersGroupedByCountry(50);
                customers = customersObj.customersGroupedByCountry(50);

                if (!customers.Success)
                {
                    Console.WriteLine("customersGroupedByCountry returned an error.");
                    tErrorMessage = customers.ErrorMessage;

                    Console.WriteLine($"{tErrorMessage}");
                }
                else if (customers.Success && (customers.Results == null || customers.Results.Count == 0))
                {
                    Console.WriteLine("customersGroupedByCountry returned an error.");
                    tErrorMessage = customers.ErrorMessage;

                    Console.WriteLine($"{tErrorMessage}");
                }
                else
                {
                    var groups = customers.Results
                    .GroupBy(c => string.IsNullOrWhiteSpace(c.Country) ? "(Unknown)" : c.Country.Trim())
                    .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

                    foreach (var group in groups)
                    {
                        Console.WriteLine($"Country: {group.Key} (Count: {group.Count()})");

                        foreach (var customer in group)
                        {
                            Console.WriteLine($"  CustomerID: {customer.CustomerId}, CompanyName: {customer.CompanyName}");
                        }

                        Console.WriteLine();
                    }
                }

                customersObj = null;
            }

            /* console output - exit */
            Console.WriteLine("-------------------------");
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();

            keyInfo = Console.ReadKey();
        }
    }
}
