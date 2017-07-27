# Breeze-UI

Graphical User Interface for Stratis Breeze Wallet.

## Getting Started

Clone this repository locally:

``` bash
git clone https://www.github.com/stratisproject/breeze.git
```

Navigate to the Breeze UI in a terminal:
``` bash
cd ./Breeze/Breeze.UI
```

## Install Nodejs (ver 8.x) and npm:

Using Ubuntu
```bash
curl -sL https://deb.nodesource.com/setup_8.x | sudo -E bash -
sudo apt-get install -y nodejs
```
Using Debian, as root
```bash
curl -sL https://deb.nodesource.com/setup_8.x | bash -
apt-get install -y nodejs
```
## Install/Update Nodejs/npm to stable/latest release version:

```bash
sudo npm install npm@latest -g
sudo npm install n -g
sudo n stable
```
*To install the latest release, use `n latest`. Alternatively, you can run n #.#.# to get a specific Node version.

## Install dependencies with npm:

From within Breeze.UI directory run:

``` bash
npm install
```

There is an issue with `yarn` and `node_modules` that are only used in electron on the backend when the application is built by the packager. Please use `npm` as dependencies manager.

If you want to generate Angular components with Angular-cli , you **MUST** install `@angular/cli` in npm global context.  
Please follow [Angular-cli documentation](https://github.com/angular/angular-cli) if you had installed a previous version of `angular-cli`.

``` bash
npm install -g @angular/cli
```

## To build for development

- **in a terminal window** -> npm start
This will compile the Angular code and spawn the Electron process in parallel.
After compilation has completed the Electron UI will refresh.

If you want to seperate the build process from the Electron process you can use:
- **in a terminal window** -> npm start:webpack
- **in another terminal window** -> npm run electron:serve

## To build for production

- Using development variables (environments/index.ts) :  `npm run electron:dev`
- Using production variables (environments/index.prod.ts) :  `npm run electron:prod`

Your built files are in the /dist folder.

## Included Commands

|Command|Description|
|--|--|
|`npm run start:web`| Execute the app in the brower |
|`npm run package:linux`| Builds your application and creates an app consumable on linux system |
|`npm run package:windows`| On a Windows OS, builds your application and creates an app consumable in windows 32/64 bit systems |
|`npm run package:mac`|  On a MAC OS, builds your application and generates a `.app` file of your application that can be run on Mac |

**The application is optimised. Only the files of /dist folder are included in the executable.**
