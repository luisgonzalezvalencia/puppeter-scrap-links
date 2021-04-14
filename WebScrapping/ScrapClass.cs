using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapping
{
    public class ScrapClass
    {
        public static async Task ScrapLinksMethod()
        {
            try
            {
                var options = new LaunchOptions { Headless = false };
                Console.WriteLine("Descargando chromium para test...");
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

                //solicitamos el sitio al usuario
                //TODO: validaciones del sitio ingresado
                Console.WriteLine("Ingrese el sitio que desea checkear con el protocolo http:// o https:// ...");
                var site = Console.ReadLine();
                Console.WriteLine($"Navegando a {site}");

                using (var browser = await Puppeteer.LaunchAsync(options))
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(site);
                    var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
                    var urls = await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
                    urls = urls.Distinct().ToArray();
                    Console.WriteLine($"Se obtuveron {urls.Length} links distintos.");
                    Console.WriteLine($"Verificando que no estén rotos. Aguarde por favor...");
                    foreach (string url in urls)
                    {
                        var sitioRespondio = await TestIfSiteAliveAsync(url);
                        if (!sitioRespondio)
                        {
                            Console.WriteLine($"El link del sitio: {url}, no responde. Verifique si está roto.");
                        }

                    }
                    Console.WriteLine("Finalizado el proceso. Presione una tecla para continuar...");
                    Console.ReadLine();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hubo un error {ex}");
                Console.WriteLine("Finalizado el proceso. Presione una tecla para continuar...");
            }

        }

        public static async Task<bool> TestIfSiteAliveAsync(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                var checkingResponse = await client.GetAsync(url);
                if (!checkingResponse.IsSuccessStatusCode)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }


    public class Rot13
    {
        private Dictionary<char, char> rot13 = new Dictionary<char, char>();

        /// <summary>
        /// Initialise the dictionary on a per object basis. I guess it could be made static as well?!
        /// </summary>
        public Rot13()
        {
            string lowLower = "abcdefghijklm", highLower = "nopqrstuvwxyz";
            string lowUpper = "ABCDEFGHIJKLM", highUpper = "NOPQRSTUVWXYZ";

            for (int i = 0; i < lowUpper.Length; i++)
            {
                // Convert a => n and A => N.
                rot13.Add(lowLower[i], highLower[i]);
                rot13.Add(highLower[i], lowLower[i]);

                // Convert n => a and N => A.
                rot13.Add(lowUpper[i], highUpper[i]);
                rot13.Add(highUpper[i], lowUpper[i]);
            }
        }

        /// <summary>
        /// Decode a Rot13 string.
        /// </summary>
        /// <param name="data">A Rot13 encoded string.</param>
        /// <returns>The original string.</returns>
        public string Decode(string data)
        {
            return Encode(data);
        }

        /// <summary>
        /// Encode a string to using Rot13.
        /// </summary>
        /// <param name="data">A string to be encoded.</param>
        /// <returns>An encoded string.</returns>
        public string Encode(string data)
        {
            char rotated = new char();
            char[] array = data.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (rot13.TryGetValue(array[i], out rotated))
                {
                    array[i] = rotated;
                }
            }
            return new string(array);
        }
    }

}
