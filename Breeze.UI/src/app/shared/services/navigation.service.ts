import { Injectable } from '@angular/core';
import { Router, Event, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operator/filter';
import { map } from 'rxjs/operator/map';
import { ReplaySubject } from 'rxJs/ReplaySubject';

export enum Page {
    Bitcoin, Stratis
}

@Injectable()
export class NavigationService {

    private readonly navBase = '/wallet';

    constructor(router: Router) {
        const navigation$ = router.events.filter(x => x instanceof NavigationEnd)
                                         .map(x => <NavigationEnd>x)
                                         .map(x => x.url);

        navigation$.filter(x => x === this.navBase).subscribe(_ => this.pageSubject.next(Page.Bitcoin));
        navigation$.filter(x => x === `${this.navBase}/stratis-wallet`).subscribe(_ => this.pageSubject.next(Page.Stratis));
    }
    
    public pageSubject = new ReplaySubject(1);
}
