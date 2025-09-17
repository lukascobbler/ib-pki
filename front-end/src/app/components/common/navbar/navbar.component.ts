import {Component, OnInit} from '@angular/core';
import {NgClass, NgFor, NgIf, NgOptimizedImage} from '@angular/common';
import {RouterLink, RouterLinkActive} from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    NgIf,
    RouterLink,
    RouterLinkActive,
    NgOptimizedImage,
    NgClass,
    NgFor
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit{
  currentUserRole: string = 'ADMIN'; // 'EE_USER', 'CA_USER', or 'ADMIN'
  sidebarItems: any[];

  constructor() {
    this.sidebarItems = [
      {label: 'My certificates', icon: 'library_books', link: '/my-certificates', roles: ['EE_USER']},
      {label: 'Signed certificates', icon: 'library_books', link: '/signed-certificates', roles: ['CA_USER']},
      {label: 'All certificates', icon: 'library_books', link: '/all-certificates', roles: ['ADMIN']},
      {label: 'Request Certificate', icon: 'add_notes', link: '/request-certificate', roles: ['EE_USER']},
      {label: 'Certificate requests', icon: 'stacks', link: '/certificate-requests', roles: ['CA_USER']},
      {label: 'Issue a certificate', icon: 'add_notes', link: '/issue-certificate', roles: ['CA_USER', 'ADMIN']},
      {label: 'My certificates', icon: 'home_storage', link: '/my-certificates', roles: ['CA_USER']},
      {label: 'Manage CA users', icon: 'group', link: '/manage-ca-users', roles: ['ADMIN']},
      {label: 'Logout', icon: 'logout', roles: ['EE_USER', 'CA_USER', 'ADMIN'], class: 'logout', action: () => this.logout()}
    ];
  }

  ngOnInit() {
    // this.currentUserRole = this.authService.getCurrentUserRole();
  }

  logout() {
    // this.authService.logout();
  }
}
