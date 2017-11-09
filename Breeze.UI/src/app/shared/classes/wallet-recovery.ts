export class WalletRecovery {

  constructor(walletName: string, mnemonic: string, password: string, creationDate: Date, folderPath: string = null) {
    this.name = walletName;
    this.mnemonic = mnemonic;
    this.password = password;
    this.creationDate = creationDate;
    this.folderPath = folderPath;
  }

  mnemonic: string;
  password: string;
  name: string;
  creationDate: Date;
  folderPath?: string;
}
