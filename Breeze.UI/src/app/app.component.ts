import { Component, OnInit, HostListener } from '@angular/core';
import { Router } from '@angular/router';

import { ApiService } from './shared/services/api.service';

import 'rxjs/add/operator/retryWhen';
import 'rxjs/add/operator/delay';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})

export class AppComponent implements OnInit {
  constructor(private router: Router, private apiService: ApiService) {}
  private errorMessage: any;
  private responseMessage: any;
  private loading: boolean = true;

  ngOnInit() {
    this.apiService.getWalletFiles().retryWhen(errors => errors.delay(2000)).subscribe(() => this.startApp());
  }

  startApp() {
    this.loading = false;
    this.router.navigate(['/login']);
  }

  @HostListener('window:unload')
  unloadHandler() {   
    this.apiService.shutdownNode().subscribe(
          response => {},
          error => {
            if (error.status === 0) {
              alert("Error closing application. please close the dotnet process manually.");
            } else if (error.status >= 400) {
              if (!error.json().errors[0]) {
                console.log(error);
              }
              else {
                alert(error.json().errors[0].message);
              }
            }
          }
        );
  }
}
