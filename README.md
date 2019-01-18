| Windows | Mac OS | Linux
| :---- | :------ | :---- |
| [![Build status](https://dev.azure.com/StratisProject/Breeze/_apis/build/status/Hosted%20Windows%20Container?branchName=master)](https://dev.azure.com/StratisProject/Breeze/_build/latest?definitionId=10) | [![Build status](https://dev.azure.com/StratisProject/Breeze/_apis/build/status/Hosted%20macOS?branchName=master)](https://dev.azure.com/StratisProject/Breeze/_build/latest?definitionId=12) | [![Build status](https://dev.azure.com/StratisProject/Breeze/_apis/build/status/Hosted%20Ubuntu%201604?branchName=master)](https://dev.azure.com/StratisProject/Breeze/_build/latest?definitionId=11)

# Breeze

__Warning: We're still in beta, so use at your own risk.__
This is the repository of the Breeze Wallet, the first full-block SPV bitcoin wallet using Angular and Electron at the front-end and C# with .NET Core in the back-end.

## Daemon Build

Breeze daemon is the backend REST service, hosting a Bitcoin node upon which Breeze UI depends:

```
# Clone and go in the directory
git clone https://github.com/stratisproject/Breeze
cd Breeze

# Initialize dependencies
git submodule update --init --recursive

# Go in the Breeze deamon folder
cd StratisBitcoinFullNode/src/Stratis.BreezeD
dotnet build

# Run the Bitcoin and Stratis full-SPV daemons on testnet in separate terminals
dotnet run -testnet
dotnet run stratis -testnet
```

## UI Build

[Read more...](https://github.com/stratisproject/Breeze/blob/master/Breeze.UI/README.md)

## CI Build
-----------

Every time someone pushes to the master branch or create a pull request on it, a build is triggered and a new unstable app release is created.

If you want the :sparkles: latest :sparkles: (unstable :bomb:) version of the Breeze app, you can get it here: 

https://github.com/stratisproject/Breeze/releases/tag/Continuous-Delivery

