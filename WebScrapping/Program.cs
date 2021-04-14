using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebScrapping;

namespace WebScrappings
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            await WebScrapping.ScrapClass.ScrapLinksMethod();
        }

    }
}
