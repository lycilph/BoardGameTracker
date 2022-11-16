using BoardGameTracker.Application.BoardGameGeek;
using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Application.Game.Services;
using BoardGameTracker.Application.Identity.Services;
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
        public GameClient GameClient { get; set; } = null!;
        [Inject]
        public IIdentityClient IdentityClient { get; set; } = null!;
        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        public bool DisableSync { get { return !games_to_add_selected.Any() && !games_to_remove_selected.Any(); } }

        private MudTextField<string> input_ref = null!;
        private string userid = string.Empty;
        private string username = string.Empty;
        private bool loading = false;
        private List<BoardGame> owned_games = new();
        private List<BoardGame> games_to_add = new();
        private List<BoardGame> games_to_remove = new();
        private List<BoardGame> games_in_collection = new();
        private HashSet<BoardGame> games_to_add_selected = new();
        private HashSet<BoardGame> games_to_remove_selected = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await input_ref.FocusAsync();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            var auth_state = await AuthenticationStateTask;
            var user = auth_state.User;
            userid = user.GetUserId()!;
            username = user.GetBGGUsername() ?? string.Empty;
        }

        private async void LoadFromBGG()
        {
            if (string.IsNullOrEmpty(username))
            {
                Snackbar.Add("Please enter a BoardGameGeek username");
                return;
            }

            Logger.LogInformation("Loading games from BGG");
            loading = true;

            // Update the BGG username
            if (!string.IsNullOrWhiteSpace(username))
                await IdentityClient.UpdateBGGUsername(userid, username);

            // Check if user exists on bgg
            if (!await BGGClient.UserExists(username))
            {
                Snackbar.Add($"Cannot find User {username} on BoardGameGeek");
                loading = false;
                StateHasChanged();
                return;
            }

            // Load games from BGG
            var bgg_collection = await BGGClient.GetCollection(username);
            var bgg_owned_games = bgg_collection.Where(g => g.IsOwned()).ToList();

            // Load profile from DB
            owned_games = await GameClient.GetCollectionAsync(userid);

            // Compare BGG and DB
            var comparer = new BoardGameComparer();
            games_to_add = bgg_owned_games.Except(owned_games, comparer).ToList();
            games_to_remove = owned_games.Except(bgg_owned_games, comparer).ToList();
            games_in_collection = owned_games.Intersect(bgg_owned_games, comparer).ToList();

            loading = false;
            StateHasChanged();
        }

        private void LoadOnEnterAsync(KeyboardEventArgs args)
        {
            if (args.Key.ToLower() == "enter")
                LoadFromBGG();
        }

        private async void ImportGames()
        {
            loading = true;
            
            var comparer = new BoardGameComparer();
            if (games_to_add_selected.Any())
                owned_games.AddRange(games_to_add_selected);
            if (games_to_remove_selected.Any())
                owned_games.RemoveAll(g => games_to_remove_selected.Contains(g, comparer));

            await GameClient.UpdateCollectionAsync(userid, owned_games);

            owned_games.Clear();
            games_to_add.Clear();
            games_to_remove.Clear();
            games_in_collection.Clear();
            games_to_add_selected.Clear();
            games_to_remove_selected.Clear();

            loading = false;
            StateHasChanged();
        }
    }
}