using BoardGameTracker.Application.Authentication.DTO;
using BoardGameTracker.Application.Authentication.Services;
using BoardGameTracker.Client.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace BoardGameTracker.Client.Pages.Authentication;

public partial class Register
{
    private readonly RegisterRequest request = new();

    private MudTextField<string> input_ref = null!;
    private bool is_loading = false;
    private bool show_errors = false;
    private IEnumerable<string> errors = new List<string>();

    [Inject]
    public IValidator<RegisterRequest> Validator { get; set; } = null!;
    [Inject]
    public IAuthenticationClient AuthenticationClient { get; set; } = null!;
    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await input_ref.FocusAsync();
        }
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await Validator.ValidateAsync(
            ValidationContext<RegisterRequest>.CreateWithOptions(
                request, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };

    private async Task SubmitAsync()
    {
        show_errors = false;
        is_loading = true;

        var validation_result = await Validator.ValidateAsync(request);
        if (!validation_result.IsValid)
        {
            show_errors = true;
            is_loading = false;
            errors = validation_result.Errors.Select(e => e.ErrorMessage);
            return;
        }

        var result = await AuthenticationClient.RegisterUser(request);
        if (!result.IsSuccessStatusCode)
        {
            is_loading = false;
            Snackbar.ShowError(result.Error!.Content!);
            return;
        }

        Snackbar.Add("Please confirm your email to activate your account", MudBlazor.Severity.Info);
        NavigationManager.NavigateTo("/");
    }

    private async Task SubmitOnEnterAsync(KeyboardEventArgs args)
    {
        if (args.Key.ToLower() == "enter")
            await SubmitAsync();
    }
}
