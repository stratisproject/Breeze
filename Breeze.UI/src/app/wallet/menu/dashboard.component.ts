import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../shared/services/api.service';

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
                response =>  {
                    if (response.status >= 200 && response.status < 400) {
                        this.balanceResponse = response.json();
                        this.confirmedBalance = this.balanceResponse.confirmed;
                        this.unconfirmedBalance = this.balanceResponse.unconfirmed;
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
