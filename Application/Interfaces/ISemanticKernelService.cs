namespace OcrProject.Application.Interfaces
{
	public interface ISemanticKernelService
	{
		Task<string> StructureDataAsync(string extractedText);
	}
}
