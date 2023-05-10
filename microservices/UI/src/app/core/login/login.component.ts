import { Component, OnInit } from '@angular/core';
import {IngressService} from "../../../Infrastructure/ingress.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit
{
  username = '';
  password = '';

  constructor(private ingressService: IngressService) { }

  onSubmit() {
    this.ingressService.login(
      {
        name: this.username,
        secret: this.password
      });
  }

  ngOnInit(): void {
  }

}
