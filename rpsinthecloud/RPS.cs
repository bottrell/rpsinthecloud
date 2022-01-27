using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// The goal is to recieve a GET request through HTTP containing either "Rock", "Paper", or "Scissors"
// And write that information to a database for visualization later on my website
// The app will respond with a random move and a message that they either won or lost

namespace rpsinthecloud
{
    public static class RPS
    {
        [FunctionName("rpsinthecloud")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            //logging to application insights
            log.LogInformation("rps in the cloud has been called");

            //Serialize the response into a usable object
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            
            //data is accessible like a .Net object, therefore we can use the . operator to access the move property
            string userMove = data.move;
            //string userMove = data["move"];

            //Choose a move to send back to the user
            string computerMove = MakeMove(log);

            string responseMessage = $"You selected {userMove}! I selected {computerMove}, You {WhoWon(userMove, computerMove)}!";

            return new OkObjectResult(responseMessage);
        }
        
        public static string WhoWon(string uMove, string cMove)
        {
            uMove = uMove.ToLower();
            cMove = cMove.ToLower();
            if (uMove == cMove) { return "tie";  };
            if (uMove == "rock" && cMove == "paper") { return "lose"; }
            if (uMove == "paper" && cMove == "scissors") { return "lose"; }
            if (uMove == "scissors" && cMove == "rock") { return "lose"; }
            return "win";
        }

        public static string MakeMove(ILogger l)
        {
            Random random = new Random();
            int move = random.Next(1,4);
            l.LogInformation($"The computer selected {move}");
            switch (move)
            {
                case 1:
                    return "Rock";
                case 2:
                    return "Paper";
                case 3:
                    return "Scissors";
                default:
                    break;
            };
            return "None";
        }
    }
}
