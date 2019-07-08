using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace PandeBot.WebHook
{
    public static class GetMessage
    {
        [FunctionName("GetMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            log.LogInformation("Recibiendo mensaje.");

            

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            Update update = JsonConvert.DeserializeObject<Update>(requestBody);

            Database db = new Database(context.FunctionAppDirectory, "database.json", log);
            BotHandler handler = new BotHandler(configuration["BotToken"], db, log);

            handler.Bot_OnMessage(update);

            return new OkObjectResult($"Mensaje procesado");
            //return me.FirstName != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {me.FirstName}")
            //    : new BadRequestObjectResult("Wrong arguments");
        }
    }
}
