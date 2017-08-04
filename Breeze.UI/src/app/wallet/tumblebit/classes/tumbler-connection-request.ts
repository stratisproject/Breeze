export class TumblerConnectionRequest {

  constructor(serverAddress: string, network:string) {
    this.serverAddress = serverAddress;
    this.network = network;
  }

  serverAddress: string;
  network: string;
}
