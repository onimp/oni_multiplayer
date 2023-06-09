# Welcome to Oxygen Not Included - Multiplayer Mod contributing guide <!-- omit in toc -->

Thank you for investing your time in contributing to our project!

In this guide you will get an overview of the contribution workflow from opening an issue, creating a PR, reviewing, and
merging the PR.

## New contributor guide

To get an overview of the project, read the [README](README.md). Here are some resources to help you get started with
open source contributions:

- [Finding ways to contribute to open source on GitHub](https://docs.github.com/en/get-started/exploring-projects-on-github/finding-ways-to-contribute-to-open-source-on-github)
- [Set up Git](https://docs.github.com/en/get-started/quickstart/set-up-git)
- [GitHub flow](https://docs.github.com/en/get-started/quickstart/github-flow)
- [Collaborating with pull requests](https://docs.github.com/en/github/collaborating-with-pull-requests)

## Getting started

1. Install `Visual Studio 2022`
2. Clone the Oni Multiplayer repository
3. If you do not have a custom `steam library` location for `Oxygen Not Included` skip to `step 4`
   - Copy/paste the file `Directory.Build.props` inside the `root` folder, and rename the copy to `Directory.Build.props.user`
   - Erase all contents, and fill it in with the following (example for `D` drive)
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
    <PropertyGroup>
        <SteamLibraryPath>D:\SteamLibrary</SteamLibraryPath>
    </PropertyGroup>
</Project>
```

4.  Open the `OniMod.sln` file inside of the `root` folder
5. If you do not have the multiplayer mod subscribe skip to step 6. If you do have it installed you will have 2 entries in game, so lets distinguish them: to the right of solution explorer, go to `MultiplayerMod -> mod.yaml`
   - Add `DEV` to the text value end of the `Title` property
6. Build solution (keys ctrl+shift+B) or `Top bar build -> build solution`
   - If you get an error about `Unable to find package` inside of `Microsoft Visual Studio Offline Packages`.
     - Right click the `solution` in the top right
     - Click `Manage Nuget`
     - Click the `gear` at the right top
     - Add name: `nuget.org`, url: `https://api.nuget.org/v3/index.json`
7. Start the game trough steam
8. If you did `step 5`, then go to mods and make sure you got the `DEV` version `enabled` and the `subscribed` one `disabled`
9. Edit your code, and repeat `step 6`, `step 7` and test ingame

### Issues

#### Create a new issue

If you spot a problem with the
docs, [search if an issue already exists](https://docs.github.com/en/github/searching-for-information-on-github/searching-on-github/searching-issues-and-pull-requests#search-by-the-title-body-or-comments).
If a related issue doesn't exist, you can open a new issue using a
relevant [issue form](https://github.com/zuev93/oni_multiplayer/issues/new).

#### Solve an issue

Scan through our [existing issues](https://github.com/zuev93/oni_multiplayer/issues) to find one that interests you. You
can narrow down the search using `labels` as filters.

After you've found an issue that you'd like to tackle - let other know about it.
Write down a comment or assign it to yourself.

If you are in doubt feel free to ask questions via comments.

### Make Changes

For local development you can use any tool that you prefer. We've tested the project with JetBrains Rider and Visual
Studio 2022.

### Pull Request

When you're finished with the changes, commit your changes and create a pull request, also known as a PR.

- Fill the "Ready for review" template so that we can review your PR. This template helps reviewers understand your
  changes as well as the purpose of your pull request.
- Don't forget
  to [link PR to issue](https://docs.github.com/en/issues/tracking-your-work-with-issues/linking-a-pull-request-to-an-issue)
  if you are solving one.
- Enable the checkbox
  to [allow maintainer edits](https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests/allowing-changes-to-a-pull-request-branch-created-from-a-fork)
  so the branch can be updated for a merge.
  Once you submit your PR, a maintainer will review your proposal. We may ask questions or request additional
  information.
- We may ask for changes to be made before a PR can be merged, either
  using [suggested changes](https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests/incorporating-feedback-in-your-pull-request)
  or pull request comments. You can apply suggested changes directly through the UI. You can make any other changes in
  your fork, then commit them to your branch.
- As you update your PR and apply changes, mark each conversation
  as [resolved](https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests/commenting-on-a-pull-request#resolving-conversations).
- If you run into any merge issues, checkout this [git tutorial](https://github.com/skills/resolve-merge-conflicts) to
  help you resolve merge conflicts and other issues.

### Your PR is merged!

Once your PR is merged, your contributions will be publicly visible on the [Mod repository](https://github.com/zuev93/oni_multiplayer).
