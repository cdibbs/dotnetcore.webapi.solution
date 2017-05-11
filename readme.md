# .NET Core WebAPI Template Project

A project to bootstrap an MSSQL-DB-based WebAPI, complete with unit and integration tests.

## Features
### High-level features
- Pre-configured to integrate with an OAuth2 provider.
- [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) auto-generated API documentation at [http://localhost:50777/swagger/ui/index](http://localhost:50777/swagger/ui/index).
- [Serilog](https://github.com/serilog/serilog) logging
  - Uses [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration).
- [AutoFac](https://github.com/autofac/Autofac) for Dependency Injection.
- Database access through [Microsoft.EntityFrameworkCore](https://github.com/aspnet/EntityFramework).

### Code features
- API uses "Manager" endpoint architecture to facilitate testing and code reuse (see BaseManager).
- Data project uses generic repo with Spec pattern for query code reuse and testing.
- IoC: Dependency Injection (DI) through Ninject.
- Pure OWIN app (no Global.asax)
- Unit and integration test suite with >120 tests covering all base functionality, particularly the
  API's BaseManager class and the Repository generic query methods.

## Requirements

The only requirement is the dotnet commandline tool, >= 2.0.0. Get it, here:
[https://github.com/dotnet/cli/tree/release/2.0.0](https://github.com/dotnet/cli/tree/release/2.0.0)

## Usage

To install this template:

1. Open powershell.
2. Clone this repository:
   `git clone https://git.its.uiowa.edu/scm/its-ad/appdev.basetemplate.git`
3. From outside the cloned directory, run `dotnet new -i ./AppDev.BaseTemplate`.
4. Create a new directory for your WebAPI solution, and change to that directory. Note: The solution will be named after the directory
   unless you use the `-n [my-proj-name]` option with the `dotnet` command.
5. From within that directory, run `dotnet new awebapi` (optionally with the `-n` switch). The solution will be scaffolded.
7. Open the new solution and run all tests to verify that the solution was created correctly.
8. Launch the solution and check out [Swashbuckle](http://localhost:50777/swagger/ui/index). Try using it to login and test the User endpoint.

## Authors
- Chris Dibbern
- Yan Li