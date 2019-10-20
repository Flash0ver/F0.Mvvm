using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F0.ComponentModel;
using F0.Windows.Input;
using Octokit;

namespace F0.Mvvm.Example.Windows.Input
{
	internal class MainViewModel : ViewModel
	{
		private int number = 240;
		public int Number
		{
			get => number;
			set => SetProperty(ref number, value);
		}

		private int parameter = 0;
		public int Parameter
		{
			get => parameter;
			set
			{
				if (TrySetProperty(ref parameter, value))
				{
					AddCommand.RaiseCanExecuteChanged();
					SubtractCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public IInputCommand IncrementCommand { get; }
		public IInputCommand DecrementCommand { get; }
		public IInputCommand<int> AddCommand { get; }
		public IInputCommand<int> SubtractCommand { get; }

		private string repoInfo;
		public string RepoInfo
		{
			get => repoInfo;
			set => SetProperty(ref repoInfo, value);
		}

		private string userInfo;
		public string UserInfo
		{
			get => userInfo;
			set => SetProperty(ref userInfo, value);
		}

		public IAsyncCommand ReadGitHubRepoCommand { get; }
		public IAsyncCommandSlim ReadGitHubUserCommand { get; }

		public MainViewModel()
		{
			IncrementCommand = Command.Create(() => Number++);
			DecrementCommand = Command.Create(() => Number--);
			AddCommand = Command.Create<int>(parameter => Number += parameter, parameter => parameter != 0);
			SubtractCommand = Command.Create<int>(parameter => Number -= parameter, parameter => parameter != 0);

			ReadGitHubRepoCommand = Command.Create(FetchRepoInfoAsync);
			ReadGitHubUserCommand = Command.Create(FetchUserInfoAsync);
		}

		private async Task FetchRepoInfoAsync()
		{
			const string owner = "Flash0ver";
			const string reponame = "F0.Mvvm";

			var client = new GitHubClient(new ProductHeaderValue("F0.Mvvm.Example"));

			User user = await client.User.Get(owner);
			Repository repository = await client.Repository.Get(owner, reponame);
			IReadOnlyList<Release> releases = await client.Repository.Release.GetAll(owner, reponame);

			string text = $"Repository: {repository.FullName} ({repository.HtmlUrl})\n";
			text += $"* Name: {repository.Name}\n";
			text += $"* Owner: {repository.Owner.Login} ({user.Followers} followers)\n";
			text += $"* {repository.SubscribersCount} watchers\n";
			text += $"* {repository.StargazersCount} Stargazers\n";
			text += $"* {repository.ForksCount} forks\n";
			text += $"* {repository.Description}\n";
			text += $"* Language: {repository.Language}\n";
			text += $"* Latest commit: {repository.UpdatedAt}\n";
			text += $"* {repository.OpenIssuesCount} open issues\n";
			text += $"* License: {repository.License.SpdxId}\n";

			text += "\n";
			var builder = new StringBuilder($"{releases.Count} Releases\n");
			foreach (Release release in releases.Where(r => !r.Draft))
			{
				builder.Append($"- {release.Name} ({release.TagName})\n");
				builder.Append($"\t{release.Body}\n");
				builder.Append($"\t{release.HtmlUrl}\n");
				builder.Append($"\t{release.PublishedAt}\n");
			}
			text += builder.ToString();

			RepoInfo = text;
		}

		private async ValueTask FetchUserInfoAsync()
		{
			const string owner = "Flash0ver";

			var client = new GitHubClient(new ProductHeaderValue("F0.Mvvm.Example"));

			User user = await client.User.Get(owner);

			string text = $"{user.Type}: {user.Login} ({user.HtmlUrl})\n";
			text += $"* Bio: {user.Bio}\n";
			text += $"* Location: {user.Location}\n";
			text += $"* {user.PublicRepos + user.OwnedPrivateRepos} Repos owned\n";
			text += $"* {user.PublicGists + user.PrivateGists.GetValueOrDefault()} Gists created\n";
			text += $"* {user.Followers} followers\n";
			text += $"* Following {user.Following} users\n";

			UserInfo = text;
		}
	}
}
