using Amazon.Textract;
using Amazon.Textract.Model;
using OcrProject.Application.Interfaces;
using OcrProject.Domain.Models;

namespace OcrProject.Application.Services;

public class OcrService : IOcrService
{
	private readonly IAmazonTextract _textractClient;

	public OcrService(IAmazonTextract textractClient)
	{
		_textractClient = textractClient;
	}

	public async Task<OcrResult> ProcessDocumentAsync(string filePath)
	{
		byte[] bytes;

		if (Uri.IsWellFormedUriString(filePath, UriKind.Absolute))
		{
			using var httpClient = new HttpClient();
			bytes = await httpClient.GetByteArrayAsync(filePath);
		}
		else
		{
			bytes = await File.ReadAllBytesAsync(filePath);
		}

		var detectResponse = await _textractClient.DetectDocumentTextAsync(new DetectDocumentTextRequest
		{
			Document = new Document
			{
				Bytes = new MemoryStream(bytes)
			}
		});

		var ocrResult = new OcrResult();
		foreach (var block in detectResponse.Blocks)
		{
			if (block.BlockType == BlockType.LINE)
			{
				ocrResult.Text += block.Text + "\n";
			}
		}

		return ocrResult;
	}
}
