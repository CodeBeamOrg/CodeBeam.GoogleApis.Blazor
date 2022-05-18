# CodeBeam.GoogleApis.Blazor
## An open-source utility package for GoogleApis in Blazor. 
## Always aim for the easiest to use.
[![GitHub Repo stars](https://img.shields.io/github/stars/codebeamorg/codebeam.googleapis.blazor?color=594ae2&style=flat-square&logo=github)](https://github.com/codebeamorg/codebeam.googleapis.blazor/stargazers)
[![GitHub last commit](https://img.shields.io/github/last-commit/codebeamorg/codebeam.googleapis.blazor?color=594ae2&style=flat-square&logo=github)](https://github.com/codebeamorg/codebeam.googleapis.blazor)
[![Contributors](https://img.shields.io/github/contributors/codebeamorg/codebeam.googleapis.blazor?color=594ae2&style=flat-square&logo=github)](https://github.com/codebeamorg/codebeam.googleapis.blazor/graphs/contributors)
[![Nuget version](https://img.shields.io/nuget/v/CodeBeam.GoogleApis.Blazor?color=ff4081&label=nuget%20version&logo=nuget&style=flat-square)](https://www.nuget.org/packages/codebeam.googleapis.blazor/)
[![Nuget downloads](https://img.shields.io/nuget/dt/CodeBeam.GoogleApis.Blazor?color=ff4081&label=nuget%20downloads&logo=nuget&style=flat-square)](https://www.nuget.org/packages/codebeam.googleapis.blazor/)

This repo is still in earlier stage of development, but all completed parts already tested with real world applications.

## Documentation
Please look at examples.


## Contributing
Welcome to all contributors!


## Usage
### Preliminary: Set your google account and your project credentials
Set your google client. This process has no special thing with this library. [Read the details.](https://developers.google.com/identity/gsi/web/guides/get-google-api-clientid)
<br /> Add the credential into appsettings.json file like the below example. (Not obligatory, but ClientId, ClientSecret and RedirectUrl is required for some features to use this library simpler.)
```json
{
  "GoogleClient": {
    "client_id": "[CLIENT_ID]",
    "project_id": "",
    "auth_uri": "",
    "token_uri": "",
    "auth_provider_x509_cert_url": "",
    "client_secret": "[CLIENT_SECRET]",
    "redirect_url": "[REDIRECT_URL]"
  }
}
```
### Step 1: Inject Services

Add Services into program.cs
```cs
builder.Services.AddCodeBeamGoogleApiServices();
```

For blazor component files
```razor
@inject AuthService AuthService
@inject CalendarService CalendarService
```
OR - For cs files
```cs
[Inject] AuthService AuthService { get; set; }
[Inject] CalendarService CalendarService { get; set; }
```

### Step 2: Implement whatever you want
```cs
private void CreateCalendar()
{
    GoogleCalendarListModel googleCalendarListModel = new GoogleCalendarListModel()
    {
        summary = "Test Calendar",
        description = "Created By CodeBeam",
        timeZone = "Europe/Istanbul",
    };
    CalendarService.AddCalendar(googleCalendarListModel);
}
```

### Step 3: null. That's all.

# Examples
## How To Use Auth Service To Get Permission From a Google Account
#### Step 1. Request Authorization Code
```cs
private async Task RequestCode()
{
    List<Scope> scopes = new();
    scopes.Add(Scope.OAuth2Email); //Not required, but useful for get user email in future.
    scopes.Add(Scope.Calendar);
    await AuthService.RequestAuthorizationCode(AuthService.GetClientId(), scopes, NavigationManager.BaseUri + "v1/browser-callback"); //"v1/browser-callback" is your page that method returns and opens the page as a new tab
}
```
#### Step 2. Get Access Token With Obtained Authorization Code in Step 1
These codes should be in the callback page.
<br /> Blazor Component
```razor
@page "/v1/browser-callback"
```
Code
```cs
[Parameter]
[SupplyParameterFromQuery]
public string Code { get; set; }

string _accessToken = "";
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        var request = AuthService.AuthorizeCredential(Code, AuthService.GetCliendId(), AuthService.GetClientSecret(), NavigationManager.BaseUri + "v1/browser-callback");
        // This method is built in this package. You also can write your own method to process request result.
        _accessToken = AuthService.GetValueFromCredential(request, CredentialValueType.AccessToken);
    }
}
```

#### Step 3. You can store this access token (can also obtain refresh token etc. with same way) in your database or make a cookie. You can use this token to call calendar, drive etc. features.


## How To Create a Calendar Event
#### Make an instance of GoogleCalendarEventModel and add it. Notice that for datetimes like event start and end, need to use GetProperDateTimeFormat method.
```cs
[Inject] CalendarService CalendarService { get; set; }
private void AddEvent()
{
    GoogleCalendarEventModel googleCalendarEvent = new()
    {
        summary = "Test Event",
        description = "Some Description",
        start = new Start { dateTime = CalendarService.GetProperDateTimeFormat(DateTime.Now) },
        end = new End { dateTime = CalendarService.GetProperDateTimeFormat(DateTime.Now) },
    };
    // If you don't know the id of calendar which will the event be added, use FindCalendarId method. In this case, the event added the calendar which has "Test Calendar" title.
    string result = CalendarService.AddEvent(googleCalendarEvent, CalendarService.FindCalendarId(CalendarValueType.Summary, "Test Calendar"));
}
```

## How To Get and Process Lists (Like Calendar and Event)
When you call the api to return a list of items (like calendars or events) it returns a root class. These classes can be find in the library. You can use the "items" property to reach list of items.
<br />
<br />
#### The Models
- GoogleCalendarListRoot -> The root calendar model which is a proper for `GetCalendars()` method result
- GoogleCalendarListModel -> The calendar model. GoogleCalendarListRoot.items has a `List<GoogleCalendarListModel>`
- GoogleCalendarEventRoot -> The root event model which is a proper for `GetEvents()` method result
- GoogleCalendarEventModel -> The event model. GoogleCalendarEventRoot.items has a `List<GoogleCalendarEventModel>`

#### Usage
```cs
var result = CalendarService.GetCalendars();
GoogleCalendarListRoot _calendars = JsonSerializer.Deserialize<GoogleCalendarListRoot>(result); // _calendars.items has the list of calendars.
```
