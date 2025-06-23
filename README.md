# SeriLogX-DotNet8
logging with Serilog in a configurable way
# LogWise - Serilog Logging in .NET 8 (File + AWS CloudWatch + Console)

A professional, extensible logging setup using **Serilog** in **.NET 8**, supporting:

- ✅ Console logging (always on)
- ✅ File logging (configurable)
- ✅ AWS CloudWatch logging (configurable)
- ✅ CloudWatch safety checks (no crash on missing credentials/log group)
- ✅ Asynchronous setup
- ✅ Uses `ILogger<T>` for clean logging in services and controllers

---

## 🔧 Configuration (`appsettings.json`)

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    },
    "LogSinks": {
      "EnableFile": true,
      "EnableCloudWatch": true
    },
    "AWS": {
      "Region": "us-east-1",
      "AccessKey": "YOUR_ACCESS_KEY",
      "SecretKey": "YOUR_SECRET_KEY",
      "LogGroup": "LogWiseGroup"
    }
  }
}

Project Structure


LogWise-Serilog-DotNet8/
├── Logging/
│   └── SerilogConfigurator.cs       # All sink setup logic (modular & async)
├── Controllers/
│   └── LogTestController.cs         # Sample test logs using ILogger<T>
├── appsettings.json
├── Program.cs                       # Clean async Main with startup logging
├── README.md


Dependencies

.NET 8

Serilog.AspNetCore

Serilog.Sinks.Console

Serilog.Sinks.File

Serilog.Sinks.AwsCloudWatch

AWSSDK.CloudWatchLogs