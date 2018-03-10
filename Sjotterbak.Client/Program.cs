using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RestSharp;
using Sjotterbak.WebApi.Controllers;

namespace Sjotterbak.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var timmahy = AddPlayer("Tim Mahy");
            var timvermeulen = AddPlayer("Tim Vermeulen");
            var chris = AddPlayer("Chris");
            var nick = AddPlayer("Nick");
            var malik = AddPlayer("Malik");
            var tomcools = AddPlayer("Tom Cools");
            var jochem = AddPlayer("Jochem");
            var robby = AddPlayer("Robby");
            var jeremy = AddPlayer("Jeremy");
            var toonvermeulen = AddPlayer("Toon Vermeulen");
            var dieter = AddPlayer("Dieter");
            var glenn = AddPlayer("Glenn");
            var arne = AddPlayer("Arne");

            Game(timmahy, timvermeulen, chris, nick, 16, 14);
            Game(timmahy, malik, tomcools, jochem, 15, 13);
            Game(timmahy, nick, tomcools, robby, 11, 6);
            Game(jeremy, nick, malik, jochem, 11, 4);
            Game(nick, chris, toonvermeulen, robby, 11, 3);
            Game(toonvermeulen, robby, chris, nick, 4, 11);
            Game(chris, jochem, jeremy, robby, 11, 3);
            Game(timmahy, jeremy, timvermeulen, malik, 13, 14);
            Game(chris, malik, jeremy, tomcools, 11, 6);
            Game(jeremy, malik, chris, tomcools, 3, 11);
            Game(chris, jochem, jeremy, nick, 12, 10);
            Game(jochem, chris, timmahy, malik, 11, 8);
            Game(timmahy, jochem, glenn, dieter, 13, 11);
            Game(chris, nick, glenn, dieter, 11, 6);
            Game(timmahy, chris, dieter, jochem, 6, 11);
            Game(jeremy, toonvermeulen, tomcools, arne, 5, 11);
            Game(timmahy, nick, tomcools, arne, 11, 4);
            Game(timmahy, nick, jeremy, jochem, 11, 1);
            Game(timmahy, dieter, chris, nick, 5, 11);
            Game(toonvermeulen, robby, chris, nick, 3, 11);
            Game(nick, jochem, chris, jeremy, 5, 11);
        }

        private static string AddPlayer(string s)
        {
            return s;
        }

        static void Game(string blue1, string blue2, string red1, string red2, int scoreBlue, int scoreRed)
        {
            var client = new RestClient("http://sjotterbak.azurewebsites.net/api");

            var request = new RestRequest("/Games/", Method.POST);

            var newGame = new GamesController.Game()
            {
                ScoreRed = scoreRed,
                ScoreBlue = scoreBlue,
                Red = new GamesController.Team()
                {
                    NamePlayerAttacker = red2,
                    NamePlayerKeeper = red1,
                },
                Blue = new GamesController.Team()
                {
                    NamePlayerAttacker = blue2,
                    NamePlayerKeeper = blue1
                }
            };
            string jsonToSend = Newtonsoft.Json.JsonConvert.SerializeObject(newGame);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            var response = client.Execute(request);
            if (response.IsSuccessful == false)
                throw new NotSupportedException(response.ErrorMessage);
        }
    }
}
