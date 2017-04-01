import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../shared/api/api.service';

@Component({
  selector: 'dashboard-component',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent {
    constructor(private apiService: ApiService) {}
    
    private balanceResponse: any;
    private confirmedBalance: number;
    private unconfirmedBalance: number;
    private errorMessage: string;

    ngOnInit() {
        this.getWalletBalance();
    }

    private getWalletBalance() {
        this.apiService.getWalletBalance()
            .subscribe(
                response => this.balanceResponse = response,
                error => this.errorMessage = <any>error,
                () => this.setBalance()
        );
    }

    private setBalance() {
        this.confirmedBalance = this.balanceResponse.confirmed;
        this.unconfirmedBalance = this.balanceResponse.unconfirmed;
    }
}
