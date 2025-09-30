using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;

namespace SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

public class NamesConstraintsValue {
    public required ListOfNames Permitted { get; set; }
    public required ListOfNames Excluded { get; set; }
}

public static class NameConstraintsExtensions {
    public static NamesConstraintsValue ToNamesConstraintsValue(this NameConstraints nameConstraints) {
        var permitted = new ListOfNames { Value = "" };
        var excluded = new ListOfNames { Value = "" };

        if (nameConstraints.PermittedSubtrees != null) {
            var permittedList = new List<string>();
            foreach (Asn1Encodable enc in nameConstraints.PermittedSubtrees) {
                var gs = GeneralSubtree.GetInstance(enc);
                permittedList.Add(new GeneralNames(gs.Base).ToListOfNames().Value);
            }
            permitted.Value = string.Join(',', permittedList);
        }

        if (nameConstraints.ExcludedSubtrees != null) {
            var excludedList = new List<string>();
            foreach (Asn1Encodable enc in nameConstraints.ExcludedSubtrees) {
                var gs = GeneralSubtree.GetInstance(enc);
                excludedList.Add(new GeneralNames(gs.Base).ToListOfNames().Value);
            }
            excluded.Value = string.Join(',', excludedList);
        }

        return new NamesConstraintsValue {
            Permitted = permitted,
            Excluded = excluded
        };
    }
}