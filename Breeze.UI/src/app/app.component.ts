import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { remote } from 'electron';

import { ApiService } from './shared/api/api.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})

export class AppComponent implements OnInit {
  constructor(private router: Router, private apiService: ApiService) {}
  private errorMessage: string;
  private response: any;
  private isConfigured: boolean = true;

  ngOnInit() {
    this.checkWalletStatus();
  }

  private checkWalletStatus(){
    this.apiService.getWalletStatus()
      .subscribe(
        response => this.response = response.success,
        error => this.errorMessage = <any>error
      );

    if (this.response = "true") {
      // remote.dialog.showMessageBox({message: remote.app.getPath('userData')})
      this.router.navigate(['/wallet'])
    } else {
      this.router.navigate(['/setup'])
    }
  }

  private hasWallet() {
    return true;
  }
}