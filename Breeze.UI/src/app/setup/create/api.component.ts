import { Observable } from 'rxjs/Observable';
import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../shared/api/api.service';

@Component({
    selector: 'is-connected',
    template: 'Wallet connected to API: {{result}}',
})

export class ApiComponent implements OnInit {
  result: string;
  constructor(private apiService: ApiService) {}
  ngOnInit() {
    this.apiService
        .isConnected()
        .subscribe((data: string) => this.result = data,
        () => console.log("isConnected() complete from init"));

    if (!this.result) {
      this.result = "false"
    }
  }
}
