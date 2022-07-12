# API

## Auth Service

### AuthorizeCredential
Get credentials with given authorizationCode. Returns string content.
#### Parameters
AuthorizationCode: The code which you have with `RequestAuthorizationCode` <br/>

<hr/>

### GetAccessTokenDetails
Get information with given accesstoken like scope, expires in etc. Returns a json format.

<hr/>

### GetClientId, GetClientSecret, GetRedirectUrl
Only works when the value stored in appsettings.json.

#### Parameters
RootValue: Allows customization. By default it searches from `GoogleClient` root. <br/>

<hr/>

### GetUserMail
Returns user email with given access token.

<hr/>

### GetValueFromCredential
Brings a value into a given credential string. The credential string can be obtain with result of AuthorizeCredential method.

#### Enum: CredentialValueType -> AccessToken, RefreshToken, Scope, TokenType, ExpiresIn

<hr/>

### IsAccessTokenExpired
Returns a bool value that access token expired.

<hr/>

### RefreshAccessToken
Refresh and create new access token with given refresh token. Returns the new access token as a string.
#### Note
If you placed your clientid, clientsecret and redirecturl in your `appsettings.json`, you can use the method only with refreshtoken. (No need to define them again)

<hr/>

### RequestAuthorizationCode
Opens Google "select account" page in a new tab.
#### Parameters
ClientId: Your project's client id. Can be obtanied on google developer console. <br/>
RedirectUrl: The page which the authorization code will send. The page opens immediately after user selects their google account.

<hr/>

## Calendar Service

### Fields
AccessToken: It must be set before the call of other methods. All methods use this access token value. <br/>
RefreshToken: It is used for automatic renewal of access token. All methods have `forceAccessToken`parameter, and it only works if refresh token is not null or empty.

<hr/>

### EventCallbacks
<b>AccessTokenChanged:</b> Fires when access token changed. <br/>
<b>RefreshTokenChanged:</b> Fires when refresh token changed.

<hr/>

### AddCalendar
Creates a new secondary calendar into user's CalendarList with given `GoogleCalendarListModel`.

<hr/>

### AddEvent
Creates a new event in a specific calendar with given `GoogleCalendarEventModel`.

<hr/>

### ClearCalendar
Deletes all events in a primary calendar. If it's a secondary calendar, it has no effect. Use `DeleteCalendar` method for secondary ones.

<hr/>

### DeleteCalendar
Deletes the specified calendar. Only deletes the secondary calendars. Use `ClearCalendar` method for primary calendar.

<hr/>

### DeleteEvent
Deletes the event in a specific calendar.

<hr/>

### GetCalendarById
Get calendar with given id that authenticated user has. Returns `GoogleCalendarListModel`.

<hr/>

### GetCalendarBySummary
Get calendar with given summary (title) that authenticated user has. Returns `GoogleCalendarListModel`.

<hr/>

### GetCalendars
Get all calendars that authenticated user has. Returns max of 250 calendars (Google limitation). Returns `GoogleCalendarListRoot`.

<hr/>

### GetEventById
Get event in a specific calendar with a given id. Returns `GoogleCalendarEventModel`.

<hr/>

### GetEventByDescription
Get event in a specific calendar with a given description. Returns `GoogleCalendarEventModel`.

<hr/>

### GetEventBySummary
Get event in a specific calendar with a given summary (title). Returns `GoogleCalendarEventModel`.

<hr/>

### GetEvents
Get event list within a specific calendar between the specified dates. Returns a max of 2500 events (Google limitation). Returns `GoogleCalendarEventRoot`.

<hr/>

### GetProperDateTimeFormat
Format datetime (ISO standarts) to make proper Google api calls.

<hr/>

### FindCalendarId
Finds and returns the calendar id which matches with the given value (Summary, description or location). Returns calendar id as string.

<hr/>

### FindEventId
Finds and returns the event id which matches with the given value and type (Summary, description or location). Returns event id as string.

<hr/>

### FindPrimaryCalendar
Get the primary calendar in calendar list. Returns null if not found or `GoogleCalendarListModel` if found.

<hr/>

### UpdateCalendar
Updates the specified calendar with given model. Returns `GoogleCalendarModel`.

<hr/>

### UpdateEvent
Updates the event.
