export class WalletRecovery {

  constructor(mnemonic: string, password: string, network:string, folderPath: string, walletName: string, creationDate: Date) {
    this.mnemonic = mnemonic;
    this.password = password;
    this.network = network;
    this.folderPath = folderPath;
    this.name = walletName;
    this.creationDate = creationDate;
  }

  mnemonic: string;
  password: string;
  folderPath: string;
  name: string;
  network: string;
  creationDate: Date;
}
