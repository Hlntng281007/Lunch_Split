Project setup
This solution was scaffolded from the command line as a src/tests split, rather than a single project:
bash
dotnet new sln -n Lunch_Split
dotnet new console -o src/Lunch_Split
dotnet new mstest -n Lunch_Split.Tests -o tests/Lunch_Split.Tests

dotnet sln add src/Lunch_Split/Lunch_Split.csproj
dotnet sln add tests/Lunch_Split.Tests/Lunch_Split.Tests.csproj

dotnet add tests/Lunch_Split.Tests reference src/Lunch_Split/Lunch_Split.csproj


Build
dotnet restore
dotnet build

Run
dotnet run --project src/Lunch_Split

Run the unit tests
dotnet test

