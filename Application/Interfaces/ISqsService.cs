using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

namespace OcrProject.Application.Interfaces
{
	public interface ISqsService
	{
		Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context);
	}
}
