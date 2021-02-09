using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using ApiConnect.Models;
using System.Collections.Generic;
using ApiConnect.Utilities;

namespace ApiConnect
{
    class Program
    {
        private const string GLOBAL_DATA_URL = "https://coronavirusapi-france.now.sh/FranceLiveGlobalData";
        private const string DATA_BY_DEPT_URL = "https://coronavirusapi-france.now.sh/LiveDataByDepartement";
        private const string ALL_DEPT_URL = "https://coronavirusapi-france.now.sh/AllLiveData";

        private const string GLOBAL_DATA_TOKEN = "FranceGlobalLiveData";
        private const string ALL_DEP_TOKEN = "allLiveFranceData";
        private const string BY_DEP_TOKEN = "LiveDataByDepartement";

        private static HttpClient client;
        private static Printer printer;

        static void Main(string[] args)
        {
            client = new HttpClient();

            printer = new Printer
            {
                TableWidth = 96
            };

            Console.WriteLine("COVID-19 FRANCE API CLI (version 0.1)");

            Console.WriteLine("Saisir 'france', pour les données globales, 'departement' pour les données par département ou \n'departement' suivi du nom pour les données locales");
            Console.WriteLine("Saisir 'exit' pour quitter.");

            while (true)
            {
                args = Console.ReadLine().Split(' ');
                var command = args[0];
                args = args.Skip(1).ToArray();

                switch (command)
                {
                    case "france":
                        RunAsync(GLOBAL_DATA_URL, GLOBAL_DATA_TOKEN).GetAwaiter().GetResult();
                        break;
                    case "departements":
                        RunAsync(ALL_DEPT_URL, ALL_DEP_TOKEN).GetAwaiter().GetResult();
                        break;
                    case "departement":
                        if (args.Length == 0)
                        {
                            Console.WriteLine("Le nom du département n'a pas été spécifié");
                            break;
                        }

                        var parameters = new Dictionary<string, string>
                        {
                            { "Departement", args[0].TotTitleCase() }
                        };

                        RunAsync(DATA_BY_DEPT_URL, BY_DEP_TOKEN, parameters).GetAwaiter().GetResult();
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            }
        }

        static async Task<string> GetDataAsync(string requestUri)
        {
            var data = string.Empty;

            var response = await client.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return data;
        }

        static async Task RunAsync(string requestUri, string token, Dictionary<string, string> parameters = null)
        {
            var baseUri = requestUri;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    baseUri += $"?{parameters.First().Key}={parameter.Value}";
                }
            }

            var json = await GetDataAsync(baseUri);

            var datas = JObject.Parse(json).SelectToken(token).ToObject<List<Data>>();

            switch (requestUri)
            {
                case GLOBAL_DATA_URL:
                    PrintGlobalData(datas.First());
                    break;
                case ALL_DEPT_URL:
                    PrintAllDeptDatas(datas);
                    break;
                case DATA_BY_DEPT_URL:
                    string departement = string.Empty;
                    parameters.TryGetValue("Departement", out departement);
                    PrintDeptData(datas.First(), departement);
                    break;
                default:
                    Console.WriteLine("Une ereur s'est produite durant l'execution de la requête.");
                    break;
            }
        }

        static void PrintGlobalData(Data data)
        {
            Console.WriteLine("Voici les données globales connues à l'heure actuelle pour la France entière");
            PrintData(data, global: true);
        }

        static void PrintAllDeptDatas(List<Data> datas)
        {
            Console.WriteLine("Voici les données globales connues à l'heure actuelle pour chaques départements");

            foreach (Data data in datas)
            {
                PrintData(data);
            }
        }

        static void PrintDeptData(Data data, string nom)
        {
            if (data == null)
            {
                Console.WriteLine($"Aucun département n'a été trouvé avec le nom \"{nom}\"");
            }
            Console.WriteLine($"Voici les données globales connues à l'heure actuelle pour {nom}");
            PrintData(data);
        }

        static void PrintData(Data data, bool global = false)
        {
            var localization = global ? "Pays" : "Départements";
            printer.line();
            printer.Row("Date", localization, "Décés", "Guéris", "Hospitalisés", "Réanimation");
            printer.line();
            printer.Row(data.Date.ToString("dd/MM/yyyy"), data.Nom, data.Deces, data.Gueris, data.Hospitalises, data.Reanimation);
            printer.line();
        }
    }
}
