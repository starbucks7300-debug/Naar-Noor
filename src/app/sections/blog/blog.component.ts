import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-blog',
  standalone: true,
  imports: [CommonModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './blog.component.html',
  styleUrls: ['./blog.component.css']
})
export class BlogComponent {
  posts = [
    {
      date: 'Oct 12, 2026',
      title: 'The Story Behind Himalayan Flavors',
      excerpt: 'Discover the rich history and cultural significance of the spices that define our unique menu offerings.',
      image: 'assets/cooking-fire.jpg'
    },
    {
      date: 'Oct 05, 2026',
      title: '5 Must-Try Dishes at Naar & Noor',
      excerpt: 'A curated guide to navigating our menu, from classic Momos to our signature flame-grilled platters.',
      image: 'assets/grilled-food.jpg'
    },
    {
      date: 'Sep 28, 2026',
      title: 'The Art of Fire-Grilled Cooking',
      excerpt: 'Why we believe cooking over an open flame is the only way to truly unlock the depth of our ingredients.',
      image: 'assets/hero.webp'
    }
  ];
}
