using BoardGameTracker.Application.Authentication.Commands;
using BoardGameTracker.Application.Authentication.Data;
using BoardGameTracker.Application.Authentication.DTO;
using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Application.Services.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using NSubstitute;

namespace ControllerTest;

public class RegisterTest
{
    private RegisterRequest request;
    private RegisterCommand command;
    private RegisterRequestValidator request_validator;
    private RegisterCommandValidator command_validator;
    private IIdentityService identity_service;
    private IMailService mail_service;
    private RegisterUserCommandHandler command_handler;

    public RegisterTest()
    {
        request = new RegisterRequest
        {
            Username = "username",
            Email = "email@site.com",
            BGGUsername = "bgg username",
            Password = "password",
            ConfirmPassword = "password"
        };
        command = new RegisterCommand(request, "origin");

        request_validator = new RegisterRequestValidator();
        command_validator = new RegisterCommandValidator(request_validator);
        identity_service = Substitute.For<IIdentityService>();
        mail_service = Substitute.For<IMailService>();
        command_handler = new RegisterUserCommandHandler(command_validator, identity_service, mail_service);
    }

    [Fact]
    public void RegisterRequest_Succeess()
    {
        // Arrange - see constructor

        // Act
        var result = request_validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void RegisterRequest_FailsWith_UsernameValidation()
    {
        // Arrange
        request.Username = "user";

        // Act
        var result = request_validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Username", result.Errors.First().PropertyName);
    }

    [Fact]
    public void RegisterRequest_FailsWith_EmailValidation()
    {
        // Arrange
        request.Email = "email";

        // Act
        var result = request_validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Email", result.Errors.First().PropertyName);
    }

    [Fact]
    public void RegisterRequest_FailsWith_PasswordValidation()
    {
        // Arrange
        request.Password = "abc";
        request.ConfirmPassword = "abc";

        // Act
        var result = request_validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Password", result.Errors.First().PropertyName);
    }

    [Fact]
    public void RegisterRequest_FailsWith_ConfirmPasswordValidation()
    {
        // Arrange
        request.ConfirmPassword = "abc";

        // Act
        var result = request_validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("ConfirmPassword", result.Errors.First().PropertyName);
    }

    [Fact]
    public async void RegisterUser_Success()
    {
        // Arrange
        identity_service.CreateUserAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        identity_service.UpdateUserAsync(Arg.Any<ApplicationUser>()).Returns(IdentityResult.Success);
        identity_service.AddUserToRoleAsync(Arg.Any<ApplicationUser>(), RoleConstants.UserRole).Returns(IdentityResult.Success);

        // Act
        var response = await command_handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(response.IsSuccessful);
        Assert.Empty(response.Error);
        Assert.Empty(response.Errors);
        await mail_service.ReceivedWithAnyArgs().SendConfirmationEmail(default!, default!);
    }
    
    [Fact]
    public async void RegisterUser_FailsWith_ValidationError()
    {
        // Arrange
        request.Username = "user";

        // Act
        var response = await command_handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.IsSuccessful);
        Assert.Empty(response.Error);
        Assert.Single(response.Errors);
    }

    [Fact]
    public async void RegisterUser_FailsWith_UsernameAlreadyExists()
    {
        // Arrange
        identity_service.FindUserByNameAsync(Arg.Any<string>()).Returns(new ApplicationUser());

        // Act
        var response = await command_handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.IsSuccessful);
        Assert.NotEmpty(response.Error);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public async void RegisterUser_FailsWith_EmailAlreadyExists()
    {
        // Arrange
        identity_service.FindUserByEmailAsync(Arg.Any<string>()).Returns(new ApplicationUser());

        // Act
        var response = await command_handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.IsSuccessful);
        Assert.NotEmpty(response.Error);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public async void RegisterUser_FailsWith_UserCreationError()
    {
        // Arrange
        var msg = "Cannot create user";
        identity_service.CreateUserAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Failed(new IdentityError { Description = msg }));

        // Act
        var response = await command_handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.IsSuccessful);
        Assert.Empty(response.Error);
        Assert.Single(response.Errors, msg);
    }

    [Fact]
    public async void RegisterUser_FailsWith_RoleAdditionError()
    {
        // Arrange
        identity_service.CreateUserAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        var msg = "Cannot add user to role";
        identity_service.AddUserToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Failed(new IdentityError { Description = msg }));

        // Act
        var response = await command_handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.IsSuccessful);
        Assert.Empty(response.Error);
        Assert.Single(response.Errors, msg);
    }

    [Fact]
    public async void RegisterUser_FailsWith_UpdateUserError()
    {
        // Arrange
        var msg = "Cannot update user";
        identity_service.CreateUserAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        identity_service.AddUserToRoleAsync(Arg.Any<ApplicationUser>(), RoleConstants.UserRole).Returns(IdentityResult.Success);
        identity_service.UpdateUserAsync(Arg.Any<ApplicationUser>()).Returns(IdentityResult.Failed(new IdentityError { Description = msg }));

        // Act
        var response = await command_handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.IsSuccessful);
        Assert.Empty(response.Error);
        Assert.Single(response.Errors, msg);
    }
}