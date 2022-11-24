using BoardGameTracker.Application.Identity.DTO;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BoardGameTracker.Application.Common.Extensions;
using FluentValidation;
using BoardGameTracker.Application.Identity.Services;
using MudBlazor;
using BoardGameTracker.Client.Extensions;

namespace BoardGameTracker.Client.Pages.Identity;

public partial class UpdateEmail
{
    private readonly UpdateEmailRequest request = new();

    private bool show_dialog = false;
    private bool show_errors = false;
    private IEnumerable<string> errors = new List<string>();

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;
    
    [Parameter]
    public EventCallback<bool> IsLoading { get; set; }

    [Inject]
    public IValidator<UpdateEmailRequest> Validator { get; set; } = null!;
    [Inject]
    public IIdentityClient Client { get; set; } = null!;
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await Validator.ValidateAsync(
            ValidationContext<UpdateEmailRequest>.CreateWithOptions(
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
        request.Email = user.GetEmail() ?? string.Empty;
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

        show_dialog = true;
    }

    private async Task OnLoadingChangedHandlerAsync(bool is_loading)
    {
        await IsLoading.InvokeAsync(is_loading);
    }

    private async Task CancelAsync() 
    {
        show_dialog = false;
        await OnLoadingChangedHandlerAsync(false);
    }

    private async Task OkAsync()
    {
        show_dialog = false;

        var response = await Client.UpdateEmail(request);

        if (response.IsSuccessful)
        {
            Snackbar.Add("Email updated", MudBlazor.Severity.Info);
            Snackbar.Add("Please confirm your email to activate your account", MudBlazor.Severity.Info);
            NavigationManager.NavigateTo("/authentication/logout");
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
}