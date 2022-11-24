using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Client.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace BoardGameTracker.Client.Pages.Identity;

public partial class UpdateAccount
{
    private readonly UpdateAccountRequest request = new();

    private bool show_errors = false;
    private IEnumerable<string> errors = new List<string>();

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    [Parameter]
    public EventCallback<bool> IsLoading { get; set; }

    [Inject]
    public IValidator<UpdateAccountRequest> Validator { get; set; } = null!;
    [Inject]
    public IIdentityClient Client { get; set; } = null!;
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await Validator.ValidateAsync(
            ValidationContext<UpdateAccountRequest>.CreateWithOptions(
                request, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };

    protected override async Task OnInitializedAsync()
    { 
        var auth_state = await AuthenticationStateTask;
        var user = auth_state.User;

        request.UserId = user.GetUserId()!;
        request.Username = user.GetUsername() ?? string.Empty;
        request.BGGUsername = user.GetBGGUsername() ?? string.Empty;
    }

    private async Task SubmitAsync()
    {
        show_errors = false;
        await OnLoadingChangedHandlerAsync(true);

        var validation_result = await Validator.ValidateAsync(request);
        if (!validation_result.IsValid)
        {
            show_errors = true;
            errors = validation_result.Errors.Select(e => e.ErrorMessage);
            await OnLoadingChangedHandlerAsync(false);
            return;
        }

        var response = await Client.UpdateAccount(request);

        if (response.IsSuccessful)
        {
            Snackbar.Add("Account updated", MudBlazor.Severity.Info);
            StateHasChanged();
        }
        else if (response.IsNoOp)
            Snackbar.Add("Nothing to update", MudBlazor.Severity.Info);
        else
        {
            var msg = response.Error.IsNullOrWhiteSpace() ? string.Join(",", response.Errors) : response.Error;
            Snackbar.ShowError(msg, MudBlazor.Severity.Warning);
        }

        await OnLoadingChangedHandlerAsync(false);
    }

    private async Task OnLoadingChangedHandlerAsync(bool is_loading)
    {
        await IsLoading.InvokeAsync(is_loading);
    }
}