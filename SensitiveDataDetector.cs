using System.Text.RegularExpressions;

namespace GeniaClipboard;

internal static partial class SensitiveDataDetector
{
    public static bool LooksSensitive(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var candidate = text.Trim();
        if (candidate.Length > 8192)
        {
            candidate = candidate[..8192];
        }

        return JwtRegex().IsMatch(candidate)
               || GithubTokenRegex().IsMatch(candidate)
               || OpenAiStyleTokenRegex().IsMatch(candidate)
               || AwsAccessKeyRegex().IsMatch(candidate)
               || BearerTokenRegex().IsMatch(candidate)
               || PasswordAssignmentRegex().IsMatch(candidate)
               || PrivateKeyHeaderRegex().IsMatch(candidate);
    }

    [GeneratedRegex(@"\beyJ[A-Za-z0-9_-]{5,}\.[A-Za-z0-9_-]{5,}\.[A-Za-z0-9_-]{5,}\b", RegexOptions.CultureInvariant)]
    private static partial Regex JwtRegex();

    [GeneratedRegex(@"\b(?:ghp|gho|ghu|ghs|github_pat)_[A-Za-z0-9_]{20,}\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex GithubTokenRegex();

    [GeneratedRegex(@"\bsk-[A-Za-z0-9_-]{20,}\b", RegexOptions.CultureInvariant)]
    private static partial Regex OpenAiStyleTokenRegex();

    [GeneratedRegex(@"\b(?:AKIA|ASIA)[A-Z0-9]{16}\b", RegexOptions.CultureInvariant)]
    private static partial Regex AwsAccessKeyRegex();

    [GeneratedRegex(@"\bBearer\s+[A-Za-z0-9._~+/-]{20,}=*\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex BearerTokenRegex();

    [GeneratedRegex(@"\b(?:password|passwd|pwd|secret|api[_ -]?key|access[_ -]?token)\s*[:=]\s*\S{8,}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex PasswordAssignmentRegex();

    [GeneratedRegex(@"-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex PrivateKeyHeaderRegex();
}
