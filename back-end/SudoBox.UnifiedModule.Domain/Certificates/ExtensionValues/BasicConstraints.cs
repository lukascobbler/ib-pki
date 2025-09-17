namespace SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

public class BasicConstraints
{
    public required Boolean IsCa { get; set; }
    public required int PathLen { get; set; }
}