using DLR.BOM.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Configuration;

namespace FunctionApp1
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("EDADemo")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            DataAccessProvider dataAccessProvider = new DataAccessProvider();

            int page = int.TryParse(req.Query["page"], out var parsedPage) ? parsedPage : 1;
            int pageSize = int.TryParse(req.Query["pageSize"], out var parsedPageSize) ? parsedPageSize : 10;

            if (page < 1 || pageSize < 1)
            {
                return new BadRequestObjectResult("Invalid page or pageSize.");
            }
            List<dynamic> result = dataAccessProvider.GetItems<dynamic>($"SELECT * FROM DMA.VW_SUITE_LEVEL_SQFT ORDER BY REGION, MASTER_SITE_CODE, SUITE offset 1 FETCH NEXT 10 ROWS ONLY;", null, false).ToList();
            return new OkObjectResult(result10)
        }
    }
}
