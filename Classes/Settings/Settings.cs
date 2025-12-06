using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace CSharp_Net8_RESTful_Query
{
    public class Settings
    {
        /*  settings */
        public string settingName1 { get; set; } = string.Empty;
        public string settingName2 { get; set; } = string.Empty;
        public string APIBaseURL { get; set; } = string.Empty;
        
        /* end settings */
        private readonly IConfiguration _configuration;

        public Settings(IConfiguration configuration)
        {
            _configuration = configuration;

            /* settings */
            settingName1 = _configuration["settingName1"] ?? string.Empty;
            settingName2 = _configuration["settingName2"] ?? string.Empty;
            APIBaseURL = _configuration["APIBaseURL"] ?? string.Empty;
            /* end settings */

            /* variables - initialization */
            //List<PrinterConfiguration> test = new List<PrinterConfiguration>()
            //    { new PrinterConfiguration { IP = String.Empty },
            //        new PrinterConfiguration { IP = String.Empty}
            //};
            //PrinterConfiguration x = new PrinterConfiguration() { Name = "Canon MF240 Serices PCL6 V4", IP = "172.22.1.242", Port = 9100, web_interface_url = "portal_top.html", health_url = "", health_regex = "" };
            //List<string> y = new List<string>() { };
            //string[] z = new string[] { };
        }
    }
}