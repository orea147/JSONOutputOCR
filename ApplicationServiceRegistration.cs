using Amazon.Textract;
using Microsoft.Extensions.DependencyInjection;
using OcrProject.Application.Interfaces;
using OcrProject.Application.Services;

namespace OcrProject;

public static class ApplicationServiceRegistration
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddScoped<IOcrService, OcrService>();
		services.AddScoped<ISqsService, SqsService>();
		services.AddScoped<ISemanticKernelService, SemanticKernelService>();

		services.AddAWSService<IAmazonTextract>();

		return services;
	}
}
