using MudBlazor;
using System.Text;

namespace BoardGameTracker.Client.Extensions;

public static class SnackbarExtensions
{
    public static void ShowError(this ISnackbar snackbar, string message, Severity severity = Severity.Error)
    {
        var str = message;

        if (message.StartsWith("["))
        {
            // This is a list of errors.... parse this
            var temp = message.Trim(new char[] { '[', ']' });
            var sb = new StringBuilder("<ul>");
            temp.Split(",", StringSplitOptions.TrimEntries)
                .ToList()
                .ForEach(e => sb.Append($"<li>{e}</li>"));
            sb.Append("</ul>");
            str = sb.ToString();
        }

        snackbar.Add(str, severity);
    }
}
