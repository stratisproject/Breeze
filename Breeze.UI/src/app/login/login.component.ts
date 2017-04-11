import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../shared/api/api.service';
import { WalletLoad } from '../shared/wallet-load';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  constructor(private apiService: ApiService, private router: Router) { }

  private walletLoad: WalletLoad;
  private hasWallet: boolean = false;
  private currentWalletName: string;
  private wallets: [any];
  private walletPath: string;
  
  private responseMessage: any;
  private errorMessage: any;
  

  ngOnInit() {
    this.currentWalletName = "";
    this.apiService.getWalletFiles()
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            this.responseMessage=response;
            this.wallets = response.json().walletsFiles;
            this.walletPath = response.json().walletsPath;
            if (this.wallets.length > 0) {
              this.hasWallet = true;
              this.currentWalletName = this.wallets[0].slice(0, -5);
            } else {
              this.hasWallet = false;
            }
          }
        },
        error => {
          this.errorMessage = <any>error;
          if (error.status >= 400) {
            alert(this.errorMessage);
            console.log(this.errorMessage);
          }
        }
      );
  }

  private onSubmit() {

    this.walletLoad = new WalletLoad();
    this.walletLoad.password = "test";
    this.walletLoad.name = this.currentWalletName;
    this.walletLoad.folderPath = this.walletPath;

    console.log(this.walletLoad);

    this.apiService.loadWallet(this.walletLoad)
      .subscribe(
        response => {
          console.log(response);
          if (response.status >= 200 && response.status < 400) {
            this.responseMessage = response;
            this.router.navigate(['/wallet']);
          }
        },
        error => {
          this.errorMessage = <any>error;
          if (error.status >= 400) {
            alert(this.errorMessage);
            console.log(this.errorMessage);
          }
        }
      );
  }

  private walletChanged(walletName: string) {
    let walletNameNoJson: string = walletName.slice(0, -5);
    this.currentWalletName = walletNameNoJson;
  }

  private clickedCreate() {
    this.router.navigate(['/setup']);
  }
}