export class TumbleRequest {

  constructor(originWalletName: string, destinationWalletName: string) {
    this.OriginWalletName = originWalletName;
    this.DestinationWalletName = destinationWalletName;
  }

  OriginWalletName: string;
  DestinationWalletName: string;
}
