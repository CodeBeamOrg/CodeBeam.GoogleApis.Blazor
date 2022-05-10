﻿using System.ComponentModel;

namespace GoogleApis.Blazor
{
    public static class EnumExtensions
    {
        public static string ToDescriptionString(this Enum val)
        {
            var attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0
                ? attributes[0].Description
                : val.ToString();
        }
    }

    public enum Scope
    {
        [Description("https://www.googleapis.com/auth/userinfo.email")]
        OAuth2Email,
        [Description("https://www.googleapis.com/auth/userinfo.profile")]
        OAuth2Profile,
        [Description("https://www.googleapis.com/auth/analytics")]
        Analytics,
        [Description("https://www.googleapis.com/auth/analytics.readonly")]
        AnalyticsReadonly,
        [Description("https://www.googleapis.com/auth/calendar")]
        Calendar,
        [Description("https://www.googleapis.com/auth/calendar.readonly")]
        CalendarReadonly,
        [Description("https://www.googleapis.com/auth/calendar.events")]
        CalendarEvents,
        [Description("https://www.googleapis.com/auth/calendar.events.readonly")]
        CalendarEventsReadonly,
        [Description("https://www.googleapis.com/auth/drive")]
        Drive,
        [Description("https://www.googleapis.com/auth/drive.metadata")]
        DriveReadonly,
        [Description("https://www.googleapis.com/auth/drive.file")]
        DriveFile,
        [Description("https://www.googleapis.com/auth/drive.appdata")]
        DriveAppData,
        [Description("https://www.googleapis.com/auth/drive.metadata")]
        DriveMetaData,
        [Description("https://www.googleapis.com/auth/drive.metadata.readonly")]
        DriveMetaDataReadonly,
    }

    public enum CredentialValueType
    {
        [Description("access_token")]
        AccessToken,
        [Description("refresh_token")]
        RefreshToken,
        [Description("scope")]
        Scope,
        [Description("token_type")]
        TokenType,
        [Description("expires_in")]
        ExpiresIn,
    }

}