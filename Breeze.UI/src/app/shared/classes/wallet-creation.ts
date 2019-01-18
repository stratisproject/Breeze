export class WalletCreation {

  constructor(name: string, mnemonic: string, password: string, folderPath: string = null ) {
    this.name = name;
    this.mnemonic = mnemonic;
    this.password = password;
    this.folderPath = folderPath;
    this.passphrase = "";
  }

  name: string;
  mnemonic: string;
  password: string;
  folderPath?: string;
  passphrase: string;
}
