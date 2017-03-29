import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})

export class AppComponent implements OnInit {
  constructor(private router: Router) {}
  private isConfigured: boolean = true;

  private checkConfigured(){
    if (this.isConfigured) {
      this.router.navigateByUrl('/wallet')
    } else {
      this.router.navigateByUrl('/setup')
    }
  }

  ngOnInit() {
    this.checkConfigured();
  }

}