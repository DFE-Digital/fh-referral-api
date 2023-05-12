namespace FamilyHubs.Referral.Data.EMailServices;

public class GovNotifySetting
{
    public string APIKey { get; set; } = default!;
    public string TemplateId { get; set; } = default!;
}

public record MessageDto
{
    public required string RecipientEmail { get; set; }
    public string TemplateId { get; set; } = default!;
    public Dictionary<string, string> TemplateTokens { get; set; } = new Dictionary<string, string>();

    public override int GetHashCode()
    {
        int result = 0;
        foreach (var token in TemplateTokens)
        {
            result += EqualityComparer<KeyValuePair<string, string>>.Default.GetHashCode(token);
        }

        result +=
            EqualityComparer<string?>.Default.GetHashCode(RecipientEmail) * -1521134295 +
            EqualityComparer<string?>.Default.GetHashCode(TemplateId) * -1521134295;

        return result;
    }

    public virtual bool Equals(MessageDto? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other))
            return true;

        var keys = TemplateTokens.Select(x => x.Key);

        foreach (var key in keys)
        {
            if (!other.TemplateTokens.ContainsKey(key))
                return false;


            if (!EqualityComparer<string>.Default.Equals(TemplateTokens[key], other.TemplateTokens[key]))
                return false;

        }

        return
            EqualityComparer<string>.Default.Equals(RecipientEmail, other.RecipientEmail) &&
            EqualityComparer<string>.Default.Equals(TemplateId, other.TemplateId)
            ;
    }
}
