using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Application.Identity.DTO;
using FluentValidation;
using BoardGameTracker.Application.Identity.Services;
using MudBlazor;
using BoardGameTracker.Client.Extensions;
using BoardGameTracker.Application.Common;

namespace BoardGameTracker.Client.Pages.Identity;

public partial class Account
{
    private readonly UpdateAccountRequest request = new();
    private readonly LoadingState state;

    public string username = string.Empty;
    public string bgg_username = string.Empty;
    private string email = string.Empty;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    [Inject]
    public IValidator<UpdateAccountRequest> Validator { get; set; } = null!;
    [Inject]
    public ILogger<Account> Logger { get; set; } = null!;
    [Inject]
    public IIdentityClient Client { get; set; } = null!;
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    public Account()
    {
        state = new LoadingState(StateHasChanged);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await Validator.ValidateAsync(
            ValidationContext<UpdateAccountRequest>.CreateWithOptions(
                request, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };

    protected override async Task OnParametersSetAsync()
    {
        var auth_state = await AuthenticationStateTask;
        var user = auth_state.User;

        username = user.GetUsername() ?? string.Empty;
        bgg_username = user.GetBGGUsername() ?? string.Empty;
        email = user.GetEmail() ?? string.Empty;

        request.UserId = user.GetUserId()!;
        request.Username = user.GetUsername() ?? string.Empty;
        request.BGGUsername = user.GetBGGUsername() ?? string.Empty;
    }

    private async Task SubmitAsync()
    {
        using (state.Load())
        {

            var response = await Client.UpdateAccount(request);

            if (response.IsSuccessful)
                Snackbar.Add("Account updated", MudBlazor.Severity.Info);
            else if (response.IsNoOp)
                Snackbar.Add("Nothing to update", MudBlazor.Severity.Info);
            else
            {
                var msg = response.Error.IsNullOrWhiteSpace() ? string.Join(",", response.Errors) : response.Error;
                Snackbar.ShowError(msg, MudBlazor.Severity.Warning);
            }
        }
    }
}