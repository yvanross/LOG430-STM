import { Component } from '@angular/core';
import {Subscription} from "rxjs";
import {IngressService} from "../Infrastructure/ingress.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  isAuthenticated = false;

  private readonly authSubscription: Subscription;

  constructor(private ingressService: IngressService) {
    this.authSubscription = this.ingressService.getAuthenticationStatus().subscribe(isAuthenticated => {
      this.isAuthenticated = isAuthenticated.length > 0;
    });
  }

  ngOnDestroy() {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }
}
