using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System.Text.RegularExpressions;

namespace MVCProtectingSecrets.Initializers;

public class LogSanitizerInsightsInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        var traceTelemetry = telemetry as TraceTelemetry;

        if (traceTelemetry != null)
        {
            traceTelemetry.Message = SanitizeString(traceTelemetry.Message);
            // If we don't remove this CustomDimension, the telemetry message will still contain the PII in the "OriginalFormat" property.
            traceTelemetry.Properties.Remove("OriginalFormat");
        }
    }

    public static string SanitizeString(string msg)
    {
        // Sanitize email addresses
        msg = SanitizeEmail(msg);
        // Sanitize connection strings
        msg = SanitizeConnectionStringDetails(msg);
        // Sanitize SAS Tokens
        msg = SanitizeSASToken(msg);
        // Add more sanitization logic here as needed

        //return sanitized string
        return msg;
    }

    private static string SanitizeEmail(string msg)
    {
        var regexEmail = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        var replacedEmail = "[emailaddress]";
        return Regex.Replace(msg, regexEmail, replacedEmail);
    }

    private static string SanitizeConnectionStringDetails(string msg)
    {
        var regexSQLConnectionString = @"(?i)[a-z][a-z0-9-]+\.database(?:\.secure)?\.(?:(?:windows|usgovcloudapi)\.net|chinacloudapi\.cn|cloudapi\.de)";
        var regexInitialCatalog = @"Initial Catalog=[a-zA-Z0-9-]*";
        var regexUserID = @"User ID=[a-zA-Z0-9-_]*";
        var regexPassword = @"Password=[a-zA-Z0-9-!@#$%^&*()_]*";

        var cnstrReplaced = "[redacted-server]";
        var catReplace = "Intiial Catalog=[redacted-db]";
        var userIDReplace = "User ID=[redacted-user]";
        var passwordReplace = "Password=[redacted-password]";

        msg = Regex.Replace(msg, regexSQLConnectionString, cnstrReplaced);
        msg = Regex.Replace(msg, regexInitialCatalog, catReplace);
        msg = Regex.Replace(msg, regexUserID, userIDReplace);
        msg = Regex.Replace(msg, regexPassword, passwordReplace);
        return msg;
    }

    private static string SanitizeSASToken(string msg)
    {
        var regexSASToken = @"sig=[a-zA-Z0-9%]*";
        var sasReplaced = "[sastoken]";
        msg = Regex.Replace(msg, regexSASToken, sasReplaced);
        return msg;
    }
}
