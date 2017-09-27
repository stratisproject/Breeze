const electron = require('electron');

// Module to control application life.
const app = electron.app;
// Module to create native browser window.
const BrowserWindow = electron.BrowserWindow;

const path = require('path');
const url = require('url');
const os = require('os');

let serve;
const args = process.argv.slice(1);
serve = args.some(val => val === "--serve");

if (serve) {
  require('electron-reload')(__dirname, {
    electron: require('${__dirname}/../../node_modules/electron')
  });
}

require('electron-context-menu')({
  showInspectElement: serve
});

// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
let mainWindow = null;

function createWindow() {

  // Create the browser window.
  mainWindow = new BrowserWindow({
    width: 1200,
    height: 650,
    frame: true,
    minWidth: 1200,
    minHeight: 650,
    icon: __dirname + "/assets/images/breeze-logo.png",
    title: "Breeze Wallet"
  });

   // and load the index.html of the app.
  mainWindow.loadURL(url.format({
    pathname: path.join(__dirname, '/index.html'),
    protocol: 'file:',
    slashes: true
  }));

  if (serve) {
    mainWindow.webContents.openDevTools();
  }

  // Emitted when the window is closed.
  mainWindow.on('closed', function () {
    // Dereference the window object, usually you would store windows
    // in an array if your app supports multi windows, this is the time
    // when you should delete the corresponding element.
    mainWindow = null;
  });

  // Emitted when the window is going to close.
  mainWindow.on('close', function () {
    closeBitcoinApi(),
    closeStratisApi();
  })
};

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on('ready', function () {
  if (serve) {
    console.log("Breeze UI was started in development mode. This requires the user to be running the Breeze Daemon himself.")
  }
  else {
    startBitcoinApi();
    startStratisApi();
  }
  createTray();
  createWindow();
  if (os.platform() === 'darwin'){
    createMenu();
  }
});

// Quit when all windows are closed.
app.on('window-all-closed', function () {
  // On OS X it is common for applications and their menu bar
  // to stay active until the user quits explicitly with Cmd + Q
  if (process.platform !== 'darwin') {
    //apiProcess.kill();
    app.quit();
  }
});

app.on('activate', function () {
  // On OS X it's common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  if (mainWindow === null) {
    createWindow();
  }
});

function closeBitcoinApi() {
  if (process.platform !== 'darwin' && !serve) {
    var http1 = require('http');
    const options1 = {
      hostname: 'localhost',
      port: 37220,
      path: '/api/node/shutdown',
      method: 'POST'
  };

  const req = http1.request(options1, (res) => {});
  req.write('');
  req.end();
  }
};

function closeStratisApi() {
  if (process.platform !== 'darwin' && !serve) {
    var http2 = require('http');
    const options2 = {
      hostname: 'localhost',
      port: 37221,
      path: '/api/node/shutdown',
      method: 'POST'
    };

  const req = http2.request(options2, (res) => {});
  req.write('');
  req.end();
  }
};

function startBitcoinApi() {
  var bitcoinProcess;
  const execBitcoin = require('child_process').exec;

  //Start Breeze Bitcoin Daemon
  let apiPath = path.join(__dirname, '".//assets//daemon//Breeze.Daemon"');
  if (os.platform() === 'win32') {
      apiPath = path.join(__dirname, '".\\assets\\daemon\\Breeze.Daemon.exe"');
  }

  bitcoinProcess = execBitcoin('"' + apiPath + '" light -testnet', {
      detached: true
  }, (error, stdout, stderr) => {
      if (error) {
          writeLogError(`exec error: ${error}`);
          return;
      }
      if (serve) {
        writeLog(`stdout: ${stdout}`);
        writeLog(`stderr: ${stderr}`);
      }
  });
}

function startStratisApi() {
  var stratisProcess;
  const execStratis = require('child_process').exec;

  //Start Breeze Stratis Daemon
  let apiPath = path.join(__dirname, '".//assets//daemon//Breeze.Daemon"');
  if (os.platform() === 'win32') {
      apiPath = path.join(__dirname, '".\\assets\\daemon\\Breeze.Daemon.exe"');
  }

  stratisProcess = execStratis('"' + apiPath + '" stratis light -testnet', {
      detached: true
  }, (error, stdout, stderr) => {
      if (error) {
          writeLogError(`exec error: ${error}`);
          return;
      }
      if (serve) {
        writeLog(`stdout: ${stdout}`);
        writeLog(`stderr: ${stderr}`);
      }
  });
}

function createTray() {
  //Put the app in system tray
  const Menu = electron.Menu;
  const Tray = electron.Tray;

  let appIcon = null;

var iconPath
if (os.platform() === 'win32') {
  if (serve) {
    iconPath = '.\\src\\assets\\images\\breeze-logo-tray.ico';
  } else {
    iconPath = path.join(__dirname + '\\assets\\images\\breeze-logo-tray.png');
  }

} else {
  if (serve) {
    iconPath = './src/assets/images/breeze-logo-tray.png';
  } else {
    iconPath = path.join(__dirname + '//assets//images//breeze-logo-tray.png');
  }
}

  appIcon = new Tray(iconPath);
  const contextMenu = Menu.buildFromTemplate([{
    label: 'Hide/Show',
    click: function () {
      mainWindow.isVisible() ? mainWindow.hide() : mainWindow.show();
    }
  }]);
  appIcon.setToolTip('Breeze Wallet');
  appIcon.setContextMenu(contextMenu);

  app.on('window-all-closed', function () {
    if (appIcon) appIcon.destroy();
  });
};

function writeLog(msg) {
  console.log(msg);
};

function writeLogError(msg) {
  console.error(msg);
};

function createMenu() {
  const Menu = electron.Menu;

  // Create the Application's main menu
  var menuTemplate = [{
    label: "Application",
    submenu: [
        { label: "About Application", selector: "orderFrontStandardAboutPanel:" },
        { type: "separator" },
        { label: "Quit", accelerator: "Command+Q", click: function() { app.quit(); }}
    ]}, {
    label: "Edit",
    submenu: [
        { label: "Undo", accelerator: "CmdOrCtrl+Z", selector: "undo:" },
        { label: "Redo", accelerator: "Shift+CmdOrCtrl+Z", selector: "redo:" },
        { type: "separator" },
        { label: "Cut", accelerator: "CmdOrCtrl+X", selector: "cut:" },
        { label: "Copy", accelerator: "CmdOrCtrl+C", selector: "copy:" },
        { label: "Paste", accelerator: "CmdOrCtrl+V", selector: "paste:" },
        { label: "Select All", accelerator: "CmdOrCtrl+A", selector: "selectAll:" }
    ]}
  ];

  Menu.setApplicationMenu(Menu.buildFromTemplate(menuTemplate));
};
