using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OcrProject.Application.Interfaces;
using System.Text.Json;

namespace OcrProject.Application.Services;

public class SemanticKernelService : ISemanticKernelService
{
	private readonly string _apiKey;
	private readonly string _chatModelId;

	public SemanticKernelService(IConfiguration configuration)
	{
		_apiKey = configuration["OpenAI:ApiKey"];
		_chatModelId = configuration["OpenAI:ChatModelId"];

		if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_chatModelId))
		{
			throw new ArgumentNullException("API Key and Chat Model ID must be provided.");
		}
	}

	public async Task<string> StructureDataAsync(string extractedText)
	{
		Kernel kernel = Kernel.CreateBuilder()
			.AddOpenAIChatCompletion(modelId: _chatModelId, apiKey: _apiKey)
			.Build();

		var jsonSchema = new
		{
			title = "PersonInformation",
			description = "Extracted person information from text.",
			type = "object",
			properties = new
			{
				name = new { title = "Name", description = "The person's name", type = "string" },
				affiliation = new { title = "Affiliation", description = "The person's parents, family", type = "string" },
				birth_date = new { title = "Birth Date", description = "The person's birth date (dd/mm/yyyy)", type = "string" },
				cpf = new { title = "CPF", description = "CPF in the format xxx.xxx.xxx-xx", type = "string" },
				registration_number = new { title = "Registration Number", description = "Registration number with 11 digits (xxxxxxxxxx-x)", type = "string" },
				place_of_birth = new { title = "Place of Birth", description = "Place of birth in the format city - state", type = "string" }
			},
			required = new[] { "name", "affiliation", "birth_date", "cpf", "registration_number", "place_of_birth" }
		};

		string schemaStr = JsonSerializer.Serialize(jsonSchema, new JsonSerializerOptions { WriteIndented = true });

		string prompt = $"Please use the following JSON schema to extract information from the provided text. Schema: {schemaStr}\nText: {extractedText}";

		string chatPrompt = $"""
                <message role="system">You are a helpful assistant designed to output JSON.</message>
                <message role="user">{prompt}</message>
                """;

		var result = await kernel.InvokePromptAsync(chatPrompt);

		return result.ToString();
	}
}
