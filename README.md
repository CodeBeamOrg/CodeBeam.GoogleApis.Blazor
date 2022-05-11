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
Prepairing.


## Contributing
Welcome to all contributors!


## Usage
### Step 1: Inject HttpClientFactory

For blazor component files
```razor
@inject IHttpClientFactory HttpClientFactory
```
For cs files
```cs
[Inject] IHttpClientFactory HttpClientFactory { get; set; }
```

### Step 2: Create an instance of needed api class and implement
```cs
private void NewCalendar()
{
  CalendarService calendarService = new CalendarService(HttpClientFactory);
  calendarService.AddCalendar(_accessToken, "Calendar Title"); //Can get access token with AuthService.
}
```

### Step 3: null. That's all.
