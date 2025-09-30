namespace SudoBox.UnifiedModule.Application.Users.Utils.Password;

public class FileCommonPasswordStore : ICommonPasswordStore
{
    private readonly HashSet<string> _set;

    public FileCommonPasswordStore(string path, bool normalizeToLower = true)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Common password list not found.", path);

        _set = new HashSet<string>(StringComparer.Ordinal);
        foreach (var raw in File.ReadLines(path))
        {
            var s = raw.Trim();
            if (s.Length == 0) continue;
            _set.Add(normalizeToLower ? s.ToLowerInvariant() : s);
        }
    }

    public bool Contains(string candidate)
    {
        if (string.IsNullOrEmpty(candidate)) return false;
        return _set.Contains(candidate.ToLowerInvariant());
    }
}
