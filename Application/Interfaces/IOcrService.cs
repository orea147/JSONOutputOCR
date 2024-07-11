using OcrProject.Domain.Models;

namespace OcrProject.Application.Interfaces
{
	public interface IOcrService
	{
		Task<OcrResult> ProcessDocumentAsync(string filePath);
	}
}
