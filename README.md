# Import from Tempo to Another Time Tracking Tool - v 1.0.0

This script will update your logged time on Tempo to or `Toggl` or `Clockify`. The idea is to make it possible to add more Tracking Tools if needed.

This script use [`.NET 6.0`](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) to run.

To use the script you need:
- Update file trackingtoolsuserinformation.json. See more information about it below
- Download the raw data from Tempo, copying its content on rawtempo.csv
- On your terminal, navegate to the folder where you download this script and run 
```
dotnet import-from-tempo-to-another-tool.dll
```

## Tempo Raw Data

To download the raw data from Tempo:
1. Go to Tempo
2. On the top right corner, click on Timesheet
3. Click on 3 dots (...) menu
4. Click on Download Raw Data
5. Select CSV option


## Authenticator

To select the Time Tracking Tool you want to upload your logged time, you need to change trackingtoolsuserinformation.json file.

If you want to use `Clockify` update trackingtoolsuserinformation.json file as example:

```
{
    "clockify":
    {
        "api_key":"ABCDEFGHIJ12345678",
        "projectId":""
    }
}
```

If you want to use `Toogl` update trackingtoolsuserinformation.json file as example:

```
{
    "toggl":
    {
        "usertoken":"",
        "useremail":"myemail@email.com",
        "userpassword":"mypassword",
        "workspaceid":123456
    }
}
```
To use User Token on Toggl, see example below.


## Authenticator

### Clockify
To identify your `Clockify` account, you need to generate an API Key. To generate it, go to your [Clockify profile page](https://app.clockify.me/user/settings) , Scroll to the bottom of the page, on API session. After Key generated, copy it to the trackingtoolsuserinformation.json file. 

Example:
```
{
    "clockify":
    {
        "api_key":"ABCDEFGHIJ12345678",
        "projectId":""
    }
}
```

### Toggl
There is two ways you can use to identify your `Toggl` account:

1. Using your email and password
2. Using Toggl Token

#### Email and password

To use email and password as authenticator, on trackingtoolsuserinformation.json file:
- usertoken as a empty string
- useremail the email you use to login into Toggl
- userpassword the password you use to login into Toggl

Example:
```
{
    "toggl":
    {
        "usertoken":"",
        "useremail":"youremail@email.com",
        "userpassword":"yourpassword",
        "workspaceid":123456
    }
}
```

#### Token

To use Toggl Token as authenticator, on trackingtoolsuserinformation.json file:
- usertoken as token string
- useremail as a empty string
- userpassword as a empty string

Example:
```
{
    "toggl":
    {
        "usertoken":"abcdefgh1234567890",
        "useremail":"",
        "userpassword":"",
        "workspaceid":123456
    }
}
```

## v 1.0.0

1. BIG BANG commit

2. Read time entries from rawtempo.csv file and User Information from trackingtoolsuserinformation.json and update time entries to selected Time Tracking tool

3. Script check if time entries from Tempo are from current week. If there is a time entry from different week, script ask user if they want to continue or cancel all the import

4. Before export time entry to selected Time Tracking tool, script check if there is a time entry on selected Time Tracking tool with the same Start Date, same Duration and same Description of the time entry that it is about to save. If there is, script ask user if they want to continue or cancel export of current time entry.