export class WalletCreation {

  constructor(password: string, network:string, folderPath: string, name: string) {
    this.password = password;
    this.network = network;
    this.folderPath = folderPath;
    this.name = name;
  }

  password: string;
  network: string;
  folderPath: string;
  name: string;
}
