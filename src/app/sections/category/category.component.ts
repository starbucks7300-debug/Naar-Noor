import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-category',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent {
  categories = [
    {
      title: 'Starters',
      image: 'https://images.unsplash.com/photo-1541529086526-db283c563270?q=80&w=2070&auto=format&fit=crop'
    },
    {
      title: 'Grill & BBQ',
      image: 'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?q=80&w=1974&auto=format&fit=crop'
    },
    {
      title: 'Himalayan Mains',
      image: 'https://images.unsplash.com/photo-1631515243349-e0cb75fb8d3a?q=80&w=2088&auto=format&fit=crop'
    },
    {
      title: 'Cocktails',
      image: 'https://images.unsplash.com/photo-1514362545857-3bc16c4c7d1b?q=80&w=2070&auto=format&fit=crop'
    }
  ];
}
