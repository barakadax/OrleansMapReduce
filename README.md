# Microsoft Orleans MapReduce

Start a Silo, run your client and see how much words with the same length are in the same text.

## Getting Started

### Prerequisites
- .NET 10 SDK

### Compilation
To compile the entire solution, run:
```bash
dotnet build
```

### Running the Application

#### 1. Start the Silo
The Silo must be running for the client to connect.
```bash
dotnet run --project Silo/Silo.csproj
```

#### 2. Start the Client
Open a new terminal and run:
```bash
dotnet run --project Client/Client.csproj
```

## Testing
To run all unit and functional tests:
```bash
dotnet test
```

### Test Coverage
To run tests and collect code coverage data, use the following command:
```bash
dotnet test --collect:"XPlat Code Coverage"
```
The coverage results (`coverage.cobertura.xml`) will be available in the `TestResults` directory within each test project.

#### Viewing Coverage Reports
To view the coverage results as a human-readable HTML report, you can use `ReportGenerator`.

1. **Install ReportGenerator** (global tool):
    ```bash
    dotnet tool install -g dotnet-reportgenerator-globaltool
    ```

2. **Generate the Report**:
    ```bash
    reportgenerator "-reports:**/coverage.cobertura.xml" -targetdir:TestResults/Report -reporttypes:Html
    ```

3. **Open the Report**:
    Open the generated `TestResults/Report/index.html` file in your browser.

## Code Quality
To ensure code style consistency and apply rules from `.editorconfig`, run:
```bash
dotnet format
```

## Alice's Adventures in Wonderland Result:
Word Length: 1 | encountered: 1705
Word Length: 2 | encountered: 4413
Word Length: 3 | encountered: 7067
Word Length: 4 | encountered: 5782
Word Length: 5 | encountered: 3341
Word Length: 6 | encountered: 1952
Word Length: 7 | encountered: 1573
Word Length: 8 | encountered: 723
Word Length: 9 | encountered: 447
Word Length: 10 | encountered: 184
Word Length: 11 | encountered: 108
Word Length: 12 | encountered: 34
Word Length: 13 | encountered: 11
Word Length: 14 | encountered: 5

## Moby Dick Result:
Word Length: 1 | encountered: 5050
Word Length: 2 | encountered: 18422
Word Length: 3 | encountered: 24837
Word Length: 4 | encountered: 21503
Word Length: 5 | encountered: 13647
Word Length: 6 | encountered: 9061
Word Length: 7 | encountered: 7611
Word Length: 8 | encountered: 5104
Word Length: 9 | encountered: 3236
Word Length: 10 | encountered: 1795
Word Length: 11 | encountered: 952
Word Length: 12 | encountered: 506
Word Length: 13 | encountered: 263
Word Length: 14 | encountered: 77
Word Length: 15 | encountered: 35
Word Length: 16 | encountered: 7
Word Length: 17 | encountered: 5
