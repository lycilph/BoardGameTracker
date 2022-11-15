using BoardGameTracker.Application.BoardGameGeek;
using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Application.Game.Services;
using BoardGameTracker.Domain.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace BoardGameTracker.Client.Pages.Content
{
    public partial class Import
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

        [Inject]
        public ILogger<Import> Logger { get; set; } = null!;
        [Inject]
        public BoardGameGeekClient BGGClient { get; set; } = null!;
        [Inject]
        public GameClient Client { get; set; } = null!;

        private MudTextField<string> input_ref = null!;
        private string username = string.Empty;
        private bool loading = false;
        private Profile profile = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await input_ref.FocusAsync();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            loading = true;

            var auth_state = await AuthenticationStateTask;
            var user = auth_state.User;
            var userid = user.GetUserId();

            if (userid != null) 
            {
                profile = await Client.GetProfile(userid);
                username = profile.LastUsedBGGUsername;
            }

            loading = false;
        }

        private async void ImportFromBGG()
        {
            Logger.LogInformation("Starting import now");
            loading = true;

            // Update the bgg username
            if (!string.IsNullOrWhiteSpace(username))
            {
                profile.LastUsedBGGUsername = username;
                await Client.UpdateProfile(profile);
            }

            //var bgg_collection = await BGGClient.GetCollection(username);
            //var collection = await Client.GetCollection(userid);

            loading = false;
            StateHasChanged();
        }

        private void ImportOnEnterAsync(KeyboardEventArgs args)
        {
            if (args.Key.ToLower() == "enter")
                ImportFromBGG();
        }
    }
}