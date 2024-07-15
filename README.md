# dotnet-lambda-libsql-demo

This is a demo project to show how to use the [libSQL](https://docs.turso.tech/libsql) library in a .NET Lambda function.

Check the [blog post](https://dev.to/hermogenes/you-should-try-net-libsql-and-heres-why-173h) for more details.

## Prerequisites

- [Node.js 20](https://nodejs.org/en/download/)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [AWS CLI](https://aws.amazon.com/cli/)
- [AWS SAM CLI](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install.html)
- [libSQL DB](https://turso.tech/)
- [Taskfile](https://taskfile.dev/installation/)

## Setup

1. Clone this repository
2. Configure your AWS credentials
3. Configure the `libSQL` env vars in .env
4. Run `task libsql-setup` to create the DB schema
5. Run `task aws-setup` to create the functions
6. Run `task loadtest FUNCTION=libsql` to load test the function

## Cloudwatch Query

```
filter @type="REPORT"
| fields greatest(@initDuration, 0) + @duration as duration, ispresent(@initDuration) as coldstart
| stats count(*) as count, min(duration) as min, avg(duration) as avg, pct(duration, 50) as p50, pct(duration, 75) as p75, pct(duration, 90) as p90, pct(duration, 95) as p95, pct(duration, 99) as p99, max(duration) as max by coldstart
```