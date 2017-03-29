import { Component } from '@angular/core';
import { ApiService } from '../../shared/api/api.service';

@Component({
  selector: 'send-component',
  templateUrl: './send.component.html',
  styleUrls: ['./send.component.css'],
})

export class SendComponent {
  constructor(private apiService: ApiService) {}
    
}
