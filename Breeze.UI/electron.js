const electron = require('electron')
const dotenv = require('dotenv')
// const edge = require('electron-edge')
// Module to control application life.
const app = electron.app
// Module to create native browser window.
const BrowserWindow = electron.BrowserWindow

const path = require('path')
const url = require('url')

// Require dotenv
dotenv.config();
if (process.env.DEBUG === 'true'){
  // Require electron-reload for dev options
  require('electron-reload')(__dirname);
}

// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
let mainWindow = null;

function createWindow () {
  // Create the browser window.
  mainWindow = new BrowserWindow({width: 1000, height: 600, frame: true, minWidth: 1000, minHeight: 600, icon: "./src/assets/images/stratis-tray.png"})

  if (process.env.DEBUG === 'false'){
    mainWindow.loadURL(url.format({
      pathname: path.join(__dirname, 'index.html'),
      protocol: 'file:',
      slashes: true
    }));
  } else {
    mainWindow.loadURL('http://localhost:4200');
    // Open the DevTools.
    mainWindow.webContents.openDevTools();
  }

  // Emitted when the window is closed.
  mainWindow.on('closed', function () {
    // Dereference the window object, usually you would store windows
    // in an array if your app supports multi windows, this is the time
    // when you should delete the corresponding element.
    mainWindow = null
  })
}

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on('ready', function () {
  createWindow()
  createTray()
})

// Quit when all windows are closed.
app.on('window-all-closed', function () {
  // On OS X it is common for applications and their menu bar
  // to stay active until the user quits explicitly with Cmd + Q
  if (process.platform !== 'darwin') {
    app.quit()
  }
})

app.on('activate', function () {
  // On OS X it's common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  if (mainWindow === null) {
    createWindow()
  }
})

function createTray() {
  //Put the app in system tray
  const Menu = electron.Menu
  const Tray = electron.Tray

  let appIcon = null

  const iconName = process.platform === 'win32' ? './src/assets/images/stratis-tray.png' : './src/assets/images/stratis-tray.png'
  const iconPath = path.join(__dirname, iconName)
  appIcon = new Tray(iconPath)
  const contextMenu = Menu.buildFromTemplate([{
    label: 'Hide/Show',
    click: function () {
      mainWindow.isVisible() ? mainWindow.hide() : mainWindow.show();
    }
  }])
  appIcon.setToolTip('Breeze Wallet')
  appIcon.setContextMenu(contextMenu)

  app.on('window-all-closed', function () {
    if (appIcon) appIcon.destroy()
  })
}
