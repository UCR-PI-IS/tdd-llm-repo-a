# Git Guidelines

## Index

<!-- TOC -->

- [Git Guidelines](#git-guidelines)
  - [Index](#index)
  - [Git Workflow: Git feature branch workflow](#git-workflow-git-feature-branch-workflow)
  - [Commits](#commits)
    - [**Commit Example**](#commit-example)
  - [Branches](#branches)
    - [Branch Types](#branch-types)
    - [Branch development team](#branch-development-team)
    - [**Branch Examples**](#branch-examples)
  - [Pull Requests](#pull-requests)
    - [Approvals](#approvals)
      - [Reviewers](#reviewers)
      - [Assignees](#assignees)
      - [**How to approve a Pull Request**](#how-to-approve-a-pull-request)
    - [**Pull Request Template**](#pull-request-template)
      - [Pull Request Description](#pull-request-description)

<!-- /TOC -->

## Git Workflow: Git feature branch workflow

The core idea behind the Feature Branch Workflow is that all feature development should take place in a dedicated branch instead of the main branch.

This encapsulation makes it easy for multiple developers to work on a particular feature without disturbing the main codebase. It also means the main branch will never contain broken code, which is a huge advantage for continuous integration environments. ([Source](https://www.atlassian.com/git/tutorials/comparing-workflows/feature-branch-workflow))

## Commits

Commit template:

```gherkin
<type>: <description> (<issue>)

<body>


<optional footer>
<optional co-author>
```

### Commit types

|Type|Description|
|-|-|
|`feat`|A new feature|
|`fix`|A bug fix|
|`docs`|Documentation only changes|
|`style`|Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)|
|`refactor`|A code change that neither fixes a bug nor adds a feature or deletes something|
|`perf`|A code change that improves performance|
|`test`|Adding missing tests or correcting existing tests|
|`ci`|Changes to our Continuous Integrations configuration files and scripts|
|`chore`|Other changes that don't modify src or test files|
|`reverts`|Reverts a previous commit|

### Description

- A properly formed Git commit `<description>` should always be able to complete the following sentence: **If applied, this commit will `<description>`**.
  - For example: **If applied, this commit will remove deprecated methods.**

- The first line of the commit `<type>: <description> (<issue>)` should always be in lowercase and present-tense.
  - Good example: **fix: show referrer for Wasm module dependency errors (#28653)**.
  - Bad example: **Changing behavior of Wasm module**. This commit does not follow the template, and it is in past-tense.

### Body

`<body>` should be used for a more detailed explanatory description of the change made in the commit. It also needs to be wrapped at about 72 characters per line.

If there is any pending work or additional consideration for the other developers, specify it at the end of the body. Include contact information of all
the developers that contributed as well as their percentage of attribution.

### Footer

If a commit fixes or closes the issue, `<footer>` should indicate a message ex: `Fixes #23`.

### Co-author

Co-authors should be added at the **end** of the commit message with the following template:

```sh
Co-authored-by: git_username <git_primary_email@example.com>
```

### Commit Example

```md
docs: add git guidelines

This commit adds the markdown file with the workflow description and templates to
be used in the repo

Fixes #2

Co-authored-by: git_username1 <git_primary_email@ucr.ac.cr>
Co-authored-by: git_username2 <git_primary_email@ucr.ac.cr>
Co-authored-by: git_username3 <git_primary_email@ucr.ac.cr>
```

## Branches

Branch template:

```gherkin
<type>/<team>/<description>
```

### Branch Types

|Type|Description|Example|
|-|-|-|
|`feature`|Used to develop new features, bug fixes or other changes in a separated branch from `main`|feature/team/add-user-authentication|
|`bugfix`|A bug applied to a *feature* branch.|bugfix/team/fix-buildings-read|
|`hotfix`|A bug fix applied to the *main* branch.|hotfix/team/fix-exit-button|
|`docs`|Add or edit any kind of documentation.|docs/team/git-template|

### Branch development team

A branch development **team** can refer to either a working group or a cross-functional team. If a branch has been worked on by multiple development teams, then the following acronyms should be used to specify which   name of each contributing group:

|Working Group| Acronym|
|-|-|
|Sprinters|SPT|
|#CPrendió++|CPD|
|SQLit(s)|SQL|
|Prequels|PQL|

|Cross-functional team|Acronym|
|-|-|
|Git|GIT|
|Software Quality Assurance|SQA|
|Product Backlog|PB|
|Clean Architecture|CA|
|Databases|DB|
|Communications|COM|
|Look and Feel|LF|

### Branch Examples

Example of the documentation branch where these guidelines were added:

```sh
# <type>/<team>/<description>
docs/GIT/git-guidelines
```

```sh
# <type>/<team>/<description>
feature/CA-SQA-GIT/codebase
```

## Pull Requests

### Approvals

#### Reviewers

Each Pull Request must be reviewed by at least two developers:

- One of the reviewers must complete the [Pull Request Checklist](#pull-request-template).
- If available, Copilot will be used as the first automatic validation for PRs.
- A functional review by a member of the **SQA team**:
  
  |Working Group|Git username|
  |-|-|
  |Sprinters|andresMurillo23|
  |#CPrendió++|Earaya12|
  |SQLit(s)|JustAC1|
  |Prequels|disotocastro|
  |Prequels|WilliamJMF|

#### Assignees

Assignees are the ones in charge of merging an approved PR, include at least the **PR opener** and assign other developers at your own discretion.

#### How to approve a Pull Request

See a short tutorial on how to approve a PR [here](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/reviewing-changes-in-pull-requests/approving-a-pull-request-with-required-reviews), also see how to assign reviewers [in this link](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/requesting-a-pull-request-review). For more information about PR's, refer to the [Github documentation](https://docs.github.com/en/pull-requests).

### Pull Request Template

#### Pull Request Description

Include a summary of the changes and the related issue. List any dependencies that are required for this change. For example:

```git
## Description

### Type of Change

- [ ] Bug fix (**non-breaking** change which fixes an issue)
- [ ] New feature (**non-breaking** change which adds functionality)
- [ ] Documentation update

#### What was done?

- Briefly explain the changes introduced.
- List new features, behavior changes, or refactors.

#### How was it done?

- Technical description of the approach taken.
- Key points of the design or implementation.

#### How Has This Been Tested? (Only for feature changes, not documentation)

Describe the tests that were made to verify the changes. Provide instructions so other developers can perform these tests, also list any relevant details to the test configuration.

- [ ] Test A
- [ ] Test B

#### Checklist (Only for feature changes, not documentation)

Each author should assign members of **their own development team** the task of ensuring this pull request is made in accordance to the following checklist:

- [ ] The code follows the style guidelines of this project.
- [ ] The code is commented, particularly in hard-to-understand areas.
- [ ] They have made corresponding changes to the documentation.
- [ ] The changes generates no new warnings.
- [ ] The are new tests that prove the fix is effective or that the feature works.
- [ ] New and existing unit tests pass locally with my changes.
- [ ] Any dependent changes have been merged and published in downstream modules.

### Additional notes
- Are there any database changes?
- Is this change backward-compatible?
```

<p align="center">
    <img src="https://media.discordapp.net/attachments/1353815757414338590/1354489699695333547/agile.png?ex=67ffd855&is=67fe86d5&hm=57d2b274c0345b813cfe0c91a3b0127f94ef8c85bf3574580842fa5fed06f1f4&=&format=webp&quality=lossless" alt="Make Agile Scrum Again" />
</p>
