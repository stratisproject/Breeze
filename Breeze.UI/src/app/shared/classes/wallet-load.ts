export class WalletLoad {

  constructor(password: string, folderPath: string, name: string) {
    this.password = password;
    this.folderPath = folderPath;
    this.name = name;
  }

  public password: string;
  public folderPath: string;
  public name: string;
}
