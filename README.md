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
git submodule init
git submodule update

# Go in Breeze's solution folder
cd Breeze
dotnet restore
dotnet build

# Run a daemon Bitcoin SPV node on testnet
cd src/Breeze.Daemon
dotnet run light -testnet
```