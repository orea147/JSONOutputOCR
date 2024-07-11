using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using OcrProject.Application.Interfaces;
using OcrProject.Domain.Models;
using System.Text.Json;

namespace OcrProject.Application.Services;

public class SqsService : ISqsService
{
	private readonly IOcrService _ocrService;
	private readonly ISemanticKernelService _semanticKernelService;

	public SqsService(IOcrService ocrService, ISemanticKernelService semanticKernelService)
	{
		_ocrService = ocrService;
		_semanticKernelService = semanticKernelService;
	}

	public async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
	{
		var documentRequest = JsonSerializer.Deserialize<DocumentRequest>(message.Body);
		var ocrResult = await _ocrService.ProcessDocumentAsync(documentRequest!.FilePath);
		var structuredData = await _semanticKernelService.StructureDataAsync(ocrResult.Text);

		context.Logger.LogInformation($"Processed document {documentRequest.FilePath} with structured result: {structuredData}");
	}
}
