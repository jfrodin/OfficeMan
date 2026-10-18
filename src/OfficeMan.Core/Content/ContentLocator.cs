namespace OfficeMan.Core.Content;

public static class ContentLocator
{
    /// <summary>
    /// Finds the <c>content</c> directory by walking up from the working directory and the
    /// executable's directory. Lets tools run from anywhere inside the repository.
    /// </summary>
    public static string Find()
    {
        foreach (string start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            for (var dir = new DirectoryInfo(start); dir is not null; dir = dir.Parent)
            {
                string candidate = Path.Combine(dir.FullName, "content");
                if (File.Exists(Path.Combine(candidate, "phases.json"))) return candidate;
            }
        }
        throw new DirectoryNotFoundException("Could not find the content directory (looked for content/phases.json).");
    }
}
