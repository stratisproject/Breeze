import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../shared/api/api.service'
import { WalletRecovery } from '../../shared/wallet-recovery'

@Component({
  selector: 'app-recover',
  templateUrl: './recover.component.html',
  styleUrls: ['./recover.component.css']
})
export class RecoverComponent implements OnInit {

  constructor(private apiService: ApiService) { }
  private walletRecovery: WalletRecovery;
  private responseBody: string;

  ngOnInit() {
  }

  private recoverWallet(mnemonic: string, password: string, folderPath: string, name: string, network: string) {
    this.walletRecovery.mnemonic = mnemonic;
    this.walletRecovery.password = password;
    this.walletRecovery.folderPath = folderPath;
    this.walletRecovery.name = name;
    this.walletRecovery.network = network;

    this.apiService
      .recoverWallet(this.walletRecovery)
      .subscribe((response: string) => this.responseBody = response,
      () => console.log("recoverWallet() completed"));
  }
}