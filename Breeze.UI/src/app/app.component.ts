import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { ApiService } from './shared/services/api.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})

export class AppComponent implements OnInit {
  constructor(private router: Router, private apiService: ApiService) {}
  private errorMessage: any;
  private responseMessage: any;

  ngOnInit() {
    this.router.navigate(['/login']);
  }
}
