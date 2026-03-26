import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [CommonModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './reviews.component.html',
  styleUrls: ['./reviews.component.css']
})
export class ReviewsComponent {
  reviews = [
    {
      text: 'An unforgettable dining experience. The Momo selection was incredible, and the atmosphere feels both exclusive and welcoming.',
      author: 'James T.'
    },
    {
      text: 'The ambiance is warm and premium. The flame-grilled lamb chops were arguably the best I\'ve ever had. Highly recommend booking in advance.',
      author: 'Sarah L.'
    },
    {
      text: 'Best Himalayan food I\'ve ever had outside of Kathmandu. The dark, moody aesthetic makes it perfect for date night.',
      author: 'Michael R.'
    }
  ];
}
