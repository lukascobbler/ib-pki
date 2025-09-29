namespace SudoBox.UnifiedModule.Domain.CRL;

public enum RevocationReason
{
    Unspecified = 0,
    KeyCompromise = 1,
    AffiliationChanged = 2, 
    Superseded = 3,
    CessationOfOperation = 4,
    PrivilegeWithdrawn = 5,
}