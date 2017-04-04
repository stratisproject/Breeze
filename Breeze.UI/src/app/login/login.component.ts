import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../shared/api/api.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  constructor(private apiService: ApiService, private router: Router) { }
  
  private response: any;
  private errorMessage: string;

  ngOnInit() {
  }

  private onSubmit() {
    this.apiService.loadWallet("123")
      .subscribe(
        response => this.response = response,
        error => this.errorMessage = error,
        () => this.loadWallet()
      );
  }

  private loadWallet() {
    if (this.response.success === "true") {
      this.router.navigate(['/wallet/send']);

    } else {
      alert("Something went wrong.")
    }
  }
}
