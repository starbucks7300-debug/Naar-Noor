import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService, MenuItem } from '../../services/api.service';

interface MenuItemView {
  name: string;
  price: string;
  description: string;
  category: string;
  isVegetarian: boolean;
  isVegan: boolean;
}

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {
  private readonly api = inject(ApiService);

  menuItems: MenuItemView[] = [];
  loading = true;
  error = false;

  ngOnInit(): void {
    this.api.getMenu().subscribe({
      next: (items: MenuItem[]) => {
        this.menuItems = items.map(item => ({
          name: item.name,
          price: `£${item.price.toFixed(2)}`,
          description: item.description,
          category: item.category,
          isVegetarian: item.isVegetarian,
          isVegan: item.isVegan
        }));
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }
}
