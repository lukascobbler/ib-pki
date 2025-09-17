using System.Numerics;

namespace SudoBox.UnifiedModule.Domain.Certificates;

public class Certificate
{
    public required BigInteger SerialNumber { get; set; }
    public required string IssuedBy { get; set; }
    public required string IssuedTo { get; set; }
    public required DateOnly ValidFrom { get; set; }
    public required DateOnly ValidUntil { get; set; }
    public string? EncodedValue { get; set; }
    public required bool IsApproved { get; set; }
}