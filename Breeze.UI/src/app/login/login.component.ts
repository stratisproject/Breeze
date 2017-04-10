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
  
  private responseMessage: any;
  private errorMessage: any;
  private walletLoad: WalletLoad;

  ngOnInit() {
  }

  private onSubmit() {
    this.walletLoad = new WalletLoad();
    this.walletLoad.password = "123";
    this.walletLoad.name = "test"
    this.walletLoad.folderPath = "folderPath"

    this.apiService.loadWallet(this.walletLoad)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            this.responseMessage = response;
            this.router.navigate['/wallet']
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
}