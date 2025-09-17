namespace SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

public class BasicConstraintsValue {
    public required Boolean IsCa { get; set; }
    public required int PathLen { get; set; }
}