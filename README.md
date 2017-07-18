| Windows | Linux | OS X |
| :---- | :------ | :---- |
[![Windows build status][1]][2] | [![Linux build status][3]][4] | [![OS X build status][5]][6] | 

[1]: https://ci.appveyor.com/api/projects/status/j1evinfefeetctvk?svg=true
[2]: https://ci.appveyor.com/project/stratis/breeze
[3]: https://travis-ci.org/stratisproject/Breeze.svg?branch=master
[4]: https://travis-ci.org/stratisproject/Breeze
[5]: https://travis-ci.org/stratisproject/Breeze.svg?branch=master
[6]: https://travis-ci.org/stratisproject/Breeze


# Breeze

This is the repository of the Breeze Wallet, the first full-block SPV bitcoin wallet using Angular and Electron at the front-end and C# with .NET Core in the back-end.

At the moment, only bitcoin on testnet is supported but more is coming soon. 

## How to build the Breeze Daemon

Breeze daemon is the backend REST service, hosting a Bitcoin node upon which Breeze UI depends:

```
# Clone and go in the directory
git clone https://github.com/stratisproject/Breeze
cd Breeze

# Initialize dependencies
git submodule update --init --recursive

# Go in Breeze's solution folder
cd Breeze
dotnet restore
dotnet build

# Run a daemon Bitcoin SPV node on testnet
cd src/Breeze.Daemon
dotnet run light -testnet
```

CI build
-----------

We use [AppVeyor](https://www.appveyor.com/) for Windows CI builds and [Travis CI](https://travis-ci.org/) (coming soon) for our Linux and MacOS ones.
Every time someone pushes to the master branch or create a pull request on it, a build is triggered and a new unstable app release is created.

To skip a build, for example if you've made very minor changes, include the text **[skip ci]** or **[ci skip]** in your commits' comment (with the squared brackets).

If you want the :sparkles: latest :sparkles: (unstable :bomb:) version of the Breeze app, you can get it here: 

|    | x86 Release | x64 Release | Notes |
|:---|----------------:|------------------:|
|**Windows 7**| [download][7] | [download][8] | continuous build - up to date with commits |
|**Windows 10**| [download][9] | [download][10] | continuous build - up to date with commits | 
|**Ubuntu 14.04**| - | coming soon | manual build 
|**Ubuntu 16.04**| - | coming soon | manual build
|**OS X 10.11**| - | [download][13] |  continuous build - up to date with commits |
|**OS X 10.12**| - | [download][14] |  continuous build - up to date with commits |


[7]: https://ci.appveyor.com/api/projects/stratis/breeze/artifacts/breeze_out/breeze-win7-x86-Release.zip?job=Environment%3A%20win_runtime%3Dwin7-x86
[8]: https://ci.appveyor.com/api/projects/stratis/breeze/artifacts/breeze_out/breeze-win7-x64-Release.zip?job=Environment%3A%20win_runtime%3Dwin7-x64
[9]: https://ci.appveyor.com/api/projects/stratis/breeze/artifacts/breeze_out/breeze-win10-x86-Release.zip?job=Environment%3A%20win_runtime%3Dwin10-x86
[10]: https://ci.appveyor.com/api/projects/stratis/breeze/artifacts/breeze_out/breeze-win10-x64-Release.zip?job=Environment%3A%20win_runtime%3Dwin10-x64
[11]: https://github.com/stratisproject/Breeze/releases/download/cd-unstable/breeze-ubuntu.14.04-x64-Release.zip
[12]: https://github.com/stratisproject/Breeze/releases/download/cd-unstable/breeze-ubuntu.14.04-x64-Release.zip
[13]: https://github.com/stratisproject/Breeze/releases/download/cd-unstable/breeze-osx.10.11-x64-Release.zip
[14]: https://github.com/stratisproject/Breeze/releases/download/cd-unstable/breeze-osx.10.12-x64-Release.zip


