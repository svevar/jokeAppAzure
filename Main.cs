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

            // Kj�r metoden for � gj�re sp�rring til APIet
            //var joke = new Joke();
            var joke = new Joke();
            Boolean acceptedJoke = false;
            int retries = 0;
            int maxRetry = 10;
            while (!acceptedJoke && retries < maxRetry)
            {
                // Kj�r metoden for � gj�re sp�rring til APIet
                // JokeAPI.GETRandomJoke();
                joke = JokeAPI.GETRandomJoke();
                acceptedJoke = joke.ValidateJoke();
                retries += 1;
            }
            //if (!acceptedJoke)
            //{
            //    // Hvis vi bruker mer enn maxRetry fors�k p� � finne en godkjent vits avslutter vi kj�ringen
            //    log.LogInformation($"After {maxRetry} subsequent tries, function couldn't retrieve a joke that passed validation.");
            //    return;
            //}

            // Logger ut vitsen til konsoll:
            log.LogInformation($"Her kommer en vits!");
            log.LogInformation($"{joke.Setup}");
            log.LogInformation($"{joke.Punchline}");

            // Vi henter connectionString til databasen fra milj�variabler
            string ConnectionString = System.Environment.GetEnvironmentVariable("SQLConnectionString", EnvironmentVariableTarget.Process);
            //log.LogInformation($"V�r connection string: {ConnectionString}");

            // Oppretter tilkobling til databasen vha. System.Data.SqlClient og �pner denne
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

            // Definerer sp�rringen v�r og kj�rer denne
            con.Execute("insert into Jokes2 (Id, JokeType, Setup, Punchline) values (@Id, @Type, @Setup, @Punchline)",
                joke);
            }
        }
    }
}
