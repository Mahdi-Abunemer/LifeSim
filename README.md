# LifeSim

LifeSim is a .NET 8 console simulation with plants, herbivores, and predators.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

Check that the SDK is installed:

```bash
dotnet --version
```

## Restore Dependencies

From the repository root, run:

```bash
dotnet restore LifeSim.sln
```

## Build

```bash
dotnet build LifeSim.sln
```

## Run the Application

```bash
dotnet run --project src/LifeSim/LifeSim.csproj
```

The application runs in the terminal.

Controls:

- `Space` or `P`: pause or resume the simulation
- `Q` or `Esc`: quit

## Run Tests

```bash
dotnet test LifeSim.sln
```

The tests are located in `tests/LifeSim.Tests` and use xUnit.
