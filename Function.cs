using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OcrProject.Application.Interfaces;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace OcrProject
{
	public class Function
	{
		private readonly IServiceProvider _serviceProvider;

		public Function()
		{
			var serviceCollection = new ServiceCollection();
			var configuration = LoadConfiguration();
			serviceCollection.AddSingleton<IConfiguration>(configuration);
			serviceCollection.AddApplicationServices();
			_serviceProvider = serviceCollection.BuildServiceProvider();
		}

		private IConfiguration LoadConfiguration()
		{
			var configurationBuilder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

			return configurationBuilder.Build();
		}

		public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
		{
			var sqsService = _serviceProvider.GetRequiredService<ISqsService>();

			foreach (var message in evnt.Records)
			{
				try
				{
					await sqsService.ProcessMessageAsync(message, context);
				}
				catch (JsonException jsonEx)
				{
					context.Logger.LogError($"Error deserializing message body: {message.Body}");
					context.Logger.LogError(jsonEx.ToString());
				}
				catch (Exception ex)
				{
					context.Logger.LogError($"Error processing message: {message.Body}");
					context.Logger.LogError(ex.ToString());
				}
			}
		}
	}
}
