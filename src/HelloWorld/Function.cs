using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloWorld
{
    public class Lowercase
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public Dictionary<string, string> FunctionHandler(APIGatewayProxyRequest proxyEvent)
        {
            return proxyEvent.QueryStringParameters.ToDictionary(
                entry => entry.Key,
                entry => entry.Value.ToLower());
        }
    }


    public class DynamoWriter
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public int FunctionHandler(Dictionary<string, string> data)
        {
            try
            {
                Console.WriteLine(JsonConvert.SerializeObject(data));
                var client = new AmazonDynamoDBClient();
                var table = Table.LoadTable(client, "example_table");
                var document = new Document();
                document["id"] = System.Guid.NewGuid().ToString();
                foreach (var (key, value) in data)
                {
                    document[key] = value;
                }

                var result = table.PutItemAsync(document).Result;
            }
            catch (AmazonDynamoDBException exception)
            {
                Console.WriteLine(string.Concat("Exception while filtering records in DynamoDb table: {0}",
                    exception.Message));
                Console.WriteLine(String.Concat("Error code: {0}, error type: {1}", exception.ErrorCode,
                    exception.ErrorType));
            }
            return 200;
        }
    }
}