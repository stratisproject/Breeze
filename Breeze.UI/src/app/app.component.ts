import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Title } from '@angular/platform-browser';

import { ApiService } from './shared/services/api.service';

import { remote } from 'electron';

import 'rxjs/add/operator/retryWhen';
import 'rxjs/add/operator/delay';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})

export class AppComponent implements OnInit {
  constructor(private router: Router, private apiService: ApiService, private titleService: Title) {}
  private errorMessage: any;
  private responseMessage: any;
  public loading: boolean = true;

  ngOnInit() {
    this.setTitle();
    this.apiService.getWalletFiles().retryWhen(errors => errors.delay(2000)).subscribe(() => this.checkStratisDaemon());
  }

  private checkStratisDaemon() {
    this.apiService.getStratisWalletFiles().retryWhen(errors => errors.delay(2000)).subscribe(() => this.startApp());
  }

  private startApp() {
    this.loading = false;
    this.router.navigate(['/login']);
  }

  private setTitle() {
    let applicationName = "Breeze Wallet";
    let applicationVersion = remote.app.getVersion();
    let releaseCycle = "beta";
    let newTitle = applicationName + " v" + applicationVersion + " " + releaseCycle;
    this.titleService.setTitle(newTitle);
  }
}
