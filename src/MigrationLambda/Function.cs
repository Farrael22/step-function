using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using MigrationLambda.Models;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MigrationLambda
{
    public class Function
    {
        private const int BULK_CALL_NUMBER = 2;
        private const int EXECUTION_TIME_BREAKER = 120000;

        public async Task<GetDataResponse> FunctionHandler(RequestParameter input, ILambdaContext context)
        {
            LambdaLogger.Log($"Lambda input parameters {JsonConvert.SerializeObject(input)}");

            var getDataResponse = new GetDataResponse
            {
                RemainingFilesCount = 0
            };

            // Files from legacy systems
            var files = new List<string> 
            {
                "file1",
                "file2",
                "file3",
                "file4",
            };

            var startIndex = 0;
            
            // Check if we are receiving last execution status on Lambda Input
            if (input.GetDataResponse != null)
            {
                // Calculate start index of the current execution
                startIndex = files.Count - input.GetDataResponse.RemainingFilesCount;
            }

            // Iterate over files skiping the ones already done
            for (var currentIndex = startIndex; currentIndex < files.Count; currentIndex += BULK_CALL_NUMBER)
            {
                // Check if Lambda execution remaining time is enough to do one more bulk execution
                if (context.RemainingTime < TimeSpan.FromMilliseconds(EXECUTION_TIME_BREAKER))
                {
                    // Break execution to no face timeout problem and return current status to be continued
                    var remainingFiles = files.Count - currentIndex;
                    
                    getDataResponse.RemainingFilesCount = remainingFiles <= 0 ? 0 : remainingFiles;
                    return getDataResponse;
                }

                // Bulk executions
                var tasks = files.Skip(currentIndex).Take(BULK_CALL_NUMBER).Select(async file =>
                {
                    LambdaLogger.Log($"Do stuffs for file {file}");
                });
                await Task.WhenAll(tasks);
            }

            getDataResponse.RemainingFilesCount = 0;
            return getDataResponse;
        }
    }
}
