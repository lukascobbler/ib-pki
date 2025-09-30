namespace SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

using Org.BouncyCastle.Asn1.X509;
using System.Text;

public class ListOfNames {
    public required string Value { get; set; }

    private static readonly Dictionary<string, int> nameTypeMap = new(StringComparer.OrdinalIgnoreCase) {
        { "other", GeneralName.OtherName },
        { "email", GeneralName.Rfc822Name },
        { "dns", GeneralName.DnsName },
        { "uri", GeneralName.UniformResourceIdentifier },
        { "ip", GeneralName.IPAddress }
    };

    private IEnumerable<GeneralName> ParseGeneralNames() {
        var names = Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var name in names) {
            var parts = name.Split(':', 2);
            if (parts.Length < 2)
                throw new ArgumentException($"Invalid name format: {name}");

            var key = parts[0].Trim();
            if (!nameTypeMap.TryGetValue(key, out var nameType))
                throw new ArgumentException($"Unknown general name type: {key}");

            yield return new GeneralName(nameType, parts[1]);
        }
    }

    public GeneralName[] ToGeneralNames() => [.. ParseGeneralNames()];
    public IList<GeneralSubtree> ToGeneralSubtrees() => [.. ParseGeneralNames().Select(name => new GeneralSubtree(name))];
}

public static class GeneralNamesExtensions {
    public static ListOfNames ToListOfNames(this GeneralNames generalNames) {
        var sb = new StringBuilder();

        foreach (var gn in generalNames.GetNames()) {
            string prefix = gn.TagNo switch {
                GeneralName.OtherName => "other",
                GeneralName.Rfc822Name => "email",
                GeneralName.DnsName => "dns",
                GeneralName.UniformResourceIdentifier => "uri",
                GeneralName.IPAddress => "ip",
                _ => throw new ArgumentException($"Unsupported GeneralName tag: {gn.TagNo}")
            };

            if (sb.Length > 0)
                sb.Append(',');

            sb.Append(prefix).Append(':').Append(gn.Name.ToString());
        }

        return new ListOfNames { Value = sb.ToString() };
    }
}