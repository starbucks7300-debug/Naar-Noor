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
      image: 'assets/cooking-fire.jpg'
    },
    {
      title: 'Grill & BBQ',
      image: 'assets/grilled-food.jpg'
    },
    {
      title: 'Himalayan Mains',
      image: 'assets/hero.webp'
    },
    {
      title: 'Cocktails',
      image: 'assets/cooking-fire.jpg'
    }
  ];
}
