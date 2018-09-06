# Configuration

This repository contains code to extend functionality around IConfigurationBuilder in the Microsoft.Configuration packages.

## Jayrowe.Configuration.AwsSecretsManager

Allows using JSON stored in AWS Secrets Manager with IConfigurationBuilder. Much in the same way local JSON configuration files function,
the remote configuration can be specified as optional.

Sample usage:

```
WebHost.CreateDefaultBuilder()
  .ConfigureAppConfiguration(c => c.AddAwsSecretsManager($"ApplicationName/{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}", true))
  
  ...
  
```

The general expectation is that the proper AWS region will be configured in the environment and that you'll either be using IAM roles to
control access to your secrets, or you'll be using credentials that the Secrets Manager client can determine automagically. Assuming
that's the case, there is nothing more you should need to do.

If required, you can pass an instance of IAmazonSecretsManager that you've built externally.

I'm certain there are use cases this doesn't cover well, but for simple cases this is a quick way to get secrets out of an
appsettings.json file, secured, and accessible where it is needed.
