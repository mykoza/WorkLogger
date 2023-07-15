# WorkLogger

Simple .NET 7 Console Application that logs work tasks and sums up time spent.

Releases are not self-contained.

## Configuration

There are two settings available in `appsettings.json`:

1. `WorkdayInMinutes`: integer representing the length of workday in minutes
2. `Shortcuts`: string[] which contains default task shortcuts, available before any new tasks are logged

Above settings should be inside `Settings` section of the `json` file.

Default settings are:
```json
{
    "Settings": {
        "WorkdayInMinutes": 480,
        "Shortcuts": [
            "HD",
            "Meeting",
            "Przerwa"
        ]
    }
}
```