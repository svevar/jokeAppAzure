﻿using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using Newtonsoft.Json;

namespace JokeApp
{
    class APIController
    {
        // A Class to control our joke API
        // Define some properties for our class
        public string APIURL;

        // Create constructor to set variables
        public APIController()
        {
            // Let's put the URL in our environment variables to make it easier to update it later if needed
            this.APIURL = System.Environment.GetEnvironmentVariable("RandomJokeAPIURL", EnvironmentVariableTarget.Process);
        }

        public Joke GETRandomJoke()
        {
            // Vi definerer først en ny RestClient med endepunktet vi henter fra miljøvariablen i konstruktøren over
            var client = new RestClient(this.APIURL);

            // Deretter definerer vi en RestRequest som holder detaljene fra spørringen vår
            // En RestRequest hører til en RestClient og får det generelle endepunktet fra denne
            // Det første argumentet er URL-parametre der vi i vårt eksempel ikke trenger noen fordi vi skal rett til base-URLen
            // Det andre argumentet forteller hvilken metode vi benytter
            var request = new RestRequest("", Method.GET);

            // Da kan vi kjøre spørringen vår!
            IRestResponse response = client.Execute(request);

            // La oss hente ut content fra responsen
            var content = response.Content;
            // Og la oss videre opprette en instans av Joke-klassen vår
            Joke joke = new Joke();
            // Til slutt parser vi innholdet fra APIet til formen av av Joke-klassen vår og lagrer dette i joke-variabelen vår
            joke = JsonConvert.DeserializeObject<Joke>(content);
            return joke;
        }
    }
}
