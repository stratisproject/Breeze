import { Component, OnInit } from '@angular/core';

import { ApiService } from '../../shared/api/api.service'

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
        response => this.transactions = response.history,
        error => this.errorMessage = <any>error,
        () => console.log(this.transactions)
    );
  }
}
