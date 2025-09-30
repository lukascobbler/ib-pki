namespace SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

public class BasicConstraintsValue {
    public required bool IsCa { get; set; }
    public required int? PathLen { get; set; }
}