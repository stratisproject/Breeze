export class WalletRecovery {

  constructor(walletName: string, mnemonic: string, password: string, network:string, creationDate: Date, folderPath: string = null) {
    this.name = walletName;
    this.mnemonic = mnemonic;
    this.password = password;
    this.network = network;
    this.creationDate = creationDate;
    this.folderPath = folderPath;
  }

  mnemonic: string;
  password: string;
  name: string;
  network: string;
  creationDate: Date;
  folderPath?: string;
}
