import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions, URLSearchParams} from '@angular/http';
    
@Injectable()
export class TumblebitService {
    // The service to connect to & operate a TumbleBit Server via the 
    // TumbleBit.Client.CLI tool
    constructor(private http: Http) {};

    private tumblerClientUrl = 'http://localhost/api';
    private headers = new Headers({'Content-Type': 'application/json'});
    
    private tumblerParameters;

    connect(): void {}; // stub
    tumble(): void {}; // stub
}