namespace SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

public class NamesConstraintsValue {
    public required ListOfNames Permitted { get; set; }
    public required ListOfNames Excluded { get; set; }
}