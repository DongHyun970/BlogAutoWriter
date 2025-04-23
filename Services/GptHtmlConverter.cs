using System.Text;
using System.Text.RegularExpressions;

public static class GptHtmlConverter
{
    public static string ConvertToHtml(string gptOutput)
    {
        var lines = gptOutput.Split('\n');
        var html = new StringBuilder();
        bool inList = false;
        bool inTable = false;
        var sectionContent = new StringBuilder();
        string? currentHeader = null;
        var tableRows = new List<string>();

        foreach (var rawLine in lines)
        {
            string line = rawLine.Trim();

            // 새 섹션 시작
            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                if (currentHeader != null)
                    html.Append(WrapSection(currentHeader, sectionContent.ToString(), inList, inTable, tableRows));

                currentHeader = line.Trim('[', ']');
                sectionContent.Clear();
                inList = false;
                inTable = false;
                tableRows.Clear();
                continue;
            }

            // 리스트 감지
            if (line.StartsWith("- "))
            {
                inList = true;
                tableRows.Clear();
                sectionContent.AppendLine($"<li>{line.Substring(2)}</li>");
                continue;
            }

            // 표 감지
            if (line.Contains("|"))
            {
                inTable = true;
                inList = false;
                tableRows.Add(line);
                continue;
            }

            // 일반 문단
            sectionContent.AppendLine($"<p>{line}</p>");
        }

        // 마지막 섹션 정리
        if (currentHeader != null)
            html.Append(WrapSection(currentHeader, sectionContent.ToString(), inList, inTable, tableRows));

        return html.ToString();
    }

    private static string WrapSection(string title, string content, bool inList, bool inTable, List<string> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<section class=\"baw-section\">");
        sb.AppendLine($"  <h2>{title}</h2>");

        if (inList)
        {
            sb.AppendLine("<ul>");
            sb.Append(content);
            sb.AppendLine("</ul>");
        }
        else if (inTable && rows.Count > 1)
        {
            sb.AppendLine("<table class=\"baw-table\">");

            var headers = rows[0].Split('|').Select(h => h.Trim()).ToArray();
            sb.AppendLine("<thead><tr>" + string.Join("", headers.Select(h => $"<th>{h}</th>")) + "</tr></thead>");
            sb.AppendLine("<tbody>");

            foreach (var row in rows.Skip(1))
            {
                var cols = row.Split('|').Select(c => c.Trim()).ToArray();
                sb.AppendLine("<tr>" + string.Join("", cols.Select(c => $"<td>{c}</td>")) + "</tr>");
            }

            sb.AppendLine("</tbody></table>");
        }
        else
        {
            sb.Append(content);
        }

        sb.AppendLine("</section>");
        return sb.ToString();
    }
}
