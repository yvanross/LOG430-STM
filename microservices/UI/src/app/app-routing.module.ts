import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RouterModule, Routes} from "@angular/router";
import {ChaosDashboardComponent} from "./core/chaos-dashboard/chaos-dashboard.component";
import {DashboardComponent} from "./core/dashboard/dashboard.component";

const routes: Routes = [
  { path: 'dashboard', component: DashboardComponent },
  { path: 'chaos-dashboard', component: ChaosDashboardComponent },
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' }
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forRoot(routes),
    CommonModule
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
