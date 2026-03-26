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
      image: 'https://hoirqrkdgbmvpwutwuwj.supabase.co/storage/v1/object/public/assets/assets/917d6f93-fb36-439a-8c48-884b67b35381_1600w.jpg'
    },
    {
      date: 'Oct 05, 2026',
      title: '5 Must-Try Dishes at Naar & Noor',
      excerpt: 'A curated guide to navigating our menu, from classic Momos to our signature flame-grilled platters.',
      image: 'https://images.unsplash.com/photo-1563379926898-05f4575a45d8?q=80&w=2070&auto=format&fit=crop'
    },
    {
      date: 'Sep 28, 2026',
      title: 'The Art of Fire-Grilled Cooking',
      excerpt: 'Why we believe cooking over an open flame is the only way to truly unlock the depth of our ingredients.',
      image: 'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?q=80&w=1974&auto=format&fit=crop'
    }
  ];
}
