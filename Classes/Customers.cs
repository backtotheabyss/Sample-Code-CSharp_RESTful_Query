using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
        public Response<Customer> customersByCountry(int rows, string country)
        {
            Response<Customer> customers = this.customersRetrieve(rows);

            if (!customers.Success)
            {
                return new Response<Customer>
                {
                    Success = false,
                    ErrorMessage = customers.ErrorMessage ?? "Error retrieving customers.",
                    Results = new List<Customer>()
                };
            }

            if (customers.Results == null || customers.Results.Count == 0)
            {
                return new Response<Customer>
                {
                    Success = false,
                    ErrorMessage = "No customers have been found.",
                    Results = new List<Customer>()
                };
            }

            var result = customers.Results
                .Where(c => !string.IsNullOrEmpty(c.Country) &&
                            string.Equals(c.Country, country, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return new Response<Customer>
            {
                Success = true,
                Results = result,
                TotalCount = result.Count
            };
        }
        public Response<Customer> customersGroupedByCountry(int rows)
        {
            Response<Customer> customers = this.customersRetrieve(rows);

            if (!customers.Success)
            {
                return new Response<Customer>
                {
                    Success = false,
                    ErrorMessage = customers.ErrorMessage ?? "Error retrieving customers.",
                    Results = new List<Customer>()
                };
            }

            if (customers.Results == null || customers.Results.Count == 0)
            {
                return new Response<Customer>
                {
                    Success = false,
                    ErrorMessage = "No customers have been found.",
                    Results = new List<Customer>()
                };
            }

            var groups = customers.Results
                .GroupBy(c => string.IsNullOrWhiteSpace(c.Country) ? "(Unknown)" : c.Country.Trim())
                .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
                .SelectMany(g => g)
                .ToList(); 

            return new Response<Customer>
            {
                Success = true,
                Results = groups,
                TotalCount = groups.Count
            };
        }
    }
}
