using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static DTO.DTO;

namespace CSharp_Net8_RESTful_Query.Classes
{
    public class Customers
    {        
        private readonly Settings _settings;

        public Customers(Settings settings)
        {            
            _settings = settings;
        }
        public Response<Customer> customersRetrieve(int maxRows)
        {
            string url = $"{_settings.APIBaseURL}/Customers/entity/customers?rows={maxRows}";

            using (var client = new HttpClient())
            {
                try
                {
                    var httpResponse = client.GetAsync(url).GetAwaiter().GetResult();

                    string json = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    var result = JsonSerializer.Deserialize<Response<Customer>>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    return new Response<Customer>
                    {
                        Success = true,
                        Results = result != null ? result.Results : new List<Customer>() { }
                    };
                }
                catch (Exception ex)
                {
                    return new Response<Customer>()
                    {
                        Success = false,
                        Results = new List<Customer>() { },
                        ErrorMessage = ex.Message
                    };
                }                
            }
        }
        public void customersByCountry(int rows, string country)
        {
            Response<Customer> customers = this.customersRetrieve(rows);

            if (!customers.Success)
            {
                Console.WriteLine(customers.ErrorMessage);
                Console.WriteLine("-------------------------");
                Console.WriteLine("Press any key to continue ...");
                Console.ReadKey();
                return;
            }

            // Si no hay resultados generales
            if (customers.Results == null || customers.Results.Count == 0)
            {
                Console.WriteLine("No customers have been found.");
                Console.WriteLine("-------------------------");
                Console.WriteLine("Press any key to continue ...");
                Console.ReadKey();
                return;
            }

            var filtered = customers.Results
                .Where(c => !string.IsNullOrEmpty(c.Country) &&
                            string.Equals(c.Country, country, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filtered.Count == 0)
            {
                Console.WriteLine($"No customers have been found for country '{country}'.");
            }
            else
            {
                Console.WriteLine(filtered[0].Country);
                Console.WriteLine("-------------");

                foreach (var customer in filtered)
                    Console.WriteLine($"CustomerID: {customer.CustomerId}, CompanyName: {customer.CompanyName}");
            }
        }
        public void customersGroupedByCountry(int rows)
        {
            Response<Customer> customers = this.customersRetrieve(rows);

            if (!customers.Success)
            {
                Console.WriteLine(customers.ErrorMessage);
                Console.WriteLine("-------------------------");
                Console.WriteLine("Press any key to continue ...");
                Console.ReadKey();
                return;
            }

            if (customers.Results == null || customers.Results.Count == 0)
            {
                Console.WriteLine("No customers have been found.");
                Console.WriteLine("-------------------------");
                Console.WriteLine("Press any key to continue ...");
                Console.ReadKey();
                return;
            }

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
    }
}
