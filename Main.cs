using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Dapper;

namespace JokeApp
{
    public static class Main
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            APIController JokeAPI = new APIController();
            log.LogInformation($"API URL is: {JokeAPI.APIURL}");

            // Kjør metoden for å gjøre spørring til APIet
            //var joke = new Joke();
            var joke = new Joke();
            Boolean acceptedJoke = false;
            int retries = 0;
            int maxRetry = 10;
            while (!acceptedJoke && retries < maxRetry)
            {
                // Kjør metoden for å gjøre spørring til APIet
                // JokeAPI.GETRandomJoke();
                joke = JokeAPI.GETRandomJoke();
                acceptedJoke = joke.ValidateJoke();
                retries += 1;
            }
            //if (!acceptedJoke)
            //{
            //    // Hvis vi bruker mer enn maxRetry forsøk på å finne en godkjent vits avslutter vi kjøringen
            //    log.LogInformation($"After {maxRetry} subsequent tries, function couldn't retrieve a joke that passed validation.");
            //    return;
            //}

            // Logger ut vitsen til konsoll:
            log.LogInformation($"Her kommer en vits!");
            log.LogInformation($"{joke.Setup}");
            log.LogInformation($"{joke.Punchline}");

            // Vi henter connectionString til databasen fra miljøvariabler
            string ConnectionString = System.Environment.GetEnvironmentVariable("SQLConnectionString", EnvironmentVariableTarget.Process);
            //log.LogInformation($"Vår connection string: {ConnectionString}");

            // Oppretter tilkobling til databasen vha. System.Data.SqlClient og åpner denne
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

            // Definerer spørringen vår og kjører denne
            con.Execute("insert into Jokes2 (Id, JokeType, Setup, Punchline) values (@Id, @Type, @Setup, @Punchline)",
                joke);
            }
        }
    }
}
