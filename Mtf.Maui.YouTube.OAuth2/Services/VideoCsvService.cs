using Mtf.Maui.YouTube.OAuth2.Models;
using System.Text;

namespace Mtf.Maui.YouTube.OAuth2.Services;

public static class VideoCsvService
{
    private const char CsvDelimiter = ';';

    public static Task SaveVideosToCsvAsync(IEnumerable<Video> videos, string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Title{CsvDelimiter}Description{CsvDelimiter}VideoId{CsvDelimiter}VideoType{CsvDelimiter}Liked{CsvDelimiter}Image");
        foreach (var video in videos)
        {
            var line = new[]
            {
                EscapeCsvField(video.Title),
                EscapeCsvField(video.Description),
                EscapeCsvField(video.VideoId),
                EscapeCsvField(video.VideoType),
                video.Liked.ToString(),
                EscapeCsvField(video.Image)
            };
            sb.AppendLine(String.Join(CsvDelimiter, line));
        }

        return File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
    }

    public static async Task<IEnumerable<Video>> ReadVideosFromCsvAsync(string filePath)
    {
        var videos = new List<Video>();
        if (!File.Exists(filePath))
        {
            return videos.AsEnumerable();
        }

        var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8).ConfigureAwait(false);

        foreach (var line in lines.Skip(1))
        {
            if (String.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var fields = ParseCsvLine(line);

            if (fields.Count == 6)
            {
                try
                {
                    var video = new Video
                    {
                        Title = fields[0],
                        Description = fields[1],
                        VideoId = fields[2],
                        VideoType = fields[3],
                        Liked = Boolean.TryParse(fields[4], out bool liked) && liked,
                        Image = fields[5]
                    };
                    videos.Add(video);
                }
                catch (Exception ex)
                {
                    ErrorDisplayer.ShowError(ex);
                }
            }
        }

        return videos.AsEnumerable();
    }

    private static string EscapeCsvField(string field)
    {
        if (String.IsNullOrEmpty(field))
        {
            return "\"\"";
        }

        if (field.Contains(CsvDelimiter, StringComparison.CurrentCultureIgnoreCase) ||
            field.Contains('"', StringComparison.CurrentCultureIgnoreCase) ||
            field.Contains('\n', StringComparison.CurrentCultureIgnoreCase) ||
            field.Contains('\r', StringComparison.CurrentCultureIgnoreCase))
        {
            return $"\"{field.Replace("\"", "\"\"", StringComparison.CurrentCultureIgnoreCase)}\"";
        }

        return field;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var currentField = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentField.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    currentField.Append(c);
                }
            }
            else
            {
                if (c == CsvDelimiter)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else if (c == '"')
                {
                    inQuotes = true;
                }
                else
                {
                    currentField.Append(c);
                }
            }
        }

        fields.Add(currentField.ToString());
        return fields;
    }
}