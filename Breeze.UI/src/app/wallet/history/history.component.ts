import { Component, OnInit } from '@angular/core';

import { ApiService } from '../../shared/services/api.service'

@Component({
  selector: 'history-component',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css'],
})

export class HistoryComponent {
  constructor(private apiService: ApiService) {}

  private transactions: any;
  private errorMessage: string;

  ngOnInit() {
    this.getWalletHistory();
  }

  private getWalletHistory() {
    this.apiService.getWalletHistory()
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            this.transactions = response.json().history;
          }
        },
        error => {
          if (error.status >= 400) {
            this.errorMessage = <any>error;
            console.log(this.errorMessage);
          }
        }
    );
  }
}
