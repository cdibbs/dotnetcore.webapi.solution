# .NET Core WebAPI Template Project

A project to bootstrap an MSSQL-DB-based WebAPI, complete with unit and integration tests.

## Features
### High-level features
- Pre-configured to integrate with an OAuth2 provider.
- [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) auto-generated API documentation at [http://localhost:50777/swagger/ui/index](http://localhost:50777/swagger/ui/index).
- [Serilog](https://github.com/serilog/serilog) logging
  - Uses [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration).
- [AutoFac](https://github.com/autofac/Autofac) for Dependency Injection, with
  - [AutoFac.Extensions.DependencyInjection](https://github.com/autofac/Autofac.Extensions.DependencyInjection)
- Database access through [Microsoft.EntityFrameworkCore](https://github.com/aspnet/EntityFramework).

### Code features
- API code abstracted away from endpoints to facilitate code reuse and testing.
- Data project uses generic repo with Spec pattern for query code reuse and testing.
- Code implements SOLID patterns including IoC and SoC.
- Unit test suite with >120 tests covering all base functionality. Integration tests will be added
  once dotnet core supports the [required items](https://github.com/Microsoft/testfx/issues/96).

## Requirements

The only requirement is the dotnet commandline tool, >= 2.0.0. Get it, here:
[https://github.com/dotnet/cli/tree/release/2.0.0](https://github.com/dotnet/cli/tree/release/2.0.0)

## Usage

To install this template:

1. Open powershell.
2. Clone this repository:
   `git clone https://github.com/cdibbs/dotnetcore.webapi.solution.git`
3. From outside the cloned directory, run `dotnet new -i ./dotnetcore.webapi.solution`.
4. Create a new directory for your WebAPI solution, and change to that directory. Note: The solution will be named after the directory
   unless you use the `-n [my-proj-name]` option with the `dotnet` command.
5. From within that directory, run `dotnet new awebapi` (optionally with the `-n` switch). The solution will be scaffolded.
7. Open the new solution and run all tests to verify that the solution was created correctly.
8. Launch the solution and check out [Swashbuckle](http://localhost:62480/swagger/). Try using it to login and test the User endpoint.

## Authors
- Chris Dibbern