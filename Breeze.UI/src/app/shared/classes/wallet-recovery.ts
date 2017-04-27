export class WalletRecovery {

  constructor(mnemonic: string, password: string, network:string, folderPath: string, name: string) {
    this.mnemonic = mnemonic;
    this.password = password;
    this.network = network;
    this.folderPath = folderPath;
    this.name = name;
  }

  mnemonic: string;
  password: string;
  folderPath: string;
  name: string;
  network: string;
}
