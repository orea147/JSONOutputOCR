namespace OcrProject.Domain.Models
{
	public class DocumentRequest
	{
		public string FilePath { get; set; }
		public string DocumentType { get; set; } // Optional for future use
	}
}
