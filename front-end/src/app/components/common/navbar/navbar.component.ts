import {Component, OnInit} from '@angular/core';
import {NgClass, NgFor, NgIf, NgOptimizedImage} from '@angular/common';
import {Router, RouterLink, RouterLinkActive} from '@angular/router';
import { Role } from '../../../models/Role';
import { AuthService } from '../../../services/auth/auth.service';

type NavItem = {
  label: string;
  icon: string;
  link?: string;
  roles: Role[];
  class?: string;
  action?: () => void;
};

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
  currentUserRole: Role | null = null;
  sidebarItems: NavItem[] = [
    // EeUser
    { label: 'My certificates', icon: 'library_books', link: '/my-certificates', roles: ['EeUser', 'CaUser'] },
    { label: 'Request Certificate', icon: 'add_notes', link: '/request-certificate', roles: ['EeUser'] },

    // CaUser
    { label: 'Signed certificates', icon: 'library_books', link: '/signed-certificates', roles: ['CaUser'] },
    { label: 'Certificate requests', icon: 'stacks', link: '/certificate-requests', roles: ['CaUser'] },
    { label: 'Issue a certificate', icon: 'add_notes', link: '/issue-certificate', roles: ['CaUser', 'Admin'] },

    // Admin
    { label: 'All certificates', icon: 'library_books', link: '/all-certificates', roles: ['Admin'] },
    { label: 'Manage CA users', icon: 'group', link: '/manage-ca-users', roles: ['Admin'] },

    // Common
    { label: 'Logout', icon: 'logout', roles: ['EeUser', 'CaUser', 'Admin'], class: 'logout', action: () => this.logout() },
  ];

  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit() {
    this.currentUserRole = (this.auth.role as Role) ?? null;
    this.auth.role$.subscribe(r => (this.currentUserRole = (r as Role) ?? null));
  }

  logout() {
    this.auth.logout().subscribe({
      complete: () => this.router.navigate(['/login']),
      error: () => this.router.navigate(['/login']),
    });
  }
}
