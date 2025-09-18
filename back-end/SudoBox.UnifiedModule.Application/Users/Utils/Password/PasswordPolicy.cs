namespace SudoBox.UnifiedModule.Application.Users.Utils.Password;

public interface ICommonPasswordStore { bool Contains(string candidate); }

public class NullCommonPasswordStore : ICommonPasswordStore
{
    public bool Contains(string _) => false;
}

public static class PasswordPolicy
{
    public record Result(bool Ok, List<string> Errors);

    public static Result Evaluate(string password, string email, string? name, string? surname, ICommonPasswordStore common)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(password))
            return new Result(false, new() { "Password is required." });

        if (password.Length < 8) errors.Add("Use at least 8 characters.");
        if (password.Length > 64) errors.Add("Use up to 64 characters.");

        if (common.Contains(password)) errors.Add("This password is too common.");

        return new Result(errors.Count == 0, errors);
    }
}
