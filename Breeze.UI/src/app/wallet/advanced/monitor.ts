import { Observable } from 'rxjs/Observable';

import { LoadingState } from './loadingState';

function monitor<T>(this: Observable<T>, loadingState: LoadingState): Observable<T> {
    return Observable.create(observer => {
        loadingState.loading = true;
        loadingState.errored = false;
        const subs = this.subscribe(x => {
            loadingState.loading = false;
            observer.next(x);
        }, e => {
            loadingState.loading = false;
            loadingState.errored = true;
            observer.error(e);
        }, () => {
            loadingState.loading = false;
            observer.complete();
        });
        return () => subs.unsubscribe();
    });
  };

  Observable.prototype.monitor = monitor;

  declare module 'rxjs/Observable' {
    interface Observable<T> {
        monitor: typeof monitor;
    }
}
