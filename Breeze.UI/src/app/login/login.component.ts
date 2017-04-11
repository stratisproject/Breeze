import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GlobalService } from '../shared/services/global.service';
import { ApiService } from '../shared/services/api.service';
import { WalletLoad } from '../shared/classes/wallet-load';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  constructor(private globalService: GlobalService, private apiService: ApiService, private router: Router) { }

  private hasWallet: boolean = false;
  private wallets: [string];
  private password: string;

  ngOnInit() {
    this.apiService.getWalletFiles()
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            let responseMessage = response.json();
            this.wallets = responseMessage.walletsFiles;
            this.globalService.setWalletPath(responseMessage.walletsPath);
            if (this.wallets.length > 0) {
              this.hasWallet = true;
              for (let wallet in this.wallets) {
                this.wallets[wallet] = this.wallets[wallet].slice(0, -5);
              }
              this.globalService.setCurrentWalletName(this.wallets[0]);
            } else {
              this.hasWallet = false;
            }
          }
        },
        error => {
          let errorMessage = <any>error;
          if (error.status >= 400) {
            alert(errorMessage);
            console.log(errorMessage);
          }
        }
      );
  }

  private onSubmit() {
    let walletLoad = new WalletLoad(this.password, this.globalService.getWalletPath(), this.globalService.getCurrentWalletName());

    this.apiService.loadWallet(walletLoad)
      .subscribe(
        response => {
          console.log(response);
          if (response.status >= 200 && response.status < 400) {
            let responseMessage = response.json();
            this.router.navigate(['/wallet']);
          }
        },
        error => {
          let errorMessage = <any>error;
          if (error.status === 403 && error.json().errors[0].message === "Wrong password, please try again.") {
            alert("Wrong password, try again.");
          } else if (error.status >= 400) {
            alert(errorMessage);
            console.log(errorMessage);
          }
        }
      );
  }

  private walletChanged(walletName: string) {
    this.globalService.setCurrentWalletName(walletName);
  }

  private onCreateClicked() {
    this.router.navigate(['/setup']);
  }
}
