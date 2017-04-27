export class WalletLoad {

  constructor(password: string, folderPath: string, name: string) {
    this.password = password;
    this.folderPath = folderPath;
    this.name = name;
  }

  private password: string;
  private folderPath: string;
  private name: string;
}
