using System.Text.RegularExpressions;
using ShareSpaceApi.Data.DTOs;

namespace ShareSpaceApi.Extensions;

public static class ExtractPostTags
{
    public static IEnumerable<string> ExtractTags(this CreatePostDto NewPost)
    {
        string pattern = @"#\w+";
        MatchCollection matches = Regex.Matches(NewPost.TextContent!, pattern);
        foreach (Match match in matches.Cast<Match>())
        {
            yield return match.Value;
        }
    }
}
