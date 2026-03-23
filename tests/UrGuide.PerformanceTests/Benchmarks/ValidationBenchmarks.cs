using BenchmarkDotNet.Attributes;
using FluentValidation;
using UrGuide.Model.Users;
using UrGuide.Services.Users;

namespace UrGuide.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ValidationBenchmarks
{
    private LoginValidation _loginValidator = null!;
    private CreateUserValidation _createUserValidator = null!;
    private LoginModel _validLoginModel = null!;
    private LoginModel _invalidLoginModel = null!;
    private CreateUserModel _validCreateUserModel = null!;

    [GlobalSetup]
    public void Setup()
    {
        _loginValidator = new LoginValidation();
        _createUserValidator = new CreateUserValidation();

        _validLoginModel = new LoginModel
        {
            UserName = "user@example.com",
            Password = "password123"
        };

        _invalidLoginModel = new LoginModel
        {
            UserName = "",
            Password = ""
        };

        _validCreateUserModel = new CreateUserModel
        {
            Email = "user@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            FirstName = "John",
            LastName = "Smith"
        };
    }

    [Benchmark(Baseline = true)]
    public FluentValidation.Results.ValidationResult ValidateLogin_Valid() => _loginValidator.Validate(_validLoginModel);

    [Benchmark]
    public FluentValidation.Results.ValidationResult ValidateLogin_Invalid() => _loginValidator.Validate(_invalidLoginModel);

    [Benchmark]
    public FluentValidation.Results.ValidationResult ValidateCreateUser_Valid() => _createUserValidator.Validate(_validCreateUserModel);
}
