export class WalletLoad {

  constructor(name: string, password: string, folderPath: string = null ) {
    this.name = name;
    this.password = password;
    this.folderPath = folderPath;
  }

  public name: string;
  public password: string;
  public folderPath?: string;
}
